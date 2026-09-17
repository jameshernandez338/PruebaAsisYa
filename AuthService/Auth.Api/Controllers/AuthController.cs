using Auth.Application.Auth.Interfaces;
using Auth.Application.Auth.Request;
using Microsoft.AspNetCore.Mvc;

namespace Datapino.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Message });

            return Ok(result);
        }   
    }
}