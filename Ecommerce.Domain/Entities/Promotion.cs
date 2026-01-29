namespace Ecommerce.Domain.Entities;
public enum PromotiontType {FixedAmount, Percentage}
public class Promotion
{
    public Guid Id {get; set;}
    public string Code {get; set;} = string.Empty;
    public decimal Discount{get;set;}
    public decimal MinOrderAmount{get; set;}
    public int UsageLimit{get; set;}
    public int UsedCount{get;set;}
    public DateTime ExpiryDate{get; set;}
    public PromotiontType Type{get; set;}
    public decimal? MaxDiscountAmount{get; set;}
    public decimal DiscountValue{get; set;}
    public bool IsValid(decimal orderAmount)
    {
        return ExpiryDate >= DateTime.UtcNow && UsedCount < UsageLimit && orderAmount >= MinOrderAmount;
      


    }
    public void use()
    {
        if (UsedCount >= UsageLimit ) throw new DomainException("Mã khuyến mã đã hết hạn");
         UsedCount++;

    }
    public Promotion( string code,PromotiontType type,decimal value,decimal? maxDiscount, decimal discount, decimal minorder, DateTime expiry, int limit)
    {
        Id = Guid.NewGuid();
        Code =code;
        Type = type;
        DiscountValue =value;
        MaxDiscountAmount =maxDiscount;
        Discount = discount;
        MinOrderAmount = minorder;
        ExpiryDate = expiry;
        UsageLimit = limit;
        UsedCount  = 0;
 
    }
 private Promotion()
    {
      
    } 

 public decimal CalculatorDicount(decimal orderAmount)
    {
        decimal discount = 0;
        if (Type == PromotiontType.Percentage)
        {
            discount = orderAmount * (DiscountValue/100m);
  if(MaxDiscountAmount.HasValue && discount > MaxDiscountAmount.Value)
        {
            discount = MaxDiscountAmount.Value;
        }
        }
            else
            {
                discount =DiscountValue;
            }
        
      
    return Math.Min(discount, orderAmount);
    }
 
    }
  
