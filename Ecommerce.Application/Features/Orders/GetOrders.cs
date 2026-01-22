using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Features.Orders;
public record GetOrderById(Guid Id ) : IRequest<Order?>;
public record GetOrderByCustomerId(Guid CustomerId) : IRequest<IEnumerable<Order>>;
public record GetOrderByCustomerName(string CustomerName) : IRequest<IEnumerable<Order>>;

public class GetOrderHandler : 
        IRequestHandler<GetOrderById, Order?>,
        IRequestHandler<GetOrderByCustomerId, IEnumerable<Order>>,
        IRequestHandler<GetOrderByCustomerName, IEnumerable<Order>>
{
    public readonly IOrderRepository _orderrepository;
    
    public GetOrderHandler(IOrderRepository orderRepository)
    {
        _orderrepository = orderRepository;

    }
    public async Task<Order?> Handle(GetOrderById request, CancellationToken cancellationToken)
    {
        return await _orderrepository.GetByIdAsync(request.Id,cancellationToken);
    }
    public async Task<IEnumerable<Order>> Handle(GetOrderByCustomerId request , CancellationToken cancellationToken)
    {
        return await _orderrepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

    } 
    public async Task<IEnumerable<Order>> Handle(GetOrderByCustomerName request, CancellationToken cancellationToken)
    {
        return await _orderrepository.GetByCustomerName(request.CustomerName,cancellationToken);
    }

}
