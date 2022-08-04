using System;

namespace Prefix.Id
{
    public abstract class PrefixId<T> : IEquatable<PrefixId<T>> where T : PrefixId<T>, new()
    {
        public const string Separator = "_";
        private Guid _decodedGuid;
        private string _prefix;
        private string? _formattedValue;

        public abstract string Prefix { get; }

        public static T Create()
        {
            return CreatePrefixId(Guid.NewGuid());
        }

        public static T Create(Guid idGuid) => CreatePrefixId(idGuid);

        public string Value => _formattedValue ??= $"{_prefix}{Separator}{Base32.Encode(_decodedGuid.ToByteArray())}";

        public static bool TryParse(string value, out T? id)
        {
            id = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var parts = value.Split(Separator);
            if (parts.Length != 2)
            {
                return false;
            }

            var encodedGuid = parts[1];

            try
            {
                var decodedGuid = new Guid(Base32.Decode(encodedGuid));
                id = CreatePrefixId(decodedGuid, value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static T Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (TryParse(value, out var id) is false)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return id!;
        }

        public Guid ToGuid() => _decodedGuid;

        public bool HasPrefix(string prefix) => _prefix.Equals(prefix);

        public bool Equals(PrefixId<T>? other) => ReferenceEquals(this, other) || GetHashCode() == other?.GetHashCode();

        public override bool Equals(object? obj) => obj is PrefixId<T> prefixId && Equals(prefixId);

        public override int GetHashCode() => HashCode.Combine(_prefix, _decodedGuid);

        public override string ToString() => Value;

        public static explicit operator PrefixId<T>(string value) => Parse(value);

        public static implicit operator string(PrefixId<T> id) => id is null ? throw new ArgumentNullException(nameof(id)) : id.Value;

        private static T CreatePrefixId(Guid idGuid, string? formattedValue = null)
        {
            var id = new T();

            if (string.IsNullOrWhiteSpace(id.Prefix))
            {
                throw new ArgumentException($"Prefix is is Null, empty, or whitespace. Provide a prefix in {nameof(T)}", nameof(id.Prefix));
            }

            id._prefix = id.Prefix;
            id._decodedGuid = idGuid;
            id._formattedValue = formattedValue;
            
            return id;
        }
    }
}
