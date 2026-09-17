namespace Auth.Domain.Auth.ValueObjects
{
    public sealed record HashedPassword
    {
        public string Value { get; }

        private HashedPassword(string value)
        {
            Value = value;
        }

        public static HashedPassword Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exceptions.DomainException("El password hash es requerido");

            var trimmedValue = value.Trim();

            if (trimmedValue.Length != 60)
                throw new Exceptions.DomainException("Password hash inválido");

            if (!trimmedValue.StartsWith("$2"))
                throw new Exceptions.DomainException("Password hash inválido");

            return new HashedPassword(trimmedValue);
        }

        public static implicit operator string(HashedPassword password) => password.Value;

        public override string ToString() => "[PROTECTED]"; // Por seguridad, no exponemos el hash
    }
}
