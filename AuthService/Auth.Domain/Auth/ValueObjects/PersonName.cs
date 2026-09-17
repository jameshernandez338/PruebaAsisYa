using Auth.Domain.Exceptions;

namespace Auth.Domain.Auth.ValueObjects
{
    public sealed record PersonName
    {
        public string Value { get; }

        private PersonName(string value)
        {
            Value = value;
        }

        public static PersonName Create(string? value, string fieldName = "nombre")
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"El {fieldName} es requerido");

            var trimmedValue = value.Trim();

            if (trimmedValue.Length < 2)
                throw new DomainException($"El {fieldName} debe tener al menos 2 caracteres");

            if (trimmedValue.Length > 100)
                throw new DomainException($"El {fieldName} no puede exceder 100 caracteres");

            return new PersonName(trimmedValue);
        }

        public static implicit operator string(PersonName name) => name.Value;

        public override string ToString() => Value;
    }
}

