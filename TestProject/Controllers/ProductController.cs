using Microsoft.AspNetCore.Mvc;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get list of all products
    /// </summary>
    [HttpGet]
    public async Task<List<ProductViewModel>> GetAllProducts()
    {
        List<ProductViewModel> res = await _productService.GetAllProducts();

        return res;
    }
}