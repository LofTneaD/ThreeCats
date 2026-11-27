using Microsoft.AspNetCore.Identity;

namespace ThreeCats.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }
}
