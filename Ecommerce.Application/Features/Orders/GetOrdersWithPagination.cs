using Ecommerce.Application.DTOs;

namespace Ecommerce.Application.Features.Orders;
public record GetOrdersQuery(
    string? SearchTerm,
    OrderStatus? Status,
    int PageNumber =1,
    int Pagesize = 10) : IRequest<PageList<OrderDto>>;
public class GetOrderQueryhandler : IRequestHandler<GetOrdersQuery, PageList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    public GetOrderQueryhandler (IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;

    }
    public async Task<PageList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _orderRepository.GetQueryable()
                                    .Include(o =>o.Customer)
                                    .AsNoTracking();
        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query =query.Where(o =>o.Customer.Name.Contains(request.SearchTerm));
        
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
                .Skip((request.PageNumber -1)* request.Pagesize)
                .Take(request.Pagesize)
                .ToListAsync(cancellationToken);
        var dtos = _mapper.Map<List<OrderDto>>(items);    
        return new PageList<OrderDto>(dtos, totalCount, request.PageNumber, request.Pagesize);                        

    }

}
