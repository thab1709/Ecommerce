
namespace Ecommerce.Application.Features.Orders;

public record CreateOrderCommand(Guid CustomerId, decimal TotalAmount, Guid? PromotionId) : IRequest<Guid>;

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
    private readonly IPromotionRepository _promotionRepository;

    public CreateOrderHandler(IOrderRepository orderRepository, ICustomerRepository customerRepository, IPromotionRepository promotionRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _promotionRepository = promotionRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
{
    var customer = await _customerRepository
        .GetByIdAsync(request.CustomerId, cancellationToken);
    var finalamount = request.TotalAmount;
   
    if (request.PromotionId.HasValue)
        {
            var promo = await _promotionRepository.GetByIdAsync(request.PromotionId.Value, cancellationToken);
            if(promo ==  null || !promo.IsValid(request.TotalAmount))
             throw new DomainException("Mã giảm giá ko tồn tại hoặc ko đủ điều kiện sử dụng ");
             var discount = promo.CalculatorDicount(request.TotalAmount);

             finalamount = request.TotalAmount - discount;
             if(finalamount< 0) finalamount =0;
             promo.use();
             await _promotionRepository.UpdateAsync(promo , cancellationToken);
        }
      
    if (customer == null)
        throw new DomainException($"Không tìm thấy khách hàng {request.CustomerId}");

    var order = customer.CreateOrder(finalamount);
     
    await _orderRepository.AddAsync(order, cancellationToken);
    await _orderRepository.SaveChangesAsync(cancellationToken);

    return order.Id;
}
}