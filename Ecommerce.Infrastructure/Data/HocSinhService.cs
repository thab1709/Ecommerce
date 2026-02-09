using Dapper;
using DevExpress.Charts.Native;
using DevExpress.Export.Xl;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSpreadsheet.Import.Xls;
using Npgsql;

namespace Ecommerce.Infrastructure.Data;

public class HocSinhService : IHocSinhService
{
    private readonly string _connectionString;
    public HocSinhService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("PostgresConnection");
    }
    public async Task<List<HocSinh>> GetDanhSachHocSinhAsync(string? maKhoi, string? maLop)
    {
        using var db = new NpgsqlConnection(_connectionString);
        var sql = @"SELECT * FROM ""HocSinh""
        Where (@MaKhoi Is NULL OR ""MaKhoi"" = @MaKhoi) AND (@MaLop IS NULL OR ""MaLop"" = @MaLop)
        ORDER BY ""MaKhoi"" , ""MaLop"", ""HoVaTen""";
        var result = await db.QueryAsync<HocSinh>(sql, new {MaKhoi = maKhoi, MaLop = maLop});
        return result.ToList();

    }
    public async Task<byte[]> ExportDanhSachHocSinhAsync(string? maKhoi, string? maLop)
    {
        var data = await GetDanhSachHocSinhAsync(maKhoi, maLop);
    XtraReport report = XtraReport.FromFile("StudentList.repx", true);
    report.DataSource = data;

   
    string filterText = "Tất cả";
    if (!string.IsNullOrEmpty(maLop)) {
        filterText = $"Lớp {maLop}";
    }
    else if (!string.IsNullOrEmpty(maKhoi)) {
        filterText = maKhoi; 
    }

   
    report.Parameters["pFilterDescription"].Value = filterText;
    
  
    report.RequestParameters = false; 

    using var ms = new MemoryStream();
    await report.ExportToPdfAsync(ms);
    return ms.ToArray();
}
public async Task<byte[]> ExportExcelAsync(string? maKhoi, string? maLop)
    {
        var data = await GetDanhSachHocSinhAsync(maKhoi, maLop);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("DanhSachHocSinh");
        int totalColums =11;
        int row = 1;
        worksheet.Range(row, 1, row, 6).Merge().Value = "TRƯỜNG MẦM NON DỊCH VỌNG HẬU";
      worksheet.Row(row).Style.Font.Bold = true;
       worksheet.Row(row).Style.Font.FontSize = 14;
       worksheet.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
       row ++;
       worksheet.Range(row,1, row,6).Merge().Value = "DANH SÁCH HỌC SINH";
       worksheet.Row(row).Style.Font.Bold = true;
       worksheet.Row(row).Style.Font.FontSize = 13;
       worksheet.Row(row).Style.Font.FontColor = XLColor.FromHtml("#1B4E75");
       worksheet.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
       row ++;
       string filterText = "Tất cả";
if (!string.IsNullOrEmpty(maLop)) 
{
    filterText = $"Lớp {maLop}";
}
else if (!string.IsNullOrEmpty(maKhoi)) 
{
    filterText = maKhoi; 
}
       worksheet.Range(row, 1, row,6).Merge().Value = $"Điều kiện lọc:{filterText}";
       worksheet.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
       worksheet.Row(row).Style.Font.Italic = true;
       row +=2;

       string [] headers = {"STT", "Khối", "Mã Lớp", "Mã HS", "Họ và Tên", "Ngày sinh",
       "Giới tính", "Họ tên Phụ Huynh", "Số điện thoại", "Địa chỉ", "Trạng thái"};
         for(int i=0; i< headers.Length; i++)
         worksheet.Cell(row, i+1 ).Value = headers[i];
         var headerRange = worksheet.Range(row, 1, row, totalColums);
          headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0c6fba");
    headerRange.Style.Font.FontColor = XLColor.White;
    headerRange.Style.Font.Bold = true;
    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    row ++;

    int stt =1 ;
    var groupByLop = data.GroupBy(x =>x.MaLop);
    foreach(var lop in groupByLop)
        {
            int countLop = 0;
            foreach(var hs in lop)
            {
                worksheet.Cell(row, 1).Value = stt++;
                worksheet.Cell(row,2).Value = hs.MaKhoi;
                worksheet.Cell(row,3).Value =hs.MaLop;
                worksheet.Cell(row,4).Value =hs.MaHS;
                worksheet.Cell(row,5).Value =hs.HoVaTen;
                worksheet.Cell(row, 6).Value = hs.NgaySinh ?? "";
                worksheet.Cell(row,7).Value =hs.GioiTinh;
                worksheet.Cell(row,8).Value = hs.HoTenPhuHuynh;
                worksheet.Cell(row,9).Value =hs.SoDienThoai;
                worksheet.Cell(row,10).Value=hs.DiaChi;
                worksheet.Cell(row,11).Value = hs.TrangThai;

                var dataRange = worksheet.Range(row, 1,row,totalColums);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                row ++;
                countLop ++;
                
              
            }
            var totalLopRange = worksheet.Range(row, 1, row, totalColums);
    totalLopRange.Merge().Value = $"Tổng học sinh lớp {lop.Key}: {countLop}";
    totalLopRange.Style.Font.Bold = true;
    totalLopRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
    totalLopRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
    totalLopRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

    row++; 
} 
            worksheet.Range(row,1,row, totalColums).Merge().Value =$"TỔNG HỌC SINH DANH SÁCH: {data.Count}";
            worksheet.Row(row).Style.Font.Bold = true;
            worksheet.Row(row).Style.Font.FontSize = 13;
            worksheet.Row(row).Style.Border.TopBorder = XLBorderStyleValues.Thick;

              worksheet.Column(1).Width = 6; // STT
    worksheet.Column(2).Width = 8;    // Khối
    worksheet.Column(3).Width = 10;   // Mã lớp
    worksheet.Column(4).Width = 12;   // Mã HS
    worksheet.Column(5).Width = 20;   // Họ tên
    worksheet.Column(6).Width = 12;   // Ngày sinh
    worksheet.Column(7).Width = 10;   // Giới tính
    worksheet.Column(8).Width = 22;   // Phụ huynh
    worksheet.Column(9).Width = 14;   // SĐT
    worksheet.Column(10).Width = 25;  // Địa chỉ
    worksheet.Column(11).Width = 12;  // Trạng thái
             worksheet.Column(5).Style.Alignment.WrapText = true;
             worksheet.Column(8).Style.Alignment.WrapText =true;
             worksheet.Column(10).Style.Alignment.WrapText =true;

        // IN
        worksheet.PageSetup.PageOrientation = XLPageOrientation.Portrait;
        worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        worksheet.PageSetup.Margins.Top = 0.5;
        worksheet.PageSetup.Margins.Bottom =0.5;
        worksheet.PageSetup.Margins.Left =0.5;
        worksheet.PageSetup.Margins.Right =0.5;
        worksheet.PageSetup.SetRowsToRepeatAtTop(5, 5);

        
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
          }
    public async Task<int> ImportExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed().RowsUsed().Skip(5);
        var ListHocSinh = new List<HocSinh>();

        foreach (var row in rows)
        {
            var firstCell = row.Cell(4).GetValue<string>();
            if(string.IsNullOrWhiteSpace(firstCell)|| firstCell.Contains("Tổng"))
            continue;
            
            var hs = new HocSinh
            {
                MaKhoi = row.Cell(2).GetValue<string>(),         
    MaLop = row.Cell(3).GetValue<string>(),         
    MaHS = row.Cell(4).GetValue<string>(),           
    HoVaTen = row.Cell(5).GetValue<string>(),       
    
  
   NgaySinh = row.Cell(6).GetValue<string>(),


    
    GioiTinh = row.Cell(7).GetValue<string>(),       
    HoTenPhuHuynh = row.Cell(8).GetValue<string>(),  
    SoDienThoai = row.Cell(9).GetValue<string>(),  
    DiaChi = row.Cell(10).GetValue<string>(),       
    TrangThai = row.Cell(11).GetValue<string>()
            };
              ListHocSinh.Add(hs);

        }

if (ListHocSinh.Count == 0) return 0;

    using var db = new NpgsqlConnection(_connectionString);
    var sql = @"
    INSERT INTO ""HocSinh"" (
        ""MaKhoi"", ""MaLop"", ""MaHS"", ""HoVaTen"", ""NgaySinh"", 
        ""GioiTinh"", ""HoTenPhuHuynh"", ""SoDienThoai"", ""DiaChi"", ""TrangThai""
    )
    VALUES (
        @MaKhoi, @MaLop, @MaHS, @HoVaTen, @NgaySinh, 
        @GioiTinh, @HoTenPhuHuynh, @SoDienThoai, @DiaChi, @TrangThai
    )
    ON CONFLICT (""MaHS"") 
    DO UPDATE SET 
        ""MaKhoi"" = EXCLUDED.""MaKhoi"",
        ""MaLop"" = EXCLUDED.""MaLop"",
        ""HoVaTen"" = EXCLUDED.""HoVaTen"",
        ""NgaySinh"" = EXCLUDED.""NgaySinh"",
        ""GioiTinh"" = EXCLUDED.""GioiTinh"",
        ""HoTenPhuHuynh"" = EXCLUDED.""HoTenPhuHuynh"",
        ""SoDienThoai"" = EXCLUDED.""SoDienThoai"",
        ""DiaChi"" = EXCLUDED.""DiaChi"",
        ""TrangThai"" = EXCLUDED.""TrangThai"";";

    return await db.ExecuteAsync(sql, ListHocSinh);
}

    


}