using Microsoft.EntityFrameworkCore;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Interfaces;
using System.Reflection;

namespace Catalog.NotificationService.InfrastructureInfrastructure.Base
{
    public abstract class DbContextBase : DbContext
    {
        protected DbContextBase(DbContextOptions options) : base(options)
        {
            //LastSaveChangesResult = new SaveChangesResult();
        }
        /*
        public SaveChangesResult LastSaveChangesResult { get; }

        // Упрощенные SaveChanges методы без try-catch
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            DbSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DbSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            DbSaveChanges();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DbSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
        }
        */
        private void DbSaveChanges()
        {
            var now = DateTime.UtcNow; // Всегда используем UTC!

            var createdEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
            foreach (var entry in createdEntries)
            {
                if (!(entry.Entity is IAuditable auditable))
                    continue;

                // Безопасное обновление через интерфейс
                if (auditable.CreatedAt == default)
                    auditable.CreatedAt = now;
                //auditable.UpdatedAt = default;
                //auditable.CreatedBy ??= DefaultUserName;
                //auditable.UpdatedBy ??= DefaultUserName;
            }

            var updatedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
            foreach (var entry in updatedEntries)
            {
                if (entry.Entity is IAuditable auditable)
                {
                    var timeSinceLastUpdate = (now - auditable.UpdatedAt).TotalSeconds;
                    if (timeSinceLastUpdate > 1.0)
                        auditable.UpdatedAt = now;
                    //auditable.UpdatedBy ??= DefaultUserName;
                }
            }
        }

        /// <summary>
        /// Configures entity configurations from assembly
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Упрощенный способ применения конфигураций
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}