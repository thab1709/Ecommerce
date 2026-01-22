using System.Net;

namespace Ecommerce.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    private Order() { } 

    public Order(Guid customerId, decimal totalAmount)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Mã khách hàng không hợp lệ");

        if (totalAmount <= 0)
            throw new DomainException("Tổng tiền phải lớn hơn 0");

        Id = Guid.NewGuid();
        CustomerId = customerId;
        TotalAmount = totalAmount;
        Status = OrderStatus.Pending;
    }
    public void UpdateStatus(OrderStatus NewStatus)
    {
        if(this.Status == OrderStatus.Cancelled || this.Status == OrderStatus.Shipped) throw new DomainException("Đơn hàng đã kết thúc/Hủy ko thể cập nhật");
        this.Status = NewStatus;
    }
    public bool Deleted { get; private set;} =false;
    public void SoftDelete()
    {
        this.Deleted = true;

    }
public void CancelOrder()
    {
        if (Status == OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Không thể hủy đơn hàng đã được giao");
        }
        if (Status == OrderStatus.Cancelled) return;
        Status = OrderStatus.Cancelled;
        CreatedAt =DateTime.UtcNow;
    }
    public void Shipped()
{
   
    if (Status == OrderStatus.Cancelled)
    {
        throw new InvalidOperationException("Không thể giao đơn hàng đã bị hủy");
    }

    Status = OrderStatus.Shipped;
 
}
}
