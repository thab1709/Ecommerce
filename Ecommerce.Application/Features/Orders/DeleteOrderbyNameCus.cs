namespace Ecommerce.Application.Features.Orders;

public record DeleteOrderByNameCommand(string Name) : IRequest<int>;

public class DeleteOrderByNameHandler 
    : IRequestHandler<DeleteOrderByNameCommand, int>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderByNameHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<int> Handle(
        DeleteOrderByNameCommand request,
        CancellationToken cancellationToken)
    {
      
        var orders = await _orderRepository
            .GetByCustomerName(request.Name, cancellationToken);

        var orderList = orders.ToList();

        if (!orderList.Any())
            return 0;

      
        foreach (var o in orderList)
        {
            o.SoftDelete();
        }

   
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return orderList.Count;
    }
}
