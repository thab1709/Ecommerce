namespace Ecommerce.Domain.Interfaces;
public interface IHocSinhService
{
    Task<List<HocSinh>> GetDanhSachHocSinhAsync(string? maKhoi =null, string? maLop =null);
    Task<Byte[]> ExportDanhSachHocSinhAsync(string? maKhoi = null, string? maLop =null);
    Task<Byte[]> ExportExcelAsync(string? maKhoi = null, string? maLop =null);
    Task<int> ImportExcelAsync(Stream fileStream);

}