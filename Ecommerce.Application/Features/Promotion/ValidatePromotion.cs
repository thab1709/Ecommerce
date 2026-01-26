namespace Ecommerce.Application.Features.Orders;
public record ValidatePromotion(string Code, decimal TotalAmount) : IRequest<ValidateResponse>;
public record ValidateResponse
(
    bool IsValid,
    decimal DiscountValue,
   decimal finalamount,
   string Message


);
public class ValidatePromotionHandler : IRequestHandler< ValidatePromotion, ValidateResponse>
{
    private readonly IPromotionRepository _promorepo;
    public ValidatePromotionHandler(IPromotionRepository promotion)
    {
        _promorepo = promotion;
    }
  public async Task<ValidateResponse> Handle(ValidatePromotion request, CancellationToken cancellationToken)
    {
        var promo = await _promorepo.GetByCodeAsync(request.Code , cancellationToken);
        if(promo == null)
    
            return new ValidateResponse(false, 0, request.TotalAmount, "Mã giảm ko tồn tại");
        if (!promo.IsValid(request.TotalAmount))
        {
            string reason = "Mã ko đủ điền kiện sử dụng";
            if(promo.ExpiryDate < DateTime.UtcNow) reason = "Mã đã hết hạn sử dụng";
            if(promo.UsedCount >= promo.UsageLimit) reason = "Mã đã hết lượt sử dụng";
            if(request.TotalAmount < promo.MinOrderAmount) reason = $"Đơn hàng phải tối thiểu trên  {promo.MinOrderAmount:O}đ ";
             return new ValidateResponse(false,0,request.TotalAmount, reason);

        }

       var discount = promo.CalculatorDicount(request.TotalAmount);
       return new ValidateResponse(
        true,discount,request.TotalAmount - discount, "Áp dụng mã thành công"
       );
        



    }



}