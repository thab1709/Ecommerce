namespace Ecommerce.Application.Features.Orders;
public record CreatePromotionCommand(string Code, PromotiontType Type,decimal DiscountValue,decimal? MaxDiscountAmount,decimal Discount, decimal MinOrderAmount, DateTime ExpiryDate, int UsageLimit) : IRequest<Guid>;
public class CreatePromotionHandler : IRequestHandler<CreatePromotionCommand, Guid>
{
    private readonly IPromotionRepository _promoRepo;

    public CreatePromotionHandler(IPromotionRepository promoRepo) => _promoRepo = promoRepo;

    public async Task<Guid> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = new Promotion(
            request.Code, 
            request.Type,
            request.DiscountValue,
            request.MaxDiscountAmount,
            request.Discount, 
            request.MinOrderAmount, 
            request.ExpiryDate, 
            request.UsageLimit
          );

        await _promoRepo.AddAsync(promotion, cancellationToken);
        await _promoRepo.SaveChangesAsync(cancellationToken);
        return promotion.Id;
    }
}