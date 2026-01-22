
namespace Ecommerce.Application.Features.Orders;

public record CreateOrderCommand(Guid CustomerId, decimal TotalAmount) : IRequest<Guid>;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Mã khách hàng không được để trống");
        RuleFor(x => x.TotalAmount).GreaterThan(0).WithMessage("Tổng giá trị Order phải lớn hơn 0");
    }
}

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateOrderHandler(IOrderRepository orderRepository, ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
{
    var customer = await _customerRepository
        .GetByIdAsync(request.CustomerId, cancellationToken);

    if (customer == null)
        throw new DomainException($"Không tìm thấy khách hàng {request.CustomerId}");

    var order = customer.CreateOrder(request.TotalAmount);

    await _orderRepository.AddAsync(order, cancellationToken);
    await _orderRepository.SaveChangesAsync(cancellationToken);

    return order.Id;
}
}