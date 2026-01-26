namespace Ecommerce.Infrastructure.Data;
public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;
    public PromotionRepository(AppDbContext context) => _context = context;
    public async Task<Promotion?> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        return await _context.Promotions.FindAsync(new object[] {Id}, cancellationToken);

 

    }
    public async Task UpdateAsync(Promotion promotion, CancellationToken cancellationToken)
    {
        _context.Promotions.Update(promotion);
        await Task.CompletedTask;


    }
    public async Task AddAsync(Promotion promotion, CancellationToken cancellationToken)
    {
        await _context.Promotions.AddAsync(promotion, cancellationToken);



    }
   public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        
          await _context.SaveChangesAsync(cancellationToken);

    }
    public async Task DeleteAsync(Promotion promotion, CancellationToken cancellationToken)
    {
        _context.Promotions.Remove(promotion);
        await Task.CompletedTask;


    }
    public async Task<List<Promotion>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Promotions
            .AsNoTracking()
            .OrderByDescending(x => x.ExpiryDate)
            .ToListAsync(cancellationToken);
    }
   public async Task<Promotion?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await _context.Promotions
        .FirstOrDefaultAsync(x =>x.Code == code , cancellationToken);


    }
   
   
   }