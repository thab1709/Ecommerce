namespace Ecommerce.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private Customer() { } 

    public Customer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tên khách hàng không hợp lệ");

        Id = Guid.NewGuid();
        Name = name;
    }

    public Order CreateOrder(decimal totalAmount)
    {
        var order = new Order(Id, totalAmount);
        _orders.Add(order);
        return order;
    }
}
