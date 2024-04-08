using TestProject.Services;

namespace TestProject.Tests.xUnit.Integration;

public class FakeEmailService : IEmailService
{
    public Task SendAsync(string email, string subject, string body)
    {
        Console.WriteLine("Send email!");
        
        return Task.CompletedTask;
    }
}