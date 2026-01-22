
namespace Ecommerce.Application.Features.Orders;
public record UpdateStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<bool>;
public class UpdateStatusHandler : IRequestHandler<UpdateStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    public UpdateStatusHandler (IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;

    }
    public async Task<bool> Handle(UpdateStatusCommand request , CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId,cancellationToken);
        if(order == null)
        throw new DomainException($"Không tìm thấy đơn hàng{request.OrderId}");
        order.UpdateStatus(request.NewStatus);
        await _orderRepository.SaveChangesAsync(cancellationToken);
        return true;

    }


}