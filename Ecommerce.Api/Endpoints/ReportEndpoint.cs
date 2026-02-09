using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Ecommerce.Domain.Interfaces;

namespace Ecommerce.Api.Endpoints;
public class PhieuThuEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/phieuthu")
                        .WithTags("PrintReport");
        group.MapPost("/create-and-print", async ([FromBody] PhieuThu entity, IPhieuThuService service) =>
        {
            var pdf = await service.CreateAndExportAsync(entity);
            return Results.File(pdf, "application/pdf", $"PhieuThu_{entity.OrderCode}.pdf");
        });
        group.MapPost("/bullk-print", async ([FromBody] List<PhieuThu> entities,IPhieuThuService service ) =>
        {
            var pdf = await service.ExportMultipleToPdfAsync(entities);
            return Results.File(pdf, "application/pdf", "DanhSachNhieuPhieuThu.pdf");

        });
     group.MapGet("/print/{id:guid}", async (Guid id , IPhieuThuService service) =>{
              var pdf = await service.GetAndExportByIdAsync(id);
              return Results.File(pdf, "application/pdf", $"PhieuThu_{id}.pdf");
            
        });
   

    }
    
        

}