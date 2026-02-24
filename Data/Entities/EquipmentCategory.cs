namespace OECLWebsite.Data.Entities;

public class EquipmentCategory : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? IconName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Equipment> Equipment { get; set; } = [];
}
