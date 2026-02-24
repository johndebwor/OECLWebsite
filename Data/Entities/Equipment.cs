namespace OECLWebsite.Data.Entities;

public class Equipment : BaseAuditableEntity
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? DetailedDescription { get; set; }
    public string? TechnicalSpecs { get; set; } // JSON format
    public string? CatalogReference { get; set; }
    public string? AvailabilityNote { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Draft;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    // Navigation
    public EquipmentCategory Category { get; set; } = null!;
}
