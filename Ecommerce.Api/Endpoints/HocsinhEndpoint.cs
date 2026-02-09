using Ecommerce.Domain.Interfaces;


namespace Ecommerce.Api.Endpoints;

public class HocSinhEndpoint : ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hocsinh")
                       .WithTags("PrintStudentList");

        
        group.MapGet("/export", async (string? maKhoi, string? maLop, IHocSinhService service) => 
        {
    
            var pdf = await service.ExportDanhSachHocSinhAsync(maKhoi, maLop);
            
            var fileName = $"DanhSach_{DateTime.Now:yyyyMMdd}.pdf";
            return Results.File(pdf, "application/pdf", fileName);
        });
        group.MapGet("/export-excel", async (string? maKhoi, string? maLop, IHocSinhService service) => {
    var file = await service.ExportExcelAsync(maKhoi, maLop);
    return Results.File(file, 
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
        $"DanhSach_{DateTime.Now:yyyyMMdd}.xlsx");
});
        group.MapPost("/import", async (IFormFile file, IHocSinhService service) =>
{
    if (file == null || file.Length == 0) 
        return Results.BadRequest("Sếp chưa chọn file Excel kìa!");

    using var stream = file.OpenReadStream();
    var result = await service.ImportExcelAsync(stream);
    
    return Results.Ok(new { 
        Message = $"Thành công sếp ơi! Đã xử lý {result} học sinh.",
        Time = DateTime.Now 
    });
}).DisableAntiforgery(); 
    }
}