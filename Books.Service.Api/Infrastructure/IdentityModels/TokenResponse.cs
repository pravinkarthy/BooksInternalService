using System;
namespace Books.Service.Internal.Api.Infrastructure.IdentityModels
{
    public class TokenResponse
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

