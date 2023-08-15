using Google.Cloud.Firestore;

namespace AGRC.api.Models;
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
    public object ToFirestore(T value) => value.ToString();
}
