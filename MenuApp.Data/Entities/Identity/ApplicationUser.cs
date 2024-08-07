using Microsoft.AspNetCore.Identity;

namespace MenuApp.Data.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}