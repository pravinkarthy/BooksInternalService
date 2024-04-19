using System;
namespace Books.Service.Internal.Api.Infrastructure
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken(string username);

    }
}

