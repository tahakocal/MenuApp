namespace MenuApp.Data.Entities.Base;

public class IEntity
{
    public int Id { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = true;
}