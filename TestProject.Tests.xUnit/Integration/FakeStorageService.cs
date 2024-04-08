using TestProject.Models;
using TestProject.Services;

namespace TestProject.Tests.xUnit.Integration;

public class FakeStorageService : IStorageService
{
    private readonly List<Product> _products;
    private readonly List<User> _users;

    public FakeStorageService()
    {
        _products = GenerateProducts(10);
        _users = GenerateUsers(4);
    }
    
    public FakeStorageService(List<Product> products, List<User> users)
    {
        _products = products;
        _users = users;
    }

    public Task<List<Product>> GetProductsAsync() => Task.FromResult(_products);

    public Task<User?> GetUserAsync(string id) => Task.FromResult(_users.FirstOrDefault(x => x.Id == id));

    public Task<User?> GetUserByEmailAsync(string email) =>
        Task.FromResult(_users.FirstOrDefault(x => x.Email == email));

    public Task<bool> SaveOrderAsync(Order order)
    {
        Console.WriteLine("Order saved!");
        return Task.FromResult(true);
    }

    public static List<Product> GenerateProducts(int count)
    {
        var res = new List<Product>(count);
        var rand = new Random(count);
        
        for (int i = 1; i <= count; i++)
        {
            res.Add(new Product(i, "Product" + i, rand.Next(100, 5000)));
        }

        return res;
    }

    public static List<User> GenerateUsers(int count)
    {
        var res = new List<User>(count);
        var rand = new Random(count);
        
        for (int i = 1; i <= count; i++)
        {
            res.Add(new User(
                $"5ebd91e8-{rand.Next(0, 9999):0000}-4ac8-9d6d-878a86e7b2b7",
                "Username" + i,
                $"username{i}@email.com",
                "password",
                rand.NextInt64(70000000000, 79999999999).ToString()
            ));
        }

        return res;
    }
}