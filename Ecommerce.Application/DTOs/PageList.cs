namespace Ecommerce.Application.DTOs;
public class PageList<T>
{
    public List<T> Items {get; set;}
    public int PageNumber {get; set;}
    public int TotalPage {get;set;}
    public int TotalCount{get; set;}
    public bool HasPrevious => PageNumber >1;
    public bool HasNext => PageNumber < TotalPage;

    public PageList(List<T> items, int count,int pagenumber, int pagesize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pagenumber;
        TotalPage = (int)Math.Ceiling(count / (double)pagesize);

    }
    



}