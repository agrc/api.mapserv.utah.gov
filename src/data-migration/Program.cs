using Google.Api.Gax;
using Google.Cloud.Firestore;
using models;
using Raven.Client.Documents;

Console.WriteLine("Migrating RavenDB to firestore...");

var store = new DocumentStore() {
    Urls = new[] { "http://localhost:3000" },
    Database = "wsut"
}.Initialize();

var client = new FirestoreDbBuilder {
    ProjectId = "ut-dts-agrc-web-api-dev",
    EmulatorDetection = EmulatorDetection.EmulatorOnly
}.Build();

using var session = store.OpenSession();
var batch = client.StartBatch();

var clientCreatedKeyMap = new Dictionary<string, List<ApiKey>>();
var elevatedAccounts = new[] { "sgourley@utah.gov", "api-explorer@utah.gov", "mpeters@utah.gov" };
var elevatedKeys = new[] { "agrc-apiexplorer", "agrc-dev", "agrc-plssaddin", "agrc-uptime" };
var quota = 500;
var items = 1;

var keys = session.Query<ApiKey>().ToList();
var accounts = session.Query<Account>().ToList();

var keyCollection = client.Collection("keys");
var accountCollection = client.Collection("unclaimed-accounts");

foreach (var key in keys) {
    key.Key = key.Key.ToLowerInvariant();
    key.Elevated = elevatedKeys.Contains(key.Key);

    AddOrUpdate(key.AccountId, key);

    batch = await SubmitBatchIfFull(batch);

    var document = keyCollection.Document(key.Key);
    batch.Create(document, key);
}

foreach (var duplicate in accounts.GroupBy(x => x.Email).Where(x => x.Count() > 1)) {
    Console.WriteLine($"Duplicate email: {duplicate.Key}");
}

foreach (var account in accounts) {
    batch = await SubmitBatchIfFull(batch);
    account.Email = account.Email.ToLowerInvariant();

    account.Elevated = elevatedAccounts.Contains(account.Email);

    var document = accountCollection.Document(account.Email);
    batch.Create(document, account);

    if (!clientCreatedKeyMap.TryGetValue(account.Id, out var userCreatedKeys)) {
        continue;
    }

    var subCollection = document.Collection("keys");

    foreach (var key in userCreatedKeys) {
        batch = await SubmitBatchIfFull(batch);

        var subDocument = subCollection.Document(key.Key);
        var keyDocument = keyCollection.Document(key.Key);

        batch.Create(subDocument, new {
            link = keyDocument,
            key = key.Key,
            deleted = key.Deleted,
            status = key.ApiKeyStatus,
            type = key.Type,
            pattern = key.Pattern
        });

        batch = await SubmitBatchIfFull(batch);

        batch.Set(keyDocument, new {
            created_by = new {
                email = account.Email,
                account = document,
            }
        }, SetOptions.MergeAll);
    }
}

await batch.CommitAsync();

Console.WriteLine("Accounts: " + accounts.Count);
Console.WriteLine("Keys: " + keys.Count);
Console.WriteLine("Key owning accounts: " + clientCreatedKeyMap.Count);
Console.WriteLine("Average keys per account: " + clientCreatedKeyMap.Values.Average(x => x.Count));
Console.WriteLine("Most keys for an account: " + clientCreatedKeyMap.Values.Max(x => x.Count));

void AddOrUpdate(string key, ApiKey value) {
    if (clientCreatedKeyMap.TryGetValue(key, out var list)) {
        list.Add(value);
    } else {
        clientCreatedKeyMap.Add(key, new List<ApiKey> { value });
    }
}

async Task<WriteBatch> SubmitBatchIfFull(WriteBatch batch) {
    if (items++ % quota == 0) {
        Console.WriteLine($"    Committing batch {items / quota}");
        await batch.CommitAsync();

        return client.StartBatch();
    }

    return batch;
}

namespace models {
    [FirestoreData]
    public class ApiKey {
        [FirestoreData(ConverterType = typeof(FirestoreEnumNameConverter<ApplicationStatus>))]
        public enum ApplicationStatus {
            Development,
            Production,
            None
        }

        [FirestoreData(ConverterType = typeof(FirestoreEnumNameConverter<ApplicationType>))]
        public enum ApplicationType {
            None,
            Browser,
            Server
        }

        [FirestoreData(ConverterType = typeof(FirestoreEnumNameConverter<KeyStatus>))]
        public enum KeyStatus {
            None,
            Active,
            Disabled
        }

        public ApiKey(string apiKey) {
            Key = apiKey;
        }

        public ApiKey() { }

        public string Id { get; set; } = string.Empty;
        public string AccountId { get; set; }
        [FirestoreProperty("key")]
        public string Key { get; set; }
        public long CreatedAtTicks { get; set; }
        [FirestoreProperty("created_on")]
        public DateTime CreatedAt => new DateTime(CreatedAtTicks).ToUniversalTime();
        [FirestoreProperty("status")]
        public KeyStatus ApiKeyStatus { get; set; }
        [FirestoreProperty("type")]
        public ApplicationType Type { get; set; }
        [FirestoreProperty("mode")]
        public ApplicationStatus AppStatus { get; set; }
        [FirestoreProperty("pattern")]
        public string Pattern { get; set; }
        [FirestoreProperty("regular_expression")]
        public string RegexPattern { get; set; }
        [FirestoreProperty("machine_name")]
        public bool IsMachineName { get; set; }
        [FirestoreProperty("deleted")]
        public bool Deleted { get; set; }
        [FirestoreProperty("elevated")]
        public bool Elevated { get; set; }
        [FirestoreProperty("deleted_on")]
        public Timestamp? DeletedOn { get; set; }
        [FirestoreProperty("disabled_on")]
        public Timestamp? DisabledOn { get; set; }

        public override string ToString() => string.Format(@"## Key
* **Pattern**: {0}
* **Key**: {1}", Pattern, Key);
    }
    [FirestoreData]
    public class Account {
        public string Id { get; set; } = null!;
        [FirestoreProperty("email")]
        public string Email { get; set; } = string.Empty;
        [FirestoreProperty("company")]
        public string Company { get; set; } = string.Empty;
        [FirestoreProperty("elevated")]
        public bool Elevated { get; set; }
        [FirestoreProperty("account_confirmation")]
        public EmailConfirmation Confirmation { get; set; } = null!;
        public KeyQuota KeyQuota { get; set; } = null!;
        [FirestoreProperty("key_count")]
        public int KeysUsed => KeyQuota?.KeysUsed ?? 0;
        [FirestoreProperty("password")]
        public PasswordHashAndSalt Password { get; set; } = null!;
    }
    [FirestoreData]
    public class EmailConfirmation {
        private DateTime? confirmationDate;
        [FirestoreProperty("confirmed")]
        public bool Confirmed { get; set; }
        [FirestoreProperty("confirmed_on")]
        public DateTime? ConfirmationDate {
            get => confirmationDate > new DateTime(2000, 1, 1) ? confirmationDate.Value.ToUniversalTime() : null;
            set => confirmationDate = value;
        }
    }
    [FirestoreData]
    public class KeyQuota {
        [FirestoreProperty("keys_used")]
        public int KeysUsed { get; set; }
    }
    [FirestoreData]
    public class PasswordHashAndSalt {
        [FirestoreProperty("password_hash")]
        public string HashedPassword { get; set; } = string.Empty;
        [FirestoreProperty("salt")]
        public string Salt { get; set; } = string.Empty;
    }
}
