namespace OECLWebsite.Infrastructure.Services;

public interface IEmailService
{
    Task SendContactInquiryAsync(string contactName, string email, string? company,
        string? phone, string? serviceInterest, string message);

    Task SendTestEmailAsync(string toAddress);

    Task SendInquiryReplyAsync(string toAddress, string contactName, string subject, string replyBody);
}
