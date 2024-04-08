using TestProject.Models;

namespace TestProject.Services;

public interface IProductService
{
    Task<List<ProductViewModel>> GetAllProducts();
}

public class ProductsService : IProductService
{
    private readonly IStorageService _storage;
    
    public ProductsService(IStorageService storage)
    {
        _storage = storage;
    }
    
    public async Task<List<ProductViewModel>> GetAllProducts()
    {
        List<Product> dbItems = await _storage.GetProductsAsync();

        List<ProductViewModel> res = new (dbItems.Count);
        foreach (Product item in dbItems)
        {
            res.Add(new ProductViewModel(item.Id, item.Name, item.Price));
        }

        return res;
    }
}