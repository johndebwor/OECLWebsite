namespace OECLWebsite.Data.Entities;

public class Announcement : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Medium;
    public bool IsActive { get; set; } = true;
}
