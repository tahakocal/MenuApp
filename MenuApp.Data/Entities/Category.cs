using MenuApp.Data.Entities.Base;

namespace MenuApp.Data.Entities;

public class Category : IEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? BrandId { get; set; }
    public Brand Brand { get; set; }
}