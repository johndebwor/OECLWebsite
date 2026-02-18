namespace OECLWebsite.Data.Entities;

public class SitePhoto : BaseAuditableEntity
{
    public int Id { get; set; }
    public string PhotoKey { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
