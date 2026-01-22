using Dapper;
using Ecommerce.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Application.Features.Orders;
public record GetOrderDashboardQuery(): IRequest<OrderSummaryDto>;
public class GetOrderDashboardHandler : IRequestHandler<GetOrderDashboardQuery, OrderSummaryDto>
{
    private readonly string _connectionString;
    public GetOrderDashboardHandler(IConfiguration configuration)
    {
        
        _connectionString = configuration.GetConnectionString("PostgresConnection");
    }
    public async Task<OrderSummaryDto> Handle(GetOrderDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"SELECT COUNT(*) AS TotalOrder,
        SUM(CASE WHEN ""Status"" !=2 THEN ""TotalAmount"" ELSE 0 END) AS TotalRevenue,
        COUNT(CASE WHEN ""Status"" =2 THEN 1 END) AS CanceledOrders,
        COUNT(CASE WHEN ""Status"" = 0 THEN 1 END) AS PendingOrders
        FROM ""Order""
        WHERE ""Deleted"" =false";
        return await connection.QuerySingleAsync<OrderSummaryDto>(sql);

    }

}