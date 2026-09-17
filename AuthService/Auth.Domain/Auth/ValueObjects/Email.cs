using Auth.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Auth.Domain.Auth.ValueObjects
{
    public sealed record Email
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("El email es requerido");

            var normalizedEmail = value.Trim().ToLowerInvariant();

            if (normalizedEmail.Length > 150)
                throw new DomainException("El email no puede exceder 150 caracteres");

            if (!EmailRegex.IsMatch(normalizedEmail))
                throw new DomainException("Formato de email inválido");

            return new Email(normalizedEmail);
        }

        public static implicit operator string(Email email) => email.Value;

        public override string ToString() => Value;
    }
}
