namespace Ecommerce.UnitTests;

public class OrderTest
{


    [Fact]
    public void CancelOrderwhenshipped_ShouldThrowException()
    {
        var order = new Order(Guid.NewGuid(), 100_000);
        order.Shipped();
        
        Action action = () => order.CancelOrder();

        action.Should().Throw<InvalidOperationException>()
              .WithMessage("Không thể hủy đơn hàng đã được giao"); 
    }

    [Fact]
    public void OrderCreated_ShouldHaveCorrectInitialState()
    {
        var order = new Order(Guid.NewGuid(), 200_000);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void OrderCanceled_ShouldPreventShipping()
    {
        var order = new Order(Guid.NewGuid(), 200_000);
        order.CancelOrder();
        
        Action action = () => order.Shipped();
        
        action.Should().Throw<InvalidOperationException>()
              .WithMessage("Không thể giao đơn hàng đã bị hủy");
    }

    [Fact]
    public void UpdateStatus_OnFinishedOrder_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid(), 300_000);
        order.Shipped();
        
        
        Action action = () => order.UpdateStatus(OrderStatus.Cancelled);
        action.Should().Throw<Exception>()
              .WithMessage("Đơn hàng đã kết thúc/Hủy ko thể cập nhật");
    }

    [Fact]
    public void GetOrder_CheckData_ShouldValidateCollection()
    {
        var orders = new List<Order>                                     
        {
            new Order(Guid.NewGuid(), 50_000_000),
            new Order(Guid.NewGuid(), 120_000_000),
            new Order(Guid.NewGuid(), 70_000_000)
        };

        orders.Should().HaveCount(3);
        orders.Should().OnlyHaveUniqueItems();
        orders.Should().ContainSingle(x => x.TotalAmount > 100_000_000);
    }


    [Fact]
    public void IsValid_Debug_CheckWhyItFails()
    {
       
        var promo = new Promotion(
            "Thang1", 
            PromotiontType.FixedAmount, 
            200_000,                  
            null,                      
            0,                        
            0,                       
            DateTime.Parse("2027-01-24T03:13:32Z"), 
            5                          
        ); 

   
        bool isNotExpired = promo.ExpiryDate >= DateTime.UtcNow;
        bool hasLimit = promo.UsedCount < promo.UsageLimit;
        bool minAmountMet = 500_000 >= promo.MinOrderAmount;

        isNotExpired.Should().BeTrue("Lỗi do hết hạn");
        hasLimit.Should().BeTrue("Lỗi do giới hạn sử dụng");
        minAmountMet.Should().BeTrue("Lỗi do chưa đủ tiền tối thiểu");
    }
}