namespace TestProject.Models;

public record OrderItem (Product Product, float Quantity);
public record NewOrderItem (int ProductId, float Quantity);