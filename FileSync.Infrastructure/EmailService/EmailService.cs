using FileSync.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileSync.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _baseUrl;

    public EmailService(ILogger<EmailService> logger, string baseUrl)
    {
        _logger = logger;
        _baseUrl = baseUrl;
    }

    public Task SendDropNotificationAsync(List<string> recipientEmails, string dropCode, string? message)
    {
        var downloadLink = $"{_baseUrl}/drop/{dropCode}";

        // Log the email content instead of sending an actual email
        _logger.LogInformation(
            """
            ============ EMAIL ============
            To: {RecipientEmail}
            Subject: Someone sent you files via FileSync
            
            Body:
            You have received files!
            
            {Message}
            
            Download your files here:
            {DownloadLink}
            
            This link expires in 3 days.
            ===============================
            """,
            
            string.Join(", ", recipientEmails),
            message ?? "(No message)",
            downloadLink
        );

        return Task.CompletedTask;
    }
}
