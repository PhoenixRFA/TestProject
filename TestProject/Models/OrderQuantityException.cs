namespace TestProject.Models;

public class OrderQuantityException : Exception
{
    public int ProductId { get; }
    public float Quantity { get; }

    public OrderQuantityException(int productId, float quantity) :
        base($"Wrong quantity passed: {quantity} on product: {productId}")
    {
        ProductId = productId;
        Quantity = quantity;
    }
}