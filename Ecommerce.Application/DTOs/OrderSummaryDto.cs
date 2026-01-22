namespace Ecommerce.Application.DTOs;
public class OrderSummaryDto
{
     public OrderSummaryDto() { }
   public long TotalOrder {get; set;}
   public decimal TotalRevenue {get; set;}
   public long CanceledOrders {get; set;}
   public long PendingOrders {get; set;}
    
    
}