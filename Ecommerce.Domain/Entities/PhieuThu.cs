namespace Ecommerce.Domain.Entities;
public class PhieuThu
{
    public Guid Id {get; set;}
    public string OrderCode {get; set;}
    public DateTime CreatedAt {get; set;}
    public string CustomerName{get;set;}
    public string CustomerId{get; set;}
    public string ClassName {get; set;}
    public string ParentName {get; set;}
    public string ParentPhone {get; set;}
    public string Status {get; set;}
    public decimal TotalAmount {get;set;}
    public string QrcodeLink {get; set;}
    public List<PhieuThuDetail> Details {get; set;} = new();
}
public class PhieuThuDetail
    {
        public Guid Id { get; set; }
        public Guid PhieuThuId { get; set; }
        
       
        public string Content { get; set; } 
        public decimal UnitPrice { get; set; }
        public double TaxRate { get; set; } = 0.08; 
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; } 
    }