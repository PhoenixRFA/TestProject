using Microsoft.Extensions.Logging;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Tests.NUnit;

public class UserServiceTests
{
    private class FakeStorageService : IStorageService
    {
        private List<User> _users;
        
        public FakeStorageService(List<User> users)
        {
            _users = users;
        }
        public Task<List<Product>> GetProductsAsync() => throw new NotImplementedException();
        public Task<User?> GetUserAsync(string id) => Task.FromResult(_users.Find(x => x.Id == id));
        public Task<User?> GetUserByEmailAsync(string email) => throw new NotImplementedException();
        public Task<bool> SaveOrderAsync(Order order) => throw new NotImplementedException();
    }
    private class FakeLogger : ILogger<UserService>
    {
        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
        
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState,
            Exception?, string> formatter) { }
    }
    
    private const string _userId = "1dd780d2-21f6-4a50-a259-8316c65c92a6";
    private List<User> _users;
    private FakeStorageService _fakeStorage;
    private FakeLogger _fakeLogger;
    
    [SetUp]
    public void Setup()
    {
        _users = new List<User> {
            new User(_userId, "user1", "user1@email.com", "123456"),
            new User("2db4ec0f-cf6a-4a55-90a8-c7186297c961", "user2", "user2@email.com", "123456")
        };
        _fakeStorage = new FakeStorageService(_users);
        _fakeLogger = new FakeLogger();
    }
    
    [Test]
    public async Task GetUserInfo_Returns_User_ById()
    {
        var svc = new UserService(_fakeLogger, _fakeStorage);

        UserViewModel? user = await svc.GetUserInfo(_userId);
        
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Id, Is.EqualTo(_userId));
    }
}