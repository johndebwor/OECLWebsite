namespace OECLWebsite.Data.Entities;

public class PageView
{
    public int Id { get; set; }
    public string? PageUrl { get; set; }
    public string? PageTitle { get; set; }
    public DateTime ViewDate { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? ReferrerUrl { get; set; }
}
