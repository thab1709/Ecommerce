using Ecommerce.Infrastructure.Data;

namespace Ecommerce.Api.Endpoints;
public class OrderEnpoint : ICarterModule
{ 
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");
       group.MapPost("/", async ([FromBody] CreateOrderCommand command, ISender sender) =>
        {
          
            var result = await sender.Send(command);
            
            return Results.Created($"/api/orders/{result}", result);
        });
        group.MapGet("/{id:guid}", async (Guid id, ISender sender) => 
        {
            var result = await sender.Send(new GetOrderById(id));
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });
        group.MapGet("/customer/{customerId:guid}", async (Guid customerId, ISender sender) =>
        {
            var result = await sender.Send(new GetOrderByCustomerId(customerId));
            return Results.Ok(result);
        });
        group.MapGet("/search/", async ([FromQuery] string name , ISender sender) =>
        {
            var result = await sender.Send(new GetOrderByCustomerName(name));
            return Results.Ok(result);
        }
        );
        group.MapPatch("/{id:guid}/status", async (Guid id, [FromBody] OrderStatus newStatus, ISender sender)=>
        {
            var result = await sender.Send(new UpdateStatusCommand(id, newStatus));
            return result ? Results.NoContent() :Results.NotFound();
        }).RequireAuthorization();
        group.MapDelete("/{id:guid}", async(Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteOrderCommand(id));
            return result ? Results.NoContent() : Results.NotFound("Không tìn thấy đơn hàng");

        } ).RequireAuthorization();
        group.MapDelete("/by-name", async ([FromQuery] string name, ISender sender) =>
        {
            if (string.IsNullOrWhiteSpace(name))
           return Results.BadRequest("Tên khách hàng ko được để trống");
           var deleteCount = await sender.Send(new DeleteOrderByNameCommand(name));
           return deleteCount >0
           ? Results.Ok(new {Message = $"Đã xóa thành công  {deleteCount} đơn hàng của khách hàng '{name}'."})
           : Results.NotFound($"Không tìm thấy đơn hàng nào của khách {name} để xóa");
            
        });
       group.MapGet("/filter", async(
         ISender sender,
        [FromQuery] string? searchTerm,
        [FromQuery] OrderStatus? status,
        [FromQuery] int pagenumber = 1,
        [FromQuery] int pagesize = 10) =>
       {
           var query = new GetOrdersQuery(searchTerm, status, pagenumber, pagesize);
           var result = await sender.Send(query);
           return Results.Ok(result);
       }
       ) ;

       group.MapPost("/{id:guid}/cancelorder", async (Guid id, ISender sender) =>{
            var result = await sender.Send(new CancelOrder(id));
            return result
              ? Results.Ok("Đơn hàng đã được hủy")
              :Results.BadRequest("Không thể hủy đơn hàng vì đơn hàng không tồn tại hoặc đã giao");
        });
        group.MapGet("/dashboarorder", async (ISender sender) =>
        {
            var result = await sender.Send(new GetOrderDashboardQuery());
            return Results.Ok(result);

        });
        group.MapGet("/exportorder" ,  async (ISender sender) =>
        {
            var fileBytes = await sender.Send(new ExportOrder());
            var fileName = $"orders_Report_{DateTime.Now:yyyyyMMdd}.xlss";
            return Results.File(fileBytes,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",fileName);
          

        });
        group.MapGet("/logs", async (AppDbContext db) =>
        {
            return Results.Ok(await db.auditlogs.OrderByDescending(x =>x.TimeStamp).Take(50).ToListAsync());
        } ).WithTags("GetAuditlogs")
           .WithName("Admin");
    }
}