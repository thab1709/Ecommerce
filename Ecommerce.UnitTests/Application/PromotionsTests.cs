using Ecommerce.Domain.Exceptions;

public class GetPromotionsTests
{
    private readonly Mock<IPromotionRepository> _promoRepoMock;
    private readonly GetPromotionHandler _handler; 

    public GetPromotionsTests()
    {
        _promoRepoMock = new Mock<IPromotionRepository>();
        _handler = new GetPromotionHandler(_promoRepoMock.Object);
    }

    [Fact]
public async Task Handle_ShouldReturnListOfPromotions()
{
  
    var fakePromos = new List<Promotion> 
    { 
       
        // 1.Code, 2.Type, 3.Value, 4.MaxDiscount, 5.MinOrder, 6.MinOrder(decimal?), 7.Expiry, 8.Limit
        new Promotion("MÃ1", PromotiontType.FixedAmount, 10, null, 0, 0, DateTime.Now.AddDays(1), 10),
        new Promotion("MÃ2", PromotiontType.Percentage, 20, 50, 0, 0, DateTime.Now.AddDays(1), 10)
    };

    _promoRepoMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fakePromos);

   
    var result = await _handler.Handle(new GetPromotion(), default);

 
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
}
[Theory]
[InlineData(200000, 20000)]  
[InlineData(1000000, 50000)] 
public void Promotion_PercentageLogic_ShouldWorkCorrectly(decimal orderAmount, decimal expectedDiscount)
{
  
    var promo = new Promotion(
        "GIAM10_MAX50", 
        PromotiontType.Percentage, 
        10, 
        50000, 
        100000,
        0, 
        DateTime.UtcNow.AddDays(1), 
        100);

    
    var actualDiscount = promo.CalculatorDicount(orderAmount);

    
    actualDiscount.Should().Be(expectedDiscount);
}
[Fact]
public void Use_WhenReachLimit_ShouldThrowException()
{
  
    var promo = new Promotion("HET_LUOT", PromotiontType.FixedAmount, 100, null, 0, 0, DateTime.Now.AddDays(1), 5);
    
    
    for(int i = 0; i < 5; i++) promo.use();

    
    Action action = () => promo.use();

  
    action.Should().Throw<DomainException>()
          .WithMessage("Mã khuyến mã đã hết hạn");
}
[Fact]
public void CalculateDiscount_WhenDiscountHigherThanOrderAmount_ShouldReturnOrderAmount()
{
   
    var promo = new Promotion(
        code: "GIAM100K",
        type: PromotiontType.FixedAmount,
        value: 100000,   
        maxDiscount: null,
        minorder: 0,
        discount:100000,
        expiry: DateTime.Now.AddDays(1),
        limit: 10
    );

    decimal orderAmount = 80000; 

   
    var actualDiscount = promo.CalculatorDicount(orderAmount);

    
    actualDiscount.Should().Be(80000);
}
[Fact]
public void IsValid_UsageLimit()
{
       var promo = new Promotion("HẾT_LƯỢT", PromotiontType.FixedAmount,5000000, null,0,0,DateTime.UtcNow.AddDays(1),5);
       for(int i = 0;i < 5;i++) promo.use();
       var IsValid = promo.IsValid(10000000);
       IsValid.Should().BeFalse("Mã đã dùng hết 5/5 lượt thì không được hợp lệ nữa");   

}
[Fact]
public void Isvalid_Expired()
    {
        var promo = new Promotion("HẾT_HẠN", PromotiontType.FixedAmount,5000000, null,0,0,DateTime.UtcNow.AddDays(-1),5);
       var isValid = promo.IsValid(100000);
       isValid.Should().BeFalse("Mã Đã hét hạn");

    }
    [Fact]
    public void IsValid_MinAmount()
    {
        var promo = new Promotion("VIP500", PromotiontType.FixedAmount, 100000, null, 1000000, 900000, DateTime.UtcNow.AddDays(1), 10);
        var isValid = promo.IsValid(790000);
        isValid.Should().BeFalse("Đơn hàng chưa đủ điều kiện");
    }
}
