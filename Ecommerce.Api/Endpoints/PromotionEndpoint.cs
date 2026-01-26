using DocumentFormat.OpenXml.Spreadsheet;

namespace Ecommerce.Api.Endpoints;

public class PromotionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
      
        var group = app.MapGroup("/api/promotions").WithTags("Promotions");

       
        group.MapPost("/createpromotion", async ([FromBody] CreatePromotionCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/promotions/{result}", result);
        });

       
        group.MapGet("/Getpromotion", async (ISender sender) =>
        {
           var result = await sender.Send(new GetPromotion());
            return Results.Ok(result);
        });
        
        group.MapDelete("/Deletepromotion{id:guid}", async (Guid id,ISender sender) =>
        {
           var result = await sender.Send(new DeletepromotionCommand(id)) ;
           return result ? Results.NoContent() : Results.NotFound("không tìm thấy để xóa ");
        });
        group.MapPost("/validate-promotion", async ([FromBody] ValidatePromotion query ,ISender sender) =>
        {
            var result = await sender.Send(query);
            return Results.Ok(result);
        });
    }
}