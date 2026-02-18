using Microsoft.AspNetCore.Identity;

namespace OECLWebsite.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
    public int PermissionLevel { get; set; }
    public bool CanApprove { get; set; }
    public bool CanPublish { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
