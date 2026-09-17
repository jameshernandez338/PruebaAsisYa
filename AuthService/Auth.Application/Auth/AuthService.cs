using Auth.Application.Auth.Interfaces;
using Auth.Application.Auth.Request;
using Auth.Application.Auth.Response;
using Auth.Domain.Auth.Interfaces;
using Auth.Domain.Exceptions;

namespace Auth.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthUserRepository _userRepository;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IAuthUserRepository userRepository,
            IUserCredentialRepository userCredentialRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _userCredentialRepository = userCredentialRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener usuario
                var user = await _userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken);
                if (user is null)
                    return LoginResult.Fail("Credenciales inválidas");

                // Validar que puede hacer login (regla de negocio en el dominio)
                user.ValidateCanLogin();

                // Verificar credenciales
                var credentials = await _userCredentialRepository.GetCredentialByUserIdAsync(user.Id, cancellationToken);
                if (credentials is null)
                    return LoginResult.Fail("Credenciales inválidas");

                if (!_passwordHasher.Verify(request.Password, credentials.PasswordHash))
                    return LoginResult.Fail("Credenciales inválidas");

                var accessToken = _jwtTokenGenerator.Generate(user);

                return LoginResult.Success(accessToken, user.FullName);
            }
            catch (DomainException ex)
            {
                return LoginResult.Fail(ex.Message);
            }
        }
    }
}