namespace Ecommerce.Application.Features.Orders;
public record CancelOrder(Guid Id) :IRequest<bool>;
public class CancelOrderHandler : IRequestHandler<CancelOrder, bool>
{
    private readonly IOrderRepository _orderrepository;
    public CancelOrderHandler(IOrderRepository orderRepository)
    {
        _orderrepository = orderRepository;

    }
    public async Task<bool> Handle(CancelOrder request, CancellationToken cancellationToken)
    {
        var order = await _orderrepository.GetByIdAsync(request.Id, cancellationToken);
        if (order ==  null) return false;

        try
        {
           order.CancelOrder();
           await _orderrepository.SaveChangesAsync(cancellationToken);
           return true;

        }
      catch (InvalidOperationException)
        {
            return false;
        }
    }


}
