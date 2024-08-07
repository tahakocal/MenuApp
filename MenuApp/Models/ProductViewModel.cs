using MenuApp.Data.Entities;

namespace MenuApp.Models;

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public int BrandId { get; set; }
    public List<string>? ImagePaths { get; set; }
    public List<IFormFile>? UploadedImages { get; set; }
    public string? ImagePathToDelete { get; set; }
    public int CategoryId { get; set; }
    public List<Category>? Category { get; set; }
}

public class BrandViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int DisplayOrder { get; set; }
    public IFormFile? Logo { get; set; }
    public string? LogoPath { get; set; }
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Tiktok { get; set; }
    public string? SocialX { get; set; }

    public string? BrandUrl { get; set; }
    public bool IsActive { get; set; }

}

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int BrandId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

}

public class BrandIdModel
{
    public int BrandId { get; set; }
}