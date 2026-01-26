using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Ecommerce.Application.Features.Orders;
public  record ExportOrder() : IRequest<byte[]>;
public class ExportOrderHandler : IRequestHandler<ExportOrder, byte[]>
{
    private readonly IOrderRepository _orderrepository;
    public ExportOrderHandler(IOrderRepository orderRepository)
    {
        _orderrepository = orderRepository;

    }
    public async Task<byte[]> Handle(ExportOrder request , CancellationToken cancellationToken)
    {
        var orders = await _orderrepository.GetQueryable()
                                            .Include(o => o.Customer)
                                            .AsNoTracking()
                                            .ToListAsync(cancellationToken);

      using var workbook =  new XLWorkbook();
      var worksheet = workbook.Worksheets.Add("Orders Report");

      worksheet.Cell(1,1).Value = "Mã khách hàng";
      worksheet.Cell(1,2).Value = "Tên Khách hàng";
      worksheet.Cell(1,3).Value = "Tổng Tiển";
      worksheet.Cell(1,4).Value = "Trạng Thái";
      worksheet.Cell(1,5).Value = "Ngày tạo";

      var headerRow = worksheet.Row(1);
      headerRow.Style.Font.Bold = true;
      headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

      for (int i=0; i< orders.Count; i++)
        {
            var row  = i+2;
            worksheet.Cell(row ,1).Value = orders[i].Id.ToString();
            worksheet.Cell(row, 2).Value = orders[i].Customer.Name;
            worksheet.Cell(row,3).Value = orders[i].TotalAmount;
            worksheet.Cell(row,4).Value = orders[i].Status.ToString();
            worksheet.Cell(row,5).Value = orders[i].CreatedAt; 

        }
      worksheet.Columns().AdjustToContents();
      using var stream = new MemoryStream();
      workbook.SaveAs(stream);
      return stream.ToArray();
    }
    

}