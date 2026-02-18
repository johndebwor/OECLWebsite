namespace OECLWebsite.Data.Entities;

public class ApprovalHistory
{
    public int Id { get; set; }
    public int WorkflowId { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    public string? ActionBy { get; set; }
    public ApprovalAction Action { get; set; }
    public string? Comments { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }

    // Navigation
    public ApprovalWorkflow Workflow { get; set; } = null!;
}
