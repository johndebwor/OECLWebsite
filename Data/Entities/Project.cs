namespace OECLWebsite.Data.Entities;

public class Project : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Client { get; set; }
    public string? Location { get; set; }
    public string? ProjectType { get; set; } // FEED, EPCC, O&M, etc.
    public DateOnly? StartDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public decimal? ProjectValue { get; set; }
    public string? Description { get; set; }
    public string? TechnicalDetails { get; set; } // JSON format
    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;
    public string? FeaturedImageUrl { get; set; }
    public bool IsShowcase { get; set; }
    public int DisplayOrder { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Draft;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    // Navigation
    public ICollection<ProjectImage> Images { get; set; } = [];
}
