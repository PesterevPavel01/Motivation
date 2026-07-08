using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace Motivation.AnalyticsCore.Infrastructure
{
    public class AnalyticsCoreDbContext : DbContext
    {
        public AnalyticsCoreDbContext(DbContextOptions<AnalyticsCoreDbContext> options): base(options) 
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }

    public class ApplicationDbContextContextFactory : IDesignTimeDbContextFactory<AnalyticsCoreDbContext>
    {
        public AnalyticsCoreDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=localhost;Port=5432;Database=motivation_source;User ID=admin;Password=admin;TrustServerCertificate=False;";

            var optionsBuilder = new DbContextOptionsBuilder<AnalyticsCoreDbContext>();
            optionsBuilder.UseNpgsql(
                    connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            return new AnalyticsCoreDbContext(optionsBuilder.Options);
        }
    }
}