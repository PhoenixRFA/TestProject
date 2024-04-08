using System.Text;
using TestProject.Models;

namespace TestProject.Services;

public interface IOrderService
{
    Task<CreateOrderResult> CreateOrder(string userId, ICollection<NewOrderItem> items);
}

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IStorageService _storage;
    private readonly IEmailService _email;
    
    public OrderService(ILogger<OrderService> logger, IStorageService storage, IEmailService email)
    {
        _logger = logger;
        _storage = storage;
        _email = email;
    }
    
    public async Task<CreateOrderResult> CreateOrder(string userId, ICollection<NewOrderItem> items)
    {
        User? user = await _storage.GetUserAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", userId);
            return new CreateOrderResult(false, "User not found");
        }

        List<Product> products = await _storage.GetProductsAsync();

        var sb = new StringBuilder("Order content:");
        List<OrderItem> orderItems = new (items.Count);
        foreach (NewOrderItem item in items)
        {
            Product? product = products.Find(x => x.Id == item.ProductId);
            if (product is null)
            {
                _logger.LogWarning("Unknown product {productId}", item.ProductId);
                return new CreateOrderResult(false, "Unknown product: " + item.ProductId);
            }

            if (item.Quantity <= 0)
            {
                var ex = new OrderQuantityException(item.ProductId, item.Quantity);
                
                _logger.LogError(ex, "Wrong quantity");
            }
            
            orderItems.Add(new OrderItem(product, item.Quantity));
            sb.Append($"{product.Name} x {item.Quantity} = {product.Price * item.Quantity}");
        }
        
        Order order = new (user, orderItems);
        sb.Append($"Total: {order.TotalSum}");

        bool isOk = await _storage.SaveOrderAsync(order);

        if (isOk && !string.IsNullOrEmpty(user.Email))
        {
            string subject = $"Order #{order.Id} confirmed";
            string body = sb.ToString();
            
            await _email.SendAsync(user.Email, subject, body);
        }
        
        return new CreateOrderResult(isOk);
    }
    
    
}