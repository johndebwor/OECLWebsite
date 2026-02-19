using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OECLWebsite.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OECLWebsite.Infrastructure.Services;

public class SmtpEmailService(IServiceProvider serviceProvider, ILogger<SmtpEmailService> logger) : IEmailService
{
    public async Task SendContactInquiryAsync(string contactName, string email, string? company,
        string? phone, string? serviceInterest, string message)
    {
        var settings = await LoadEmailSettingsAsync();
        if (settings is null) return;

        var subject = $"New Inquiry from {contactName} — Oval Engineering";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = BuildInquiryHtml(contactName, email, company, phone, serviceInterest, message);

        await SendAsync(settings, subject, bodyBuilder);
    }

    public async Task SendTestEmailAsync(string toAddress)
    {
        var settings = await LoadEmailSettingsAsync();
        if (settings is null)
            throw new InvalidOperationException("SMTP is not configured. Please fill in the email settings.");

        var subject = "Test Email — Oval Engineering Website";
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = """
                <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                    <div style="background: #0A2463; padding: 20px; border-radius: 8px 8px 0 0;">
                        <h1 style="color: #F8FAFC; font-size: 24px; margin: 0;">OVAL ENGINEERING</h1>
                    </div>
                    <div style="background: #f9fafb; padding: 30px; border-radius: 0 0 8px 8px; border: 1px solid #E2E8F0;">
                        <h2 style="color: #0A2463;">Test Email</h2>
                        <p style="color: #475569;">This is a test email from your Oval Engineering website. Email sending is configured correctly.</p>
                        <hr style="border: none; border-top: 1px solid #E2E8F0; margin: 20px 0;" />
                        <p style="color: #94A3B8; font-size: 12px;">Oval Engineering Company Limited — Juba, South Sudan</p>
                    </div>
                </div>
                """
        };

        // Override To for test
        await SendAsync(settings, subject, bodyBuilder, overrideTo: toAddress);
    }

    private async Task<EmailSettings?> LoadEmailSettingsAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var rows = await context.SystemSettings
            .Where(s => s.Category == "Email")
            .AsNoTracking()
            .ToListAsync();

        var dict = rows.ToDictionary(r => r.Key, r => r.Value ?? string.Empty);

        var host = dict.GetValueOrDefault("Email:SmtpHost", string.Empty);
        if (string.IsNullOrWhiteSpace(host))
        {
            logger.LogWarning("SMTP host is not configured. Email sending is skipped.");
            return null;
        }

        return new EmailSettings
        {
            Host = host,
            Port = int.TryParse(dict.GetValueOrDefault("Email:SmtpPort", "587"), out var port) ? port : 587,
            Username = dict.GetValueOrDefault("Email:SmtpUsername", string.Empty),
            Password = dict.GetValueOrDefault("Email:SmtpPassword", string.Empty),
            FromAddress = dict.GetValueOrDefault("Email:FromAddress", "noreply@oecl-ss.com"),
            FromName = dict.GetValueOrDefault("Email:FromName", "Oval Engineering Website"),
            To = dict.GetValueOrDefault("Email:To", "info@oecl-ss.com"),
            Cc = dict.GetValueOrDefault("Email:Cc", string.Empty),
            EnableSsl = !string.Equals(dict.GetValueOrDefault("Email:EnableSsl", "true"), "false", StringComparison.OrdinalIgnoreCase),
        };
    }

    private async Task SendAsync(EmailSettings settings, string subject, BodyBuilder bodyBuilder, string? overrideTo = null)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(overrideTo ?? settings.To));

            if (overrideTo is null && !string.IsNullOrWhiteSpace(settings.Cc))
            {
                message.Cc.Add(MailboxAddress.Parse(settings.Cc));
            }

            message.Subject = subject;
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            var secureOption = settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
            await client.ConnectAsync(settings.Host, settings.Port, secureOption);

            if (!string.IsNullOrWhiteSpace(settings.Username))
            {
                await client.AuthenticateAsync(settings.Username, settings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("Email sent successfully: {Subject}", subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email: {Subject}", subject);
            throw;
        }
    }

    private static string BuildInquiryHtml(string contactName, string email, string? company,
        string? phone, string? serviceInterest, string message)
    {
        var companyRow = !string.IsNullOrEmpty(company)
            ? $"<tr><td style='padding:8px 12px;color:#64748B;width:160px;'>Company</td><td style='padding:8px 12px;color:#1E293B;'>{System.Net.WebUtility.HtmlEncode(company)}</td></tr>"
            : string.Empty;

        var phoneRow = !string.IsNullOrEmpty(phone)
            ? $"<tr><td style='padding:8px 12px;color:#64748B;'>Phone</td><td style='padding:8px 12px;color:#1E293B;'>{System.Net.WebUtility.HtmlEncode(phone)}</td></tr>"
            : string.Empty;

        var serviceRow = !string.IsNullOrEmpty(serviceInterest)
            ? $"<tr><td style='padding:8px 12px;color:#64748B;'>Service of Interest</td><td style='padding:8px 12px;color:#1E293B;'>{System.Net.WebUtility.HtmlEncode(serviceInterest)}</td></tr>"
            : string.Empty;

        return $"""
            <div style="font-family: Arial, sans-serif; max-width: 640px; margin: 0 auto; padding: 20px;">
                <div style="background: #0A2463; padding: 24px 28px; border-radius: 8px 8px 0 0;">
                    <h1 style="color: #F8FAFC; font-size: 22px; margin: 0; letter-spacing: 1px;">OVAL ENGINEERING</h1>
                    <p style="color: #94A3B8; margin: 6px 0 0; font-size: 13px;">New Website Inquiry</p>
                </div>
                <div style="background: #f9fafb; padding: 30px 28px; border-radius: 0 0 8px 8px; border: 1px solid #E2E8F0; border-top: none;">
                    <h2 style="color: #0A2463; font-size: 18px; margin: 0 0 20px;">Contact Details</h2>
                    <table style="width: 100%; border-collapse: collapse; background: #fff; border-radius: 6px; border: 1px solid #E2E8F0; overflow: hidden;">
                        <tr style="background: #F8FAFC;">
                            <td style="padding:8px 12px;color:#64748B;width:160px;">Name</td>
                            <td style="padding:8px 12px;color:#1E293B;font-weight:600;">{System.Net.WebUtility.HtmlEncode(contactName)}</td>
                        </tr>
                        <tr>
                            <td style="padding:8px 12px;color:#64748B;">Email</td>
                            <td style="padding:8px 12px;"><a href="mailto:{System.Net.WebUtility.HtmlEncode(email)}" style="color:#F97316;">{System.Net.WebUtility.HtmlEncode(email)}</a></td>
                        </tr>
                        {companyRow}
                        {phoneRow}
                        {serviceRow}
                    </table>

                    <h2 style="color: #0A2463; font-size: 18px; margin: 24px 0 12px;">Message</h2>
                    <div style="background: #fff; border: 1px solid #E2E8F0; border-radius: 6px; padding: 16px 20px; color: #334155; line-height: 1.7; white-space: pre-wrap;">
                        {System.Net.WebUtility.HtmlEncode(message)}
                    </div>

                    <hr style="border: none; border-top: 1px solid #E2E8F0; margin: 28px 0 16px;" />
                    <p style="color: #94A3B8; font-size: 12px; margin: 0;">
                        This inquiry was submitted via the Oval Engineering Company Limited website contact form.<br />
                        Oval Engineering Company Limited — Amarat, Juba, Republic of South Sudan
                    </p>
                </div>
            </div>
            """;
    }

    private sealed class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Cc { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}
