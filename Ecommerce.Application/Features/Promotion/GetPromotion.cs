namespace Ecommerce.Application.Features.Orders;
public record GetPromotion(): IRequest<List<Promotion>>;
public class GetPromotionHandler : IRequestHandler<GetPromotion, List<Promotion>>
{
    private readonly IPromotionRepository _promoRepo;
    public GetPromotionHandler (IPromotionRepository promotion)
    {
        _promoRepo = promotion;
    }
   public async Task<List<Promotion>> Handle(GetPromotion request, CancellationToken cancellationToken)
    {
        
return await _promoRepo.GetAllAsync(cancellationToken);

    }

}