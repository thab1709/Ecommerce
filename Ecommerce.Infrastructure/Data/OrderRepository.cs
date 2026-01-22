

namespace Ecommerce.Infrastructure.Data;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public void UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
    }

    public void DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id && !o.Deleted, cancellationToken);
            
    }

    public async Task<IEnumerable<Order>> GetByCustomerName(
        string customerName,
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.Customer.Name == customerName && !o.Deleted)
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid
     customerId, CancellationToken cancellationToken)
    {
        return await _context.Orders
              .Where(o =>o.CustomerId ==customerId)
              .ToListAsync(cancellationToken);

    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    public IQueryable<Order> GetQueryable()
    {
        return _context.Orders.AsQueryable();

    }
    }
