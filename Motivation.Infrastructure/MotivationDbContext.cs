using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace Motivation.Infrastructure
{
    public class MotivationDbContext : DbContext
    {
        public MotivationDbContext(DbContextOptions<MotivationDbContext> options): base(options) 
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }

    public class ApplicationDbContextContextFactory : IDesignTimeDbContextFactory<MotivationDbContext>
    {
        public MotivationDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=localhost;Port=5432;Database=motivation;User ID=admin;Password=admin;TrustServerCertificate=False;";

            var optionsBuilder = new DbContextOptionsBuilder<MotivationDbContext>();
            optionsBuilder.UseNpgsql(
                    connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            return new MotivationDbContext(optionsBuilder.Options);
        }
    }
}