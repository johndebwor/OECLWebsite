namespace OECLWebsite.Data.Entities;

public class SystemSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public bool IsSecret { get; set; }
}
