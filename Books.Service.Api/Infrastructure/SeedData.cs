using System;
using Books.Service.Internal.Api.Infrastructure.IdentityModels;
using Microsoft.EntityFrameworkCore;

namespace Books.Service.Internal.Api.Infrastructure
{
	public class SeedData
	{
        public static void Seed(ModelBuilder builder)
        {
            builder.Entity<TblRole>().HasData(new List<TblRole> {
            new TblRole {
                Id=1,
                Roleid = "admin",
                Name = "Admin"
            },
            new TblRole {
                Id=2,
                Roleid = "user",
                Name = "User"
            }
        });
        }
    }
}

