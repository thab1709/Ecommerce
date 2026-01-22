namespace Ecommerce.Domain.Entities;
public class Auditlog
{
    public Guid Id{get; set;} = Guid.NewGuid();
    public string EntityName {get; set;}
    public string EntityId{get; set;}
    public string Action{get; set;}
    public string OldValue{get;set;}
    public string NewValue{get;set;}
    public string? ChangedBy{get; set;}
    public DateTime TimeStamp{get; set;} =DateTime.UtcNow;


}