using Microsoft.AspNetCore.Identity;
using ThreeCats.Application.Constants;

namespace ThreeCats.Infrastructure.Identity;

public static class IdentitySeeder
{
    private static readonly string[] Roles =
    [
        UserRoles.Client,
        UserRoles.Doctor,
        UserRoles.Moderator,
        UserRoles.Administrator
    ];

    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var roleName in Roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(
                new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Не удалось создать роль {roleName}: {errors}");
            }
        }
    }
}
