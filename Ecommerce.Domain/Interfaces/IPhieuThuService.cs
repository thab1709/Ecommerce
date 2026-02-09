namespace Ecommerce.Domain.Interfaces;
public interface IPhieuThuService
{
    Task<byte[]> CreateAndExportAsync(PhieuThu entity);
    Task<byte[]> GetAndExportByIdAsync(Guid id);
    Task<byte[]> ExportMultipleToPdfAsync(List<PhieuThu> entities);

}