using System;

namespace EMark.Api.Models.Responses
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public Guid? RefreshToken { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public string Error { get; set; }

        public AuthResponse()
        {
        }

        private AuthResponse(string accessToken, Guid? refreshToken, DateTime? expiresAtUtc, string error)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresAtUtc = expiresAtUtc;
            Error = error;
        }
        
        public static AuthResponse CreateFailure(string error)
        {
            return new AuthResponse(null, null, null, error);
        }
    }
}