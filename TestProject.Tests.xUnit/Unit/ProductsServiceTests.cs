using FluentAssertions;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Tests.xUnit;

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
    
    [Fact]
    public async Task GetAllProducts_Returns_AllProducts()
    {
        var products = new List<Product>
        {
            new Product(1, "Product1", 1000),
            new Product(2, "Product2", 2000)
        };
        var fakeStorage = new FakeStorageService(products);
        var svc = new ProductsService(fakeStorage);

        List<ProductViewModel> res = await svc.GetAllProducts();

        Assert.NotEmpty(res);
        foreach (ProductViewModel item in res)
        {
            Product? sourceItem = products.Find(x => x.Id == item.Id && x.Name == item.Name && x.Price == item.Price);
            Assert.NotNull(sourceItem);
        }
    }
    
    [Fact]
     public async Task GetAllProducts_Returns_AllProducts_FluentAssertion()
     {
         var products = new List<Product>
         {
             new Product(1, "Product1", 1000),
             new Product(2, "Product2", 2000)
         };
         var fakeStorage = new FakeStorageService(products);
         var svc = new ProductsService(fakeStorage);
 
         List<ProductViewModel> res = await svc.GetAllProducts();

         res.Should().NotBeEmpty();
         foreach (ProductViewModel item in res)
         {
             Product? sourceItem = products.Find(x => x.Id == item.Id && x.Name == item.Name && x.Price == item.Price);
             sourceItem.Should().NotBeNull();
         }
     }
}