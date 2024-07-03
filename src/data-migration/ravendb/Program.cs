using Google.Api.Gax;
using Google.Cloud.Firestore;
using models;
using Raven.Client.Documents;

Console.WriteLine("Migrating RavenDB to firestore...");

var store = new DocumentStore() {
    Urls = ["http://localhost:3000"],
    Database = "export"
}.Initialize();

using var session = store.OpenSession();
// query for all raven accounts
var ravenAccounts = session.Query<RavenAccount>(null, "Accounts", false).ToList();
var totalAccounts = ravenAccounts.Count;
ravenAccounts = ravenAccounts.Where(x => x.KeyQuota.KeysUsed > 0).ToList();

Console.WriteLine($"Removed {totalAccounts - ravenAccounts.Count} accounts without keys");

// list any duplicate emails which will cause problems. only one account per email is allowed
foreach (var duplicate in ravenAccounts.GroupBy(x => x.Email).Where(x => x.Count() > 1)) {
    // get the user with the most keys
    var max = duplicate.OrderByDescending(x => x.KeyQuota.KeysUsed).First();
    // get the other users
    var others = duplicate.Where(x => x.Id != max.Id).ToList();
    Console.WriteLine($"Duplicate email: {duplicate.Key} migrating keys to {max.Id}");

    // loop over other users and update the keys to the max user
    foreach (var user in others) {
        var keys = session.Query<RavenApiKey>(null, "ApiKeys", false)
            .Where(x => !x.Deleted && x.AccountId == user.Id).ToList();

        foreach (var key in keys) {
            key.AccountId = max.Id;
        }

        // remove the duplicate user
        session.Delete(user);
    }

    session.SaveChanges();
}

var client = new FirestoreDbBuilder {
    ProjectId = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") switch {
        "Development" => "ut-dts-agrc-web-api-dev",
        "Release" => "ut-dts-agrc-web-api-prod",
        _ => "ut-dts-agrc-web-api-dev"
    },
    // EmulatorDetection = EmulatorDetection.EmulatorOnly
}.Build();

var batch = client.StartBatch();

var clientCreatedKeyMap = new Dictionary<string, List<FirestoreApiKey>>();
var elevatedAccounts = new[] { "sgourley@utah.gov", "api-explorer@utah.gov", "mpeters@utah.gov" };
var elevatedKeys = new[] { "agrc-apiexplorer", "agrc-dev", "agrc-plssaddin", "agrc-uptime" };
var quota = 500;
var items = 1;

var keyCollection = client.Collection("keys");
var accountCollection = client.Collection("clients-unclaimed");

// query for all keys from raven
var ravenKeys = session.Query<RavenApiKey>(null, "ApiKeys", false).ToList();
var totalKeys = ravenKeys.Count;
ravenKeys = ravenKeys.Where(x => !x.Deleted).ToList();

Console.WriteLine($"Removed {totalKeys - ravenKeys.Count} deleted keys");

foreach (var ravenKey in ravenKeys) {
    // submit keys if the batch size is met
    batch = await SubmitBatchIfFull(batch);
    // convert to firestore model
    var fireStoreKey = new FirestoreApiKey(ravenKey, elevatedKeys.Contains(ravenKey.Key.ToLowerInvariant()));
    // add key to map of account id to list of keys
    AddOrUpdate(ravenKey.AccountId, fireStoreKey);
    // create firestore document in /keys/agrc-000 collection
    var document = keyCollection.Document(fireStoreKey.Key);
    var snapshot = await document.GetSnapshotAsync();
    // if key already exists skip to next key
    if (snapshot.Exists) {
        Console.WriteLine("    skipping " + fireStoreKey.Key);
        continue;
    }

    // add create operation to batch
    batch.Create(document, fireStoreKey);
}

foreach (var ravenAccount in ravenAccounts) {
    // submit accounts if the batch size is met
    batch = await SubmitBatchIfFull(batch);
    // convert to firestore model
    var unclaimed = new Client(ravenAccount);
    // create firestore document in /clients-unclaimed/email@address collection
    var document = accountCollection.Document(unclaimed.Email);
    // add create operation to batch
    batch.Create(document, unclaimed);
    // if account has no keys skip to next account
    if (!clientCreatedKeyMap.TryGetValue(ravenAccount.Id, out var userCreatedKeys)) {
        continue;
    }
    // create sub collection for /clients-unclaimed/keys
    var subCollection = document.Collection("keys");

    foreach (var key in userCreatedKeys) {
        // submit keys if the batch size is met
        batch = await SubmitBatchIfFull(batch);
        // create firestore document in /clients-unclaimed/email@address/keys/agrc-000 collection
        // get firestore document in /keys/agrc-000 collection
        var keyDocument = keyCollection.Document(key.Key);
        // submit keys if the batch size is met
        batch = await SubmitBatchIfFull(batch);
        batch.Update(keyDocument, new Dictionary<string, object> { { "accountId", ravenAccount.Email } });
        // submit keys if the batch size is met
        batch = await SubmitBatchIfFull(batch);
    }
}

await batch.CommitAsync();
Console.WriteLine("--------------------");
Console.WriteLine("Accounts: " + ravenAccounts.Count);
Console.WriteLine("Keys: " + ravenKeys.Count);
Console.WriteLine("Key owning accounts: " + clientCreatedKeyMap.Count);
try {
    Console.WriteLine("Average keys per account: " + clientCreatedKeyMap.Values.Average(x => x.Count));
    Console.WriteLine("Most keys for an account: " + clientCreatedKeyMap.Values.Max(x => x.Count));
} catch { }

void AddOrUpdate(string key, FirestoreApiKey value) {
    if (clientCreatedKeyMap.TryGetValue(key, out var list)) {
        list.Add(value);
    } else {
        clientCreatedKeyMap.Add(key, [value]);
    }
}

async Task<WriteBatch> SubmitBatchIfFull(WriteBatch batch) {
    if (items++ % quota == 0) {
        Console.WriteLine($"    writing batch {items / quota}");
        await batch.CommitAsync();

        return client.StartBatch();
    }

    return batch;
}

namespace models {
    [FirestoreData(ConverterType = typeof(FirestoreEnumIgnoreCaseNameConverter<ApplicationStatus>))]
    public enum ApplicationStatus {
        Development,
        Production,
        None
    }
    [FirestoreData(ConverterType = typeof(FirestoreEnumIgnoreCaseNameConverter<ApplicationType>))]
    public enum ApplicationType {
        None,
        Browser,
        Server
    }
    [FirestoreData(ConverterType = typeof(FirestoreEnumIgnoreCaseNameConverter<KeyStatus>))]
    public enum KeyStatus {
        None,
        Active,
        Disabled
    }
    public class FirestoreEnumIgnoreCaseNameConverter<T> : IFirestoreConverter<T>
        where T : struct, Enum {
        /// <inheritdoc />
        public T FromFirestore(object value) {
            var name = (string)value;

            return Enum.TryParse<T>(name, true, out var result)
                ? result
                : throw new ArgumentException($"Unknown name {name} for enum {typeof(T).FullName}");
        }

        /// <inheritdoc />
        public object ToFirestore(T value) => value.ToString().ToLowerInvariant();
    }
    [FirestoreData]
    public class FirestoreApiKey {
        // required for firestore
        public FirestoreApiKey() { }
        public FirestoreApiKey(RavenApiKey apiKey, bool elevated) {
            AccountId = string.Empty;
            Key = apiKey.Key.ToLowerInvariant();
            Created = new DateTime(apiKey.CreatedAtTicks).ToUniversalTime();
            Pattern = apiKey.Pattern;
            RegularExpression = apiKey.RegexPattern;
            MachineName = apiKey.IsMachineName;
            Elevated = elevated;
            Notes = "👻️👻️👻️ an unclaimed key 👻️👻️👻️";
            Claimed = false;

            Flags = new Dictionary<string, bool>() {
                { "deleted", apiKey.Deleted },
                { "disabled", apiKey.ApiKeyStatus == KeyStatus.Disabled },
                { "server", apiKey.Type == ApplicationType.Server },
                { "production", apiKey.AppStatus == ApplicationStatus.Production}
            };
        }
        [FirestoreProperty("accountId")] public string AccountId { get; set; } = string.Empty;
        [FirestoreProperty("key")] public string Key { get; set; } = string.Empty;
        [FirestoreProperty("created")] public DateTime Created { get; set; }
        [FirestoreProperty("pattern")] public string? Pattern { get; set; }
        [FirestoreProperty("regularExpression")] public string? RegularExpression { get; set; }
        [FirestoreProperty("machineName")] public bool MachineName { get; set; }
        [FirestoreProperty("elevated")] public bool Elevated { get; set; }
        [FirestoreProperty("notes")]
        public string Notes { get; set; } = string.Empty;
        [FirestoreProperty("claimed")] public bool Claimed { get; set; }
        [FirestoreProperty("flags")]
        public Dictionary<string, bool> Flags { get; set; } = new Dictionary<string, bool>() {
                { "deleted", false },
                { "disabled", false },
                { "server", false },
                { "production", false }
        };
    }
    [FirestoreData]
    public class Client {
        // required for firestore
        public Client() { }
        public Client(RavenAccount account) {
            Email = account.Email.ToLowerInvariant();
            Password = account.Password.HashedPassword;
            Salt = account.Password.Salt;
        }
        [FirestoreProperty("email")] public string Email { get; set; } = null!;
        [FirestoreProperty("password")] public string Password { get; set; } = null!;
        [FirestoreProperty("salt")] public string Salt { get; set; } = null!;
    }
    public class RavenApiKey {
        public string AccountId { get; set; } = null!;
        public string Key { get; set; } = null!;
        public long CreatedAtTicks { get; set; }
        public KeyStatus ApiKeyStatus { get; set; }
        public ApplicationType Type { get; set; }
        public ApplicationStatus AppStatus { get; set; }
        public string Pattern { get; set; } = null!;
        public string RegexPattern { get; set; } = null!;
        public bool IsMachineName { get; set; }
        public bool Deleted { get; set; }
    }
    public class RavenAccount {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public Quota KeyQuota { get; set; } = new();
        public PasswordHashAndSalt Password { get; set; } = null!;
    }
    public class Quota {
        public int KeysUsed { get; set; }
    }
    public class PasswordHashAndSalt {
        public string HashedPassword { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }
}
