using System;
using System.Security.Cryptography;
using Books.Service.Internal.Api.Infrastructure;
using Books.Service.Internal.Api.Infrastructure.IdentityModels;

namespace customerapi.Model
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly ReadersDBContext _context;

        public RefreshTokenGenerator(ReadersDBContext context)
        {
            _context = context;

        }

        public string GenerateToken(string username)
        {
            var randomNumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomNumber);
                var refreshToken = Convert.ToBase64String(randomNumber);
                var user = _context.TblRefreshtokens.FirstOrDefault(o => o.UserId == username);

                if (user != null)
                {
                    user.RefreshToken = refreshToken;
                    _context.SaveChanges();

                }
                else
                {
                    TblRefreshtoken tblRefreshtoken = new()
                    {
                        UserId = username,
                        TokenId = new Random().Next().ToString(),
                        RefreshToken = refreshToken
                    };
                    _context.SaveChanges();
                }

                return refreshToken;
            }
        }
    }
}

