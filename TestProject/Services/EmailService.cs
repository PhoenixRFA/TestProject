namespace TestProject.Services;

public interface IEmailService
{
    Task SendAsync(string email, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    
    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }
    
    public async Task SendAsync(string email, string subject, string body)
    {
        await Task.Delay(700);
        
        _logger.LogInformation("Send email to {email} with subject {subject}", email, subject);
    }
}