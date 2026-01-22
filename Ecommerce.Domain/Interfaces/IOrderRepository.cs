namespace Ecommerce.Domain.Interfaces;
public interface IOrderRepository
{
  Task<Order?> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
  Task<IEnumerable<Order>> GetByCustomerName(string CustomerName, CancellationToken cancellationToken);
  Task AddAsync(Order order, CancellationToken cancellationToken);
  void DeleteAsync(Order order);
  void UpdateAsync(Order order);
  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
  IQueryable<Order> GetQueryable();
}
