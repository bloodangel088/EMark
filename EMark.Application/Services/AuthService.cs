using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EMark.Api.Models.Enums;
using EMark.Api.Models.Requests;
using EMark.Api.Models.Responses;
using EMark.Application.Options;
using EMark.Application.Services.Contracts;
using EMark.DataAccess.Connection;
using EMark.DataAccess.Entities;
using EMark.DataAccess.Entities.Enums;
using Kirpichyov.FriendlyJwt;
using Kirpichyov.FriendlyJwt.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EMark.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string InvalidRefreshTokenMessage = "Refresh token is invalid";
        
        private readonly DatabaseContext _databaseContext;
        private readonly JwtOptions _jwtOptions;
        private readonly IJwtTokenVerifier _jwtTokenVerifier;
        private IMapper _mapper;

        public AuthService(DatabaseContext databaseContext, IOptions<JwtOptions> jwtOption, IJwtTokenVerifier jwtTokenVerifier, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _jwtOptions = jwtOption.Value;
            _jwtTokenVerifier = jwtTokenVerifier;
            _mapper = mapper;
        }

        public async Task<AuthResponse> Register(UserRegisterModel model, RoleModel role)
        {
            User existingUser = await _databaseContext.Users.SingleOrDefaultAsync(user => user.Email == model.Email);
            if (existingUser is not null)
            {
                return AuthResponse.CreateFailure("Email is already registered.");
            }

            User newUser = _mapper.Map<UserRegisterModel, User>(model);
            newUser.Role = _mapper.Map<RoleModel, Role>(role);

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

        public async Task<AuthResponse> RefreshAccessToken(RefreshAccessTokenModel model)
        {
            RefreshToken refreshToken = _databaseContext.RefreshTokens
                .AsNoTracking()
                .SingleOrDefault(token => token.Id == model.RefreshToken);

            if (refreshToken is null || refreshToken.IsInvalidated)
            {
                return AuthResponse.CreateFailure(InvalidRefreshTokenMessage);
            }

            if  (refreshToken.ExpirationDate <= DateTime.UtcNow)
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