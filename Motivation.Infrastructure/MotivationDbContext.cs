using Catalog.NotificationService.InfrastructureInfrastructure.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace Motivation.Infrastructure
{
    public class MotivationDbContext : DbContextBase
    {
        public MotivationDbContext(DbContextOptions<MotivationDbContext> options): base(options) 
        {
            //Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
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