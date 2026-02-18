namespace OECLWebsite.Data.Entities;

public class ServiceInquiry : BaseAuditableEntity
{
    public int Id { get; set; }
    public int? ServiceId { get; set; }
    public string? CompanyName { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Message { get; set; }
    public DateTime InquiryDate { get; set; } = DateTime.UtcNow;
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public string? ServiceInterest { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? ResponseDate { get; set; }

    // Navigation
    public Service? Service { get; set; }
}
