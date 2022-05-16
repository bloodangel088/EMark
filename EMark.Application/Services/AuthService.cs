using AutoMapper;
using EMark.Api.Models.Enums;
using EMark.Api.Models.Requests;
using EMark.Api.Models.Responses;
using EMark.Application.Exeptions;
using EMark.Application.Options;
using EMark.Application.Services.Contracts;
using EMark.DataAccess.Connection;
using EMark.DataAccess.Entities;
using Kirpichyov.FriendlyJwt;
using Kirpichyov.FriendlyJwt.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EMark.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string InvalidRefreshTokenMessage = "Refresh token is invalid";

        private readonly DatabaseContext _databaseContext;
        private readonly JwtOptions _jwtOptions;
        private readonly IJwtTokenVerifier _jwtTokenVerifier;
        private IMapper _mapper;
        private readonly IJwtTokenReader _jwtTokenReader;

        public AuthService(DatabaseContext databaseContext, IOptions<JwtOptions> jwtOption, IJwtTokenVerifier jwtTokenVerifier, IMapper mapper, IJwtTokenReader jwtTokenReader)
        {
            _databaseContext = databaseContext;
            _jwtOptions = jwtOption.Value;
            _jwtTokenVerifier = jwtTokenVerifier;
            _mapper = mapper;
            _jwtTokenReader = jwtTokenReader;
        }

        public async Task<AuthResponse> Register(UserRegisterModel model, RoleModel role)
        {
            bool isUserExists = await _databaseContext.Users.AnyAsync(user => user.Email == model.Email);
            if (isUserExists)
            {
                return AuthResponse.CreateFailure("Email is already registered.");
            }

            User newUser = role switch
            {
                RoleModel.Student => _mapper.Map<UserRegisterModel, Student>(model),
                RoleModel.Teacher => _mapper.Map<UserRegisterModel, Teacher>(model),
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };

            _databaseContext.Users.Add(newUser);
            await _databaseContext.SaveChangesAsync();

            return await GenerateAuthResponse(newUser);
        }

        public async Task<AuthResponse> SignIn(UserSignInModel model)
        {
            User existingUser = await _databaseContext.Users.SingleOrDefaultAsync(user => user.Email == model.Email);
            if (existingUser is null || !BCrypt.Net.BCrypt.Verify(model.Password, existingUser.PasswordHash))
            {
                return AuthResponse.CreateFailure("Invalid credentials");
            }

            return await GenerateAuthResponse(existingUser);
        }

        public async Task UpdateUser(UserUpdateModel model)
        {
            User user = await _databaseContext.Users.SingleAsync(user => user.Id == int.Parse(_jwtTokenReader.UserId));

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            _mapper.Map(model, user);
            _databaseContext.Users.Update(user);

            await _databaseContext.SaveChangesAsync();
 
        }

        public async Task<UserUpdateModel> GetUser()
        {
            User user = await _databaseContext.Users.SingleAsync(user => user.Id == int.Parse(_jwtTokenReader.UserId));
            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            return _mapper.Map<User, UserUpdateModel>(user);
        }

        public async Task<UserUpdateModel> GetUserById(int userId)
        {
            User user = await _databaseContext.Users.SingleAsync(user => user.Id == userId);

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            return _mapper.Map<User, UserUpdateModel>(user);
        }

        public async Task UpdateUserPassword(UpdatePasswordModel model)
        {
            User user = await _databaseContext.Users.SingleAsync(user => user.Id == int.Parse(_jwtTokenReader.UserId));

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                throw new ValidationException("Invalid password");
            }

            if (BCrypt.Net.BCrypt.Verify(model.NewPassword, user.PasswordHash))
            {
                throw new ValidationException("Password should be different");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteUser()
        {
            var userId = int.Parse(_jwtTokenReader.UserId);
            User user = await _databaseContext.Users.SingleAsync(user => user.Id == userId);

            var isHaveGroup = await _databaseContext.Groups.AnyAsync(group => 
                group.TeacherGroups.Any(teacherGroup => teacherGroup.TeacherId == userId) || 
                group.StudentGroups.Any(studentGroup => studentGroup.StudentId == userId));

            if (isHaveGroup)
            {
                throw new ValidationException("Cant delete user with groups");
            }

            _databaseContext.Users.Remove(user);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<AuthResponse> RefreshAccessToken(RefreshAccessTokenModel model)
        {
            RefreshToken refreshToken = _databaseContext.RefreshTokens
                .AsNoTracking()
                .SingleOrDefault(token => token.Id == model.RefreshToken);

            if (refreshToken is null || refreshToken.IsInvalidated)
            {
                return AuthResponse.CreateFailure(InvalidRefreshTokenMessage);
            }

            if (refreshToken.ExpirationDate <= DateTime.UtcNow)
            {
                return AuthResponse.CreateFailure("Refresh token is expired.");
            }

            var verificationResult = _jwtTokenVerifier.Verify(model.AccessToken);
            if (!verificationResult.IsValid)
            {
                return AuthResponse.CreateFailure("Access token is invalid.");
            }

            User user = _databaseContext.Users
                .AsNoTracking()
                .Single(user => user.Id == int.Parse(verificationResult.UserId));

            return await GenerateAuthResponse(user);
        }

        private GeneratedTokenInfo GenerateAccessToken(User user)
        {
            TimeSpan lifeTime = TimeSpan.FromMinutes(_jwtOptions.AccessTokenTTLMinutes);

            return new JwtTokenBuilder(lifeTime, _jwtOptions.Secret)
                .WithAudience(_jwtOptions.Audience)
                .WithIssuer(_jwtOptions.Issuer)
                .WithUserEmailPayloadData(user.Email)
                .WithUserIdPayloadData(user.Id.ToString())
                .WithUserRolePayloadData(user.Role.ToString())
                .Build();
        }

        private async Task<Guid> GenerateRefreshToken(Guid jwtId, int userId)
        {
            var token = new RefreshToken()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMonths(_jwtOptions.RefreshTokenTTLMonth),
                IsInvalidated = false,
                JwtId = jwtId,
                UserId = userId
            };

            _databaseContext.RefreshTokens.Add(token);
            await _databaseContext.SaveChangesAsync();

            return token.Id;
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var accessTokenResult = GenerateAccessToken(user);

            return new AuthResponse()
            {
                AccessToken = accessTokenResult.Token,
                RefreshToken = await GenerateRefreshToken(Guid.Parse(accessTokenResult.TokenId), user.Id),
                ExpiresAtUtc = accessTokenResult.ExpiresOn
            };
        }
    }
}