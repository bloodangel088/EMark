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

        [HttpPut("update-user")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModel request)
        {
            await _authService.UpdateUser(request);
            return NoContent();
        }

        [HttpGet("user-info")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<UserUpdateModel> GetUser()
        {
            return await _authService.GetUser();
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("user-info-by-id")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<UserUpdateModel> GetUserById(int userId)
        {
            return await _authService.GetUserById(userId);
        }

        [HttpPut("update-user-password")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdatePasswordModel request)
        {
            await _authService.UpdateUserPassword(request);
            return NoContent();
        }

        [HttpDelete("delete-user")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser()
        {
            await _authService.DeleteUser();
            return NoContent();
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