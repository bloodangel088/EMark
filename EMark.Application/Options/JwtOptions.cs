﻿namespace EMark.Application.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenTTLMinutes { get; set; }
        public int RefreshTokenTTLMonth { get; set; }
        public string Secret { get; set; }
    }
}