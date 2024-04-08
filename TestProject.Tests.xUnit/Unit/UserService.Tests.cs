using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Tests.xUnit;

public class UserService_Tests
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
            Exception?, string> formatter)
        {
        }
    }

    [Fact]
    public async Task GetUserInfo_Returns_User_ById()
    {
        string userId = "1dd780d2-21f6-4a50-a259-8316c65c92a6";
        var users = new List<User>
        {
            new User(userId, "user1", "user1@email.com", "123456"),
            new User("2db4ec0f-cf6a-4a55-90a8-c7186297c961", "user2", "user2@email.com", "123456")
        };
        var fakeStorage = new FakeStorageService(users);
        var fakeLogger = new FakeLogger();
        var svc = new UserService(fakeLogger, fakeStorage);

        UserViewModel? user = await svc.GetUserInfo(userId);

        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
    }

    [Fact]
    public async Task GetUserInfo_Returns_User_ById_FluentAssertion()
    {
        string userId = "1dd780d2-21f6-4a50-a259-8316c65c92a6";
        var users = new List<User>
        {
            new User(userId, "user1", "user1@email.com", "123456"),
            new User("2db4ec0f-cf6a-4a55-90a8-c7186297c961", "user2", "user2@email.com", "123456")
        };
        var fakeStorage = new FakeStorageService(users);
        var fakeLogger = new FakeLogger();
        var svc = new UserService(fakeLogger, fakeStorage);

        UserViewModel? user = await svc.GetUserInfo(userId);

        user.Should().NotBeNull();
        user.Id.Should().Be(userId);
    }

    [Fact]
    public async Task Login_AccessStorage_And_ReturnsUser()
    {
        var loggerMock = new Mock<ILogger<UserService>>();
        var storageMock = new Mock<IStorageService>();
        string userEmail = "user@email.com";
        string password = "password";
        var user = new User("1dd780d2-21f6-4a50-a259-8316c65c92a6", "user", userEmail, password);
        storageMock.Setup(x => x.GetUserByEmailAsync(userEmail)).ReturnsAsync(user);
        var svc = new UserService(loggerMock.Object, storageMock.Object);

        UserViewModel? res = await svc.Login(userEmail, password);
        
        storageMock.Verify(x => x.GetUserByEmailAsync(userEmail));
        Assert.NotNull(res);
        Assert.Equal(user.Id, res.Id);
    }

    [Fact]
    public async Task Login_ReturnNull_OnWrongPassword()
    {
        var loggerMock = new Mock<ILogger<UserService>>();
        var storageMock = new Mock<IStorageService>();
        string userEmail = "user@email.com";
        var user = new User("1dd780d2-21f6-4a50-a259-8316c65c92a6", "user", userEmail, "password");
        storageMock.Setup(x => x.GetUserByEmailAsync(userEmail)).ReturnsAsync(user);
        var svc = new UserService(loggerMock.Object, storageMock.Object);

        UserViewModel? res = await svc.Login(userEmail, "wrong_password");

        Assert.Null(res);
    }
}