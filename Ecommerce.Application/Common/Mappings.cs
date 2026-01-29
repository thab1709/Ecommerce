
namespace Ecommerce.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        
        CreateMap<Order, OrderDto>()
            .ReverseMap(); 

     
    }
}