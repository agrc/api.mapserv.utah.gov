using Google.Cloud.Firestore;

#nullable enable
namespace AGRC.api.Models;
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
    public string AccountId { get; set; } = string.Empty;
    [FirestoreProperty("key")]
    public string Key { get; set; } = string.Empty;
    public long CreatedAtTicks { get; set; }
    [FirestoreProperty("created_on")]
    public DateTime CreatedAt => new DateTime(CreatedAtTicks).ToUniversalTime();
    [FirestoreProperty("status")]
    public KeyStatus Enabled { get; set; }
    [FirestoreProperty("type")]
    public ApplicationType Type { get; set; }
    [FirestoreProperty("mode")]
    public ApplicationStatus Configuration { get; set; }
    [FirestoreProperty("pattern")]
    public string? Pattern { get; set; }
    [FirestoreProperty("regular_expression")]
    public string? RegexPattern { get; set; }
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
