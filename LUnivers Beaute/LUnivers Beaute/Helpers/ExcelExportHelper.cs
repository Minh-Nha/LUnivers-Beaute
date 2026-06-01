using System.IO;
using System.Text;
using System.Data;
using System.Linq;
using Microsoft.Win32;
using System.Windows;
using ClosedXML.Excel;

namespace LUnivers_Beaute.Helpers
{
    public static class ExcelExportHelper
    {
        public static void ExportToExcel(DataTable dt, string defaultFileName)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                LUnivers_Beaute.Helpers.ModernMessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Đổi đuôi mở rộng thành .xlsx chuyên nghiệp thay vì .csv cũ
            string excelFileName = defaultFileName;
            if (excelFileName.EndsWith(".csv", System.StringComparison.OrdinalIgnoreCase))
            {
                excelFileName = excelFileName.Substring(0, excelFileName.Length - 4) + ".xlsx";
            }
            else if (!excelFileName.EndsWith(".xlsx", System.StringComparison.OrdinalIgnoreCase))
            {
                excelFileName += ".xlsx";
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FileName = excelFileName,
                Title = "Chọn nơi lưu file Excel"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Dữ liệu");

                        // Hiện rõ đường lưới ô Gridlines trong Excel
                        worksheet.ShowGridLines = true;

                        int colCount = dt.Columns.Count;

                        // 1. Tiêu đề thương hiệu cao cấp L'Univers Beauté (Màu tím chủ đạo #6D28D9)
                        worksheet.Cell("A1").Value = "✦ L'UNIVERS BEAUTÉ ✦";
                        var brandRange = worksheet.Range(1, 1, 1, colCount);
                        brandRange.Merge();
                        
                        brandRange.Style.Font.Bold = true;
                        brandRange.Style.Font.FontSize = 14;
                        brandRange.Style.Font.FontColor = XLColor.White;
                        brandRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#6D28D9");
                        brandRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        brandRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Row(1).Height = 35;

                        // 2. Metadata báo cáo hệ thống
                        worksheet.Cell("A2").Value = $"Báo cáo tự động - Ngày xuất: {System.DateTime.Now:dd/MM/yyyy HH:mm}";
                        var metaRange = worksheet.Range(2, 1, 2, colCount);
                        metaRange.Merge();
                        
                        metaRange.Style.Font.Italic = true;
                        metaRange.Style.Font.FontSize = 10;
                        metaRange.Style.Font.FontColor = XLColor.SlateGray;
                        metaRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        metaRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Row(2).Height = 22;

                        // 3. Đổ dữ liệu từ DataTable vào bắt đầu từ dòng thứ 4
                        var tableRange = worksheet.Cell(4, 1).InsertTable(dt);

                        // 4. Style Header của bảng (Màu tím nhẹ #7C3AED)
                        var headerStyle = tableRange.HeadersRow().Style;
                        headerStyle.Font.Bold = true;
                        headerStyle.Font.FontSize = 11;
                        headerStyle.Font.FontColor = XLColor.White;
                        headerStyle.Fill.BackgroundColor = XLColor.FromHtml("#7C3AED");
                        headerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Row(4).Height = 28;

                        // 5. Định dạng dữ liệu chi tiết
                        var dataRows = tableRange.DataRange.Rows();
                        foreach (var row in dataRows)
                        {
                            row.WorksheetRow().Height = 22;
                            foreach (var cell in row.Cells())
                            {
                                // Viền kẻ mờ tinh tế #E5E7EB cho từng ô
                                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.TopBorderColor = XLColor.FromHtml("#E5E7EB");
                                cell.Style.Border.BottomBorderColor = XLColor.FromHtml("#E5E7EB");
                                cell.Style.Border.LeftBorderColor = XLColor.FromHtml("#E5E7EB");
                                cell.Style.Border.RightBorderColor = XLColor.FromHtml("#E5E7EB");

                                cell.Style.Font.FontSize = 10;
                                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                                string columnName = dt.Columns[cell.Address.ColumnNumber - 1].ColumnName;
                                string cellValue = cell.Value.ToString() ?? "";

                                // Định dạng các cột tiền tệ
                                if (columnName.Contains("Tien", System.StringComparison.OrdinalIgnoreCase) || 
                                    columnName.Contains("Gia", System.StringComparison.OrdinalIgnoreCase) || 
                                    columnName.Contains("Tri", System.StringComparison.OrdinalIgnoreCase) || 
                                    columnName.Contains("ThanhTien", System.StringComparison.OrdinalIgnoreCase) ||
                                    columnName.Contains("TongTien", System.StringComparison.OrdinalIgnoreCase) ||
                                    columnName.Contains("DonGia", System.StringComparison.OrdinalIgnoreCase))
                                {
                                    // Tách riêng ký tự số để lưu đúng dạng số trong Excel
                                    string cleanVal = new string(cellValue.Where(c => char.IsDigit(c) || c == '-' || c == '.' || c == ',').ToArray());
                                    if (double.TryParse(cleanVal, out double numVal))
                                    {
                                        cell.Value = numVal;
                                    }
                                    cell.Style.NumberFormat.Format = "#,##0\" đ\"";
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }
                                // Định dạng các cột ngày tháng
                                else if (columnName.Contains("Ngay", System.StringComparison.OrdinalIgnoreCase) || 
                                         columnName.Contains("Han", System.StringComparison.OrdinalIgnoreCase))
                                {
                                    if (System.DateTime.TryParse(cellValue, out System.DateTime dateVal))
                                    {
                                        cell.Value = dateVal;
                                        cell.Style.NumberFormat.Format = "dd/MM/yyyy";
                                    }
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }
                                // Định dạng căn giữa cho Mã, SĐT, Số lô
                                else if (columnName.Contains("Ma", System.StringComparison.OrdinalIgnoreCase) || 
                                         columnName.Contains("SDT", System.StringComparison.OrdinalIgnoreCase) || 
                                         columnName.Contains("DienThoai", System.StringComparison.OrdinalIgnoreCase) ||
                                         columnName.Contains("SoLo", System.StringComparison.OrdinalIgnoreCase))
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }
                                else
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                }
                            }
                        }

                        // 6. Tự co giãn các cột vừa vặn nội dung
                        worksheet.Columns().AdjustToContents();

                        // Thêm khoảng đệm an toàn tránh việc cột quá sát nhau
                        foreach (var col in worksheet.Columns())
                        {
                            col.Width += 3;
                        }

                        // 7. Lưu lại file Excel
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    LUnivers_Beaute.Helpers.ModernMessageBox.Show("Xuất file Excel thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Tự động mở file Excel sau khi xuất
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (System.Exception ex)
                {
                    LUnivers_Beaute.Helpers.ModernMessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
