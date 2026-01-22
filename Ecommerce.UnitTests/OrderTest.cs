
namespace Ecommerce.UnitTests;
public class OrderTest
{
    [Fact]
    public void CancelOrderwhenshipped()
    {
        var order = new Order(Guid.NewGuid(), 100_000);
        order.Shipped();
        
        Action action = () => order.CancelOrder();

        action.Should().Throw<InvalidOperationException>()
              .WithMessage("Không thể hủy đơn hàng đã được giao"); 
    }
  [Fact]
    public void OrderCreated()
    {
         var order = new Order(Guid.NewGuid(), 200_000);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Id.Should().NotBeEmpty();

    }
   [Fact]
    public void OrderCanceled()
    {
        var order = new Order(Guid.NewGuid(), 200_000);
        order.CancelOrder();
        Action action = () => order.Shipped();
        
        action.Should().Throw<InvalidOperationException>()
        .WithMessage("Không thể giao đơn hàng đã bị hủy");
       
    }
}
