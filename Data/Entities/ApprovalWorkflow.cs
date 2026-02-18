using OECLWebsite.Infrastructure.Identity;

namespace OECLWebsite.Data.Entities;

public class ApprovalWorkflow
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty; // 'Service', 'Project', 'ContentPage'
    public int EntityId { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public ApprovalStatus CurrentStatus { get; set; } = ApprovalStatus.PendingApproval;
    public ApproverLevel ApproverLevel { get; set; }
    public string? CurrentApprover { get; set; }
    public string? Comments { get; set; }
    public DateTime? ResolutionDate { get; set; }
    public string? ResolvedBy { get; set; }
    public string? ResolutionComments { get; set; }

    // Navigation
    public ApplicationUser Requester { get; set; } = null!;
    public ApplicationUser? Approver { get; set; }
    public ICollection<ApprovalHistory> History { get; set; } = [];
}
