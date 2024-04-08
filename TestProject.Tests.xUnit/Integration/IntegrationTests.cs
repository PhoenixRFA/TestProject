using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TestProject.Models;

namespace TestProject.Tests.xUnit.Integration;

public class IntegrationTests
    : IClassFixture<WebApplicationFactoryWithFakeServices<Program>>
{
    private readonly WebApplicationFactoryWithFakeServices<Program> _factory;

    public IntegrationTests(WebApplicationFactoryWithFakeServices<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("index.html")]
    [InlineData("js/app.js")]
    public async Task Get_Static_ReturnOk(string path)
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(path);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_Order_ReturnSuccess()
    {
        HttpClient client = _factory.CreateClient();
        
        User user = _factory.Users.First();
        client.DefaultRequestHeaders.Add("Authorize", user.Id);
        Product[] products = _factory.Products.Take(3).ToArray();
        var model = new[]
        {
            new NewOrderItem(products[0].Id, 1),
            new NewOrderItem(products[1].Id, 2),
            new NewOrderItem(products[2].Id, 3)
        };
        HttpResponseMessage response = await client.PostAsJsonAsync("/order", model);

        response.EnsureSuccessStatusCode();
    }
    [Fact]
    public async Task Post_Order_ReturnBadRequest_OnNoUser()
    {
        HttpClient client = _factory.CreateClient();
        
        Product[] products = _factory.Products.Take(3).ToArray();
        var model = new[]
        {
            new NewOrderItem(products[0].Id, 1),
            new NewOrderItem(products[1].Id, 2),
            new NewOrderItem(products[2].Id, 3)
        };
        HttpResponseMessage response = await client.PostAsJsonAsync("/order", model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_Products_ReturnAllProducts()
    {
        HttpClient client = _factory.CreateClient();
        
        HttpResponseMessage response = await client.GetAsync("/product");
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        ProductViewModel[]? model = JsonConvert.DeserializeObject<ProductViewModel[]>(json);

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        Assert.NotNull(model);
        foreach (ProductViewModel item in model)
        {
            Product? dbItem = _factory.Products.Find(x =>
                x.Id == item.Id && x.Name == item.Name && x.Price == item.Price);
            Assert.NotNull(dbItem);
        }
    }
}