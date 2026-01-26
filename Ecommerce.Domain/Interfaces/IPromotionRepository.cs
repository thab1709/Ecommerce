namespace Ecommerce.Domain.Interfaces;
public interface IPromotionRepository
{
    Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Promotion promotion, CancellationToken cancellationToken);
    Task AddAsync(Promotion promotion, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task DeleteAsync(Promotion promotion ,CancellationToken cancellationToken);
    Task<List<Promotion>> GetAllAsync(CancellationToken cancellationToken);
    Task <Promotion?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}