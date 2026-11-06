using FoodOrdering.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FoodOrdering.Infrastructure.seeding
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DatabaseSeeder> _logger;

        // Admin credentials — move to appsettings/secrets in production
        private const string AdminEmail = "admin@foodorder.com";
        private const string AdminPassword = "Admin@12345";

        public DatabaseSeeder(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<DatabaseSeeder> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminAsync();
        }

        // ── Roles ────────────────────────────────────────────────────────────

        private async Task SeedRolesAsync()
        {
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogInformation("Role '{Role}' already exists — skipped.", role);
                    continue;
                }

                var result = await _roleManager.CreateAsync(new IdentityRole(role));

                if (result.Succeeded)
                    _logger.LogInformation("Role '{Role}' created.", role);
                else
                    _logger.LogWarning("Failed to create role '{Role}': {Errors}",
                        role, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Admin user ────────────────────────────────────────────────────────

        private async Task SeedAdminAsync()
        {
            // Guard 1 — check by email (the unique index on AspNetUsers)
            var existing = await _userManager.FindByEmailAsync(AdminEmail);
            if (existing != null)
            {
                _logger.LogInformation(
                    "Admin account '{Email}' already exists — skipped.", AdminEmail);

                // Guard 2 — make sure the existing account has the Admin role
                //           (covers the edge case where the role was added later)
                if (!await _userManager.IsInRoleAsync(existing, "Admin"))
                {
                    await _userManager.AddToRoleAsync(existing, "Admin");
                    _logger.LogInformation("Admin role assigned to existing account '{Email}'.", AdminEmail);
                }

                return;
            }

            // Create the admin account
            var admin = new ApplicationUser
            {
                FirstName = "System",
                LastName = "Admin",
                Email = AdminEmail,
                UserName = AdminEmail,          // UserName == Email prevents duplicate-username conflicts
                EmailConfirmed = true,
                PreferredLanguage = "en",
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(admin, AdminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create admin account: {Errors}", errors);
                throw new InvalidOperationException($"Admin seeding failed: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(admin, "Admin");

            if (!roleResult.Succeeded)
                _logger.LogWarning("Admin account created but role assignment failed: {Errors}",
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            else
                _logger.LogInformation(
                    "Admin account '{Email}' created and assigned 'Admin' role.", AdminEmail);
        }
    }
}