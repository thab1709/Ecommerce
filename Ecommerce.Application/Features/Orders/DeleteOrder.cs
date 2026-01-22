namespace Ecommerce.Application.Features.Orders;
public record DeleteOrderCommand(Guid Id) : IRequest<bool>;
public class DeleteHandler : IRequestHandler<DeleteOrderCommand , bool>
{
    private readonly IOrderRepository _orderrepository;
    public DeleteHandler(IOrderRepository orderRepository)
    {
        _orderrepository = orderRepository;

    }
    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderrepository.GetByIdAsync(request.Id,cancellationToken);
        if(order == null) return false;
        order.SoftDelete();

        await _orderrepository.SaveChangesAsync(cancellationToken);
        return true;


    }


}
 