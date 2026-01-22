namespace Ecommerce.Infrastructure.Data;
public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    public CustomerRepository(AppDbContext context)
    
    {
        _context = context;


    }
    public async Task <Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id,cancellationToken);

    }
    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

}