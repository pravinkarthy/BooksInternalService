using System;
using System.Configuration;
using Books.Service.Internal.Api.Infrastructure.IdentityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Books.Service.Internal.Api.Infrastructure
{
    public partial class ReadersDBContext : DbContext
    {
        public ReadersDBContext()
        {
        }

        public ReadersDBContext(DbContextOptions<ReadersDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblPermission> TblPermissions => Set<TblPermission>();
        public virtual DbSet<TblRefreshtoken> TblRefreshtokens  => Set<TblRefreshtoken>();
        public virtual DbSet<TblRole> TblRoles => Set<TblRole>();
        public virtual DbSet<TblUser> TblUsers => Set<TblUser>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedData.Seed(modelBuilder);
        }
    }
}

