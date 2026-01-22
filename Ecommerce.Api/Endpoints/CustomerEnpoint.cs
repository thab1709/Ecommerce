namespace Ecommerce.Api.Endpoints;

public class CustomerEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers");

        group.MapPost("/", async ([FromBody] CreateCustomerCommand command, ISender sender) =>
        {
            var customerId = await sender.Send(command);
            return Results.Created($"/api/customers/{customerId}", new { Id = customerId });
        });
    }
}