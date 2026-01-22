namespace Ecommerce.Domain.Interfaces;
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task AddAsync(Customer customer,CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}