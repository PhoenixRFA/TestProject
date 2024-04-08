using TestProject.Models;

namespace TestProject.Services;

public interface IUserService
{
    Task<UserViewModel?> GetUserInfo(string userId);
    Task<UserViewModel?> Login(string email, string password);
}

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IStorageService _storage;
    
    public UserService(ILogger<UserService> logger, IStorageService storage)
    {
        _logger = logger;
        _storage = storage;
    }

    public async Task<UserViewModel?> GetUserInfo(string userId)
    {
        User? user = await _storage.GetUserAsync(userId);
        if (user is null) return null;

        var res = new UserViewModel(user.Id, user.Name, user.Email, user.Phone);

        return res;
    }
    
    public async Task<UserViewModel?> Login(string email, string password)
    {
        
        User? user = await _storage.GetUserByEmailAsync(email);
        if (user is null)
        {
            _logger.LogInformation("Login attempt to non existed user {email}", email);
            return null;
        }

        if (user.Password != password)
        {
            _logger.LogWarning("Wrong password on login attempt; user {userId}", user.Id);
            return null;
        }

        var res = new UserViewModel(user.Id, user.Name, user.Email, user.Phone);
        _logger.LogInformation("Successful login; user {userId}", user.Id);
        
        return res;
    }
}