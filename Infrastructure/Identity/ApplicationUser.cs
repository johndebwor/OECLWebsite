using Microsoft.AspNetCore.Identity;

namespace OECLWebsite.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
}
