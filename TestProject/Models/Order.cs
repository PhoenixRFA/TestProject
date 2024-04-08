namespace TestProject.Models;

public class Order
{
    public Order(User user, ICollection<OrderItem>? items = null)
    {
        User = user;

        if (items is not null)
        {
            Items.AddRange(items);
        }
    }
    
    public int Id { get; set; }
    public List<OrderItem> Items { get; init; } = new();
    public User User { get; init; }
    public float TotalSum => Items.Sum(x => x.Product.Price * x.Quantity);
}