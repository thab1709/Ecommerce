
namespace Ecommerce.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message ) : base(message) {}


}
public class OrderNotFoundExeption : DomainException
{
    
public OrderNotFoundExeption(int Id)
        :base($"Lỗi không tìm thấy  mã {Id} đơn hàng của bạn trên hệ thống "){}
        }
    


