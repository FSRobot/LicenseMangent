using LicenseManagement.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LicenseManagement.SQLite
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<RsaLicenseModel> RsaLicense => Set<RsaLicenseModel>();
        public DbSet<RsaSecretModel> RsaSecret => Set<RsaSecretModel>();
        public DbSet<OemModel> Oem => Set<OemModel>();
    }

    public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlite("Data Source=database.sqlite");
            return new DataContext(optionsBuilder.Options);
        }
    }
}
