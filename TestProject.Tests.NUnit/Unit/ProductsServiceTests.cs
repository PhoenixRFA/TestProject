using TestProject.Models;
using TestProject.Services;

namespace TestProject.Tests.NUnit;

[TestFixture]
public class ProductsServiceTests
{
    private class FakeStorageService : IStorageService
    {
        private List<Product> _products;
        
        public FakeStorageService(List<Product> products)
        {
            _products = products;
        }
        
        public Task<List<Product>> GetProductsAsync()
        {
            return Task.FromResult(_products);
        }
        
        public Task<User?> GetUserAsync(string id) => throw new NotImplementedException();
        public Task<User?> GetUserByEmailAsync(string email) => throw new NotImplementedException();
        public Task<bool> SaveOrderAsync(Order order) => throw new NotImplementedException();
    }

    private List<Product> _products;
    private FakeStorageService _fakeStorage;
    
    [SetUp]
    public void Setup()
    {
        _products = new List<Product>
        {
            new Product(1, "Product1", 1000),
            new Product(2, "Product2", 2000)
        };
        _fakeStorage = new FakeStorageService(_products);
    }
    
    [Test]
    public async Task GetAllProducts_Returns_AllProducts()
    {
        var svc = new ProductsService(_fakeStorage);

        List<ProductViewModel> res = await svc.GetAllProducts();

        Assert.That(res, Is.Not.Empty);
        foreach (ProductViewModel item in res)
        {
            Product? sourceItem = _products.Find(x => x.Id == item.Id && x.Name == item.Name && x.Price == item.Price);
            Assert.That(sourceItem, Is.Not.Null);
        }
    }
}