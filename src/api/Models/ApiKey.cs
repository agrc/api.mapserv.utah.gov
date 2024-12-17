using Google.Cloud.Firestore;

namespace ugrc.api.Models;
[FirestoreData]
public class ApiKey {
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

    public ApiKey(string apiKey) {
        Key = apiKey;
    }

    public ApiKey() { }

    public string Id { get; set; } = string.Empty;
    [FirestoreProperty("accountId")] public string AccountId { get; set; } = string.Empty;
    [FirestoreProperty("key")] public string Key { get => field.ToLowerInvariant(); set; } = string.Empty;
    public long CreatedAtTicks { get; set; }
    [FirestoreProperty("created")] public DateTime CreatedAt => new DateTime(CreatedAtTicks).ToUniversalTime();
    [FirestoreProperty("pattern")] public string? Pattern { get; set; }
    [FirestoreProperty("regularExpression")] public string? RegularExpression { get; set; }
    [FirestoreProperty("machineName")] public bool IsMachineName { get; set; }
    [FirestoreProperty("elevated")] public bool Elevated { get; set; }
    [FirestoreProperty("deletedOn")] public Timestamp? DeletedOn { get; set; }
    [FirestoreProperty("claimed")] public bool Claimed { get; set; }
    [FirestoreProperty("claimedOn")] public Timestamp? ClaimedOn { get; set; }
    [FirestoreProperty("notes")] public string Notes { get; set; } = string.Empty;
    [FirestoreProperty("flags")]
    public Dictionary<string, bool> Flags { get; set; } = new Dictionary<string, bool>() {
                { "deleted", false },
                { "disabled", false },
                { "server", false },
                { "production", false }
        };

    public override string ToString() => string.Format(@"{0} {1}", Key, Pattern);
}
