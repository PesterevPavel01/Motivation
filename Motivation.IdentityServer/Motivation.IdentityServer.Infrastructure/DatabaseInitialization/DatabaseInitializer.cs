using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Motivation.IdentityServer.Domain.Base;

namespace Motivation.IdentityServer.Infrastructure.DatabaseInitialization
{
    /// <summary>
    /// Database Initializer
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Seeds one default users to database for demo purposes only
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            if (context.Users.Any())
            {
                return;
            }

            var roles = AppData.Roles.ToArray();

            foreach (var role in roles)
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                if (!context!.Roles.Any(r => r.Name == role))
                {
                    roleManager
                        .CreateAsync(new ApplicationRole { Name = role, NormalizedName = role.ToUpper() })
                        .GetAwaiter()
                        .GetResult();
                }
            }

            #region developer

            var technicalUser = new ApplicationUser
            {
                Email = "tech_user@yandex.com",
                NormalizedEmail = "TECH_USER@YANDEX.RU",
                UserName = "tech_user",
                FirstName = "Технический",
                LastName = "Пользователь",
                NormalizedUserName = "TECH_USER",
                PhoneNumber = "+79000000000",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ApplicationUserProfile = new ApplicationUserProfile
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = "SEED",
                    Permissions = new List<AppPermission>
                    {
                        new()
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = "SEED",
                            PolicyName = "Profiles:Roles:Get",
                            Description = "Access policy for view Roles in user Profiles"
                        }
                    }
                }
            };

            if (!context!.Users.Any(u => u.UserName == technicalUser.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(technicalUser, "tech9185!");
                technicalUser.PasswordHash = hashed;
                var userStore = scope.ServiceProvider.GetRequiredService<ApplicationUserStore>();
                var result = userStore
                    .CreateAsync(technicalUser)
                    .GetAwaiter()
                    .GetResult(); ;
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Cannot create account");
                }

                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                foreach (var role in roles)
                {
                    var roleAdded = userManager!
                        .AddToRoleAsync(technicalUser, role)
                        .GetAwaiter()
                        .GetResult();
                    if (roleAdded.Succeeded)
                    {
                        context.SaveChanges();
                    }
                }
            }

            #endregion

            context.SaveChanges();
        }
    }
}
