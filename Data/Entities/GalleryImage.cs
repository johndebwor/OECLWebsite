namespace OECLWebsite.Data.Entities;

public class GalleryImage : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
