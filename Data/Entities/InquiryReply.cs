namespace OECLWebsite.Data.Entities;

public class InquiryReply : BaseAuditableEntity
{
    public int Id { get; set; }
    public int ServiceInquiryId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string SentByUserId { get; set; } = string.Empty;
    public string SentByName { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool EmailSentSuccessfully { get; set; }
    public bool IsInternalNote { get; set; }

    // Navigation
    public ServiceInquiry ServiceInquiry { get; set; } = null!;
}
