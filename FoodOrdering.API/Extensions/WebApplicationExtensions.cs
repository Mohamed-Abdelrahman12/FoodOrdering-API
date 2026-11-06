using FoodOrdering.Infrastructure.seeding;
namespace FoodOrdering.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var seeder = services.GetRequiredService<IDatabaseSeeder>();
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Seeding failed: {Message}", ex.Message);
                throw;
            }
        }
    }


    //public  static  class DatabaseSeeder
    //{
    //    public static async Task SeedDatabaseAsync(
    //       this WebApplication app)
    //    {
    //        using var scope = app.Services.CreateScope();

    //        var seeder =
    //            scope.ServiceProvider
    //            .GetRequiredService<IDatabaseSeeder>();

    //        await seeder.SeedAsync();
    //    }
    //    //public static async Task SeedAsync(IServiceProvider serviceProvider)
    //    //{
    //    //    using var scope = serviceProvider.CreateScope();
    //    //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //    //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    //    //    await SeedRolesAsync(roleManager);
    //    //    await SeedAdminUserAsync(userManager);
    //    //}

    //    //private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    //    //{
    //    //    string[] roles = { "Admin", "Customer" };
    //    //    foreach (var role in roles)
    //    //    {
    //    //        if (!await roleManager.RoleExistsAsync(role))
    //    //            await roleManager.CreateAsync(new IdentityRole(role));
    //    //    }
    //    //}

    //    //private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    //    //{
    //    //    const string adminEmail = "admin@foodorder.com";
    //    //    var admin = await userManager.FindByEmailAsync(adminEmail);
    //    //    if (admin != null) return;

    //    //    admin = new ApplicationUser
    //    //    {
    //    //        FirstName = "System",
    //    //        LastName = "Admin",
    //    //        Email = adminEmail,
    //    //        UserName = adminEmail,
    //    //        EmailConfirmed = true,
    //    //        PreferredLanguage = "en",
    //    //        IsActive = true
    //    //    };

    //    //    var result = await userManager.CreateAsync(admin, "Admin@12345");
    //    //    if (result.Succeeded)
    //    //        await userManager.AddToRoleAsync(admin, "Admin");
    //    //}
    //}

}
