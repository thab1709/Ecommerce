namespace Ecommerce.Application.DTOs;

public record OrderDto
{
    
    public OrderDto() { } 

    public Guid Id { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CustomerName { get; init; }
}