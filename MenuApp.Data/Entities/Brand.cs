using MenuApp.Data.Entities.Base;
using MenuApp.Data.Entities.Identity;

namespace MenuApp.Data.Entities;

public class Brand : IEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Logo { get; set; }
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Tiktok { get; set; }
    public string? SocialX { get; set; }

    public string? BrandUrl { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser User { get; set; }
}