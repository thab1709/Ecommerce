namespace Ecommerce.Application.Features.Orders;
public record DeletepromotionCommand(Guid id) :IRequest<bool>;
public class DeletepromotionHandler : IRequestHandler<DeletepromotionCommand, bool>
{
    private readonly IPromotionRepository _promoRepo;
    public DeletepromotionHandler(IPromotionRepository promotion)
    {
        
        _promoRepo = promotion;
    }
    public async Task<bool> Handle(DeletepromotionCommand request, CancellationToken cancellationToken)
    {
        var promo = await _promoRepo.GetByIdAsync(request.id, cancellationToken);
        if(promo == null) return false;
        await _promoRepo.DeleteAsync(promo, cancellationToken);
        await _promoRepo.SaveChangesAsync(cancellationToken);
        return  true;

    }

}