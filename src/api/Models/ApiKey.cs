using Google.Cloud.Firestore;

namespace AGRC.api.Models;
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
    [FirestoreProperty("key")] public string Key { get; set; } = string.Empty;
    public long CreatedAtTicks { get; set; }
    [FirestoreProperty("created")] public DateTime CreatedAt => new DateTime(CreatedAtTicks).ToUniversalTime();
    [FirestoreProperty("status")] public KeyStatus Enabled { get; set; }
    [FirestoreProperty("type")] public ApplicationType Type { get; set; }
    [FirestoreProperty("mode")] public ApplicationStatus Configuration { get; set; }
    [FirestoreProperty("pattern")] public string? Pattern { get; set; }
    [FirestoreProperty("regularExpression")] public string? RegularExpression { get; set; }
    [FirestoreProperty("machineName")] public bool IsMachineName { get; set; }
    [FirestoreProperty("deleted")] public bool Deleted { get; set; }
    [FirestoreProperty("elevated")] public bool Elevated { get; set; }
    [FirestoreProperty("deletedOn")] public Timestamp? DeletedOn { get; set; }
    [FirestoreProperty("disabled")] public bool Disabled { get; set; }

    public override string ToString() => string.Format(@"## Key
* **Pattern**: {0}
* **Key**: {1}", Pattern, Key);
}
