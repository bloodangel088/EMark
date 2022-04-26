using System.Threading.Tasks;
using EMark.Api.Models.Enums;
using EMark.Api.Models.Requests;
using EMark.Api.Models.Responses;
using EMark.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMark.Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("{role}/register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel request, [FromRoute] RoleModel role)
        {
            return MapAuthResponse(await _authService.Register(request, role));
        }

        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] UserSignInModel request)
        {
            return MapAuthResponse(await _authService.SignIn(request));
        }  
        
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshAccessTokenModel request)
        {
            return MapAuthResponse(await _authService.RefreshAccessToken(request));
        }

        private IActionResult MapAuthResponse(AuthResponse authResponse) 
        {
            if (authResponse.Error is not null)
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }
    }
}