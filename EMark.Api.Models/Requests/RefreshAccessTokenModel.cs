using System;

namespace EMark.Api.Models.Requests
{
    public class RefreshAccessTokenModel
    {
        public string AccessToken { get; set; }
        public Guid RefreshToken { get; set; }
    }
}