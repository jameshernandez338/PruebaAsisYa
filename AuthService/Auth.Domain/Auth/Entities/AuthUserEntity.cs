using Auth.Domain.Auth.ValueObjects;

namespace Auth.Domain.Auth.Entities
{
    public sealed class AuthUserEntity
    {
        public long Id { get; private set; }
        public string UserName { get; private set; }
        public Email Email { get; private set; }
        public PersonName FirstName { get; private set; }
        public PersonName LastName { get; private set; }
        public bool IsActive { get; private set; }


        private AuthUserEntity(
            long id,
            string userName,
            Email email,
            PersonName firstName,
            PersonName lastName,
            bool isActive)
        {
            Id = id;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsActive = isActive;
        }

        public static AuthUserEntity Reconstitute(
            long id,
            string userName,
            string email,
            string firstName,
            string lastName,
            bool isActive)
        {
            // Validar que los datos de la BD sean válidos
            var emailVO = Email.Create(email);
            var firstNameVO = PersonName.Create(firstName, "nombre");
            var lastNameVO = PersonName.Create(lastName, "apellido");

            return new AuthUserEntity(id, userName, emailVO, firstNameVO, lastNameVO, isActive);
        }

        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Valida si el usuario puede iniciar sesión
        /// </summary>
        public void ValidateCanLogin()
        {
            if (!IsActive)
                throw new Exceptions.DomainException("Usuario inactivo");
        }

        /// <summary>
        /// Desactiva el usuario
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Activa el usuario
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }
    }
}
