using Dapper;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Npgsql;

namespace Ecommerce.Infrastructure.Data;
public class PhieuThuService : IPhieuThuService
{
    private readonly string _connectionString;
    public PhieuThuService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("PostgresConnection");
        DevExpress.Xpo.DB.PostgreSqlConnectionProvider.Register();

    }
    public async Task<byte[]> CreateAndExportAsync(PhieuThu entity)
{
    using var db = new NpgsqlConnection(_connectionString);
    await db.OpenAsync();
    using var trans = await db.BeginTransactionAsync();
    try
    {
      
        var sqlHeader = @"INSERT INTO ""PhieuThu"" (""Id"", ""OrderCode"", ""CreatedAt"", ""CustomerName"", ""CustomerId"", ""ClassName"", ""ParentName"", ""ParentPhone"", ""Status"", ""TotalAmount"", ""QrCodeLink"") 
                         VALUES (@Id, @OrderCode, @CreatedAt, @CustomerName, @CustomerId, @ClassName, @ParentName, @ParentPhone, @Status, @TotalAmount, @QrCodeLink);";
        await db.ExecuteAsync(sqlHeader, entity, trans);

      
        entity.Details.ForEach(d => {
            if (d.Id == Guid.Empty) d.Id = Guid.NewGuid();
            d.PhieuThuId = entity.Id; 
        });

        var sqlDetail = @"INSERT INTO ""PhieuThuDetail"" (""Id"", ""PhieuThuId"", ""Content"", ""UnitPrice"", ""TaxRate"", ""TaxAmount"", ""SubTotal"") 
                         VALUES (@Id, @PhieuThuId, @Content, @UnitPrice, @TaxRate, @TaxAmount, @SubTotal);";
        await db.ExecuteAsync(sqlDetail, entity.Details, trans);

        await trans.CommitAsync();
        
       
        return await ExportToPdf(entity);
    }
    catch
    {
        await trans.RollbackAsync();
        throw;
    }
}
    private static XtraReport BuildReport(IEnumerable<PhieuThu> data)
    {
        var report = XtraReport.FromFile("phieuthu.repx", true);
        report.DataSource = data.ToList();
        report.DataMember = null;
        report.FilterString = string.Empty;
        report.RequestParameters = false;
        return report;
    }

    private async Task<byte[]> ExportToPdf(PhieuThu entity)
    {
        using var report = BuildReport(new List<PhieuThu> { entity });
        using var ms = new MemoryStream();
        await report.ExportToPdfAsync(ms);
        return ms.ToArray();
    }
   public async Task<byte[]> GetAndExportByIdAsync(Guid id)
{
    using var db = new NpgsqlConnection(_connectionString);
    
    
    var sql = @"
        SELECT h.*, d.* FROM ""PhieuThu"" h
        LEFT JOIN ""PhieuThuDetail"" d ON h.""Id"" = d.""PhieuThuId""
        WHERE h.""Id"" = @id";

    var phieuThuDictionary = new Dictionary<Guid, PhieuThu>();

    var result = await db.QueryAsync<PhieuThu, PhieuThuDetail, PhieuThu>(
        sql,
        (header, detail) =>
        {
            if (!phieuThuDictionary.TryGetValue(header.Id, out var phieuThuEntry))
            {
                phieuThuEntry = header;
                phieuThuEntry.Details = new List<PhieuThuDetail>();
                phieuThuDictionary.Add(phieuThuEntry.Id, phieuThuEntry);
            }

            if (detail != null)
            {
                phieuThuEntry.Details.Add(detail);
            }
            return phieuThuEntry;
        },
        new { id },
        splitOn: "Id" 
    );

    var entity = result.FirstOrDefault();

    if (entity == null)
    {
        throw new Exception("Không tìm thấy dữ liệu phiếu thu với ID này.");
    }

 
    return await ExportToPdf(entity);
}
public async Task<byte[]> ExportMultipleToPdfAsync(List<PhieuThu> entities)
{
    if (entities == null || !entities.Any()) return Array.Empty<byte>();

    using var db = new NpgsqlConnection(_connectionString);
    await db.OpenAsync();
    using var trans = await db.BeginTransactionAsync();

    try
    {
        var sqlHeader = @"INSERT INTO ""PhieuThu"" (""Id"", ""OrderCode"", ""CreatedAt"", ""CustomerName"", ""CustomerId"", ""ClassName"", ""ParentName"", ""ParentPhone"", ""Status"", ""TotalAmount"", ""QrCodeLink"") 
                         VALUES (@Id, @OrderCode, @CreatedAt, @CustomerName, @CustomerId, @ClassName, @ParentName, @ParentPhone, @Status, @TotalAmount, @QrCodeLink);";

        var sqlDetail = @"INSERT INTO ""PhieuThuDetail"" (""Id"", ""PhieuThuId"", ""Content"", ""UnitPrice"", ""TaxRate"", ""TaxAmount"", ""SubTotal"") 
                         VALUES (@Id, @PhieuThuId, @Content, @UnitPrice, @TaxRate, @TaxAmount, @SubTotal);";

        foreach (var item in entities)
        {
            if (item.Id == Guid.Empty) item.Id = Guid.NewGuid();
            item.Details?.ForEach(d => {
                if (d.Id == Guid.Empty) d.Id = Guid.NewGuid();
                d.PhieuThuId = item.Id;
            });

         
            await db.ExecuteAsync(sqlHeader, item, trans);

            if (item.Details != null && item.Details.Any())
            {
                await db.ExecuteAsync(sqlDetail, item.Details, trans);
            }
        }

        await trans.CommitAsync();

        using var report = BuildReport(entities);
        using var ms = new MemoryStream();
        await report.ExportToPdfAsync(ms);
        return ms.ToArray();
    }
    catch
    {
        await trans.RollbackAsync();
        throw;
    }
}

}
