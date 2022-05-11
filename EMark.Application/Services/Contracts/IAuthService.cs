using System.Threading.Tasks;
using EMark.Api.Models.Enums;
using EMark.Api.Models.Requests;
using EMark.Api.Models.Responses;

namespace EMark.Application.Services.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponse> Register(UserRegisterModel model, RoleModel role);
        Task<AuthResponse> SignIn(UserSignInModel model);
        Task<AuthResponse> RefreshAccessToken(RefreshAccessTokenModel model);
        Task UpdateUser(UserUpdateModel model);
        Task<UserUpdateModel> GetUser();
        Task<UserUpdateModel> GetUserById(int userId);
        Task UpdateUserPassword(UpdatePasswordModel model);
        Task DeleteUser();
    }
}