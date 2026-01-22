namespace Ecommerce.Application.Features.Customers;
public record CreateCustomerCommand(string Name) : IRequest<Guid>;


public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    public readonly ICustomerRepository _customerRepository;

    public CreateCustomerHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
       
        var customer = new Customer(request.Name);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}