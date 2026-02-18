namespace OECLWebsite.Data.Entities;

public class ContentPage : BaseAuditableEntity
{
    public int Id { get; set; }
    public string PageKey { get; set; } = string.Empty; // 'about', 'vision', 'mission', etc.
    public string? PageTitle { get; set; }
    public string? Content { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Draft;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
}
