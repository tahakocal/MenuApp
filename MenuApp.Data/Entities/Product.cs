using System.ComponentModel.DataAnnotations.Schema;
using MenuApp.Data.Entities.Base;
using MenuApp.Data.Entities.Identity;
using Newtonsoft.Json;

namespace MenuApp.Data.Entities;

public class Product : IEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
    public string? Images { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser User { get; set; }

    public int? BrandId { get; set; }
    public Brand Brand { get; set; }
    public int? CategoryId { get; set; }
    public Category Category { get; set; }

    [NotMapped]
    public List<string> ImagePaths
    {
        get => string.IsNullOrWhiteSpace(Images)
            ? new List<string>()
            : JsonConvert.DeserializeObject<List<string>>(Images);
        set => Images = value == null || !value.Any() ? null : JsonConvert.SerializeObject(value);
    }
}