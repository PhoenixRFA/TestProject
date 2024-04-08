using TestProject.Models;

namespace TestProject.Services;

public interface IStorageService
{
    Task<List<Product>> GetProductsAsync();
    Task<User?> GetUserAsync(string id);
    Task<User?> GetUserByEmailAsync(string email);
    //Task<List<Order>> GetUserOrders(string id);
    Task<bool> SaveOrderAsync(Order order);
}

public class StorageService : IStorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly Lazy<List<Product>> _products;
    private readonly Lazy<List<User>> _users;
    private readonly List<Order> _orders;
    
    public StorageService(ILogger<StorageService> logger)
    {
        _logger = logger;
        _orders = new List<Order>();
        _products = new Lazy<List<Product>>(() => new List<Product>(5)
        {
            new Product(1, "Product A", 1000),
            new Product(2, "Product B", 800),
            new Product(3, "Product C", 500),
            new Product(4, "Product D", 1200),
            new Product(5, "Product E", 1500)
        });
        _users = new Lazy<List<User>>(() => new List<User>(3)
        {
            new User("c24c27e1-afe9-46f6-b182-81f65d526395", "Bob", "bob@email.com", "qwerty"),
            new User("1dd780d2-21f6-4a50-a259-8316c65c92a6", "Jim", "jib@email.com", "123456", "79876543210"),
            new User("2db4ec0f-cf6a-4a55-90a8-c7186297c961", "Bill", "bill@email.com", "4As1$9qZ8")
        });
    }
    
    public async Task<List<Product>> GetProductsAsync()
    {
        await Task.Delay(500);

        List<Product> res = _products.Value;

        return res;
    }

    public async Task<User?> GetUserAsync(string id)
    {
        await Task.Delay(300);

        User? res = _users.Value.Find(x => x.Id == id);

        return res;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await Task.Delay(300);

        User? res = _users.Value.Find(x => x.Email == email);

        return res;
    }

    public async Task<bool> SaveOrderAsync(Order order)
    {
        await Task.Delay(1400);

        if (order.Id == 0)
        {
            int maxId = _orders.Count > 0 ? _orders.Max(x => x.Id) : 0;
            order.Id = maxId + 1;
        }
        
        _orders.Add(order);
        
        _logger.LogInformation("New order created: {OrderId}, with total sum: {TotalSum}", order.Id, order.TotalSum);

        return true;
    }
}