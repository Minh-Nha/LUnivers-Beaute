using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class HoaDonView : UserControl
    {
        private HoaDonBUS _bus = new HoaDonBUS();
        private PagingHelper _pager;

        public HoaDonView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) 
                {
                    _pager.SetData(_bus.GetAll());
                    if (dgData.Items.Count > 0)
                    {
                        dgData.SelectedIndex = 0;
                    }
                    else
                    {
                        detailPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                string maHoaDon = row["MaHoaDon"].ToString();
                
                // Populate Invoice Info
                lblMaHD.Text = maHoaDon;
                lblNgayLap.Text = row.Row.Table.Columns.Contains("NgayLap") ? row["NgayLap"].ToString() : "";
                lblNhanVien.Text = row.Row.Table.Columns.Contains("NhanVienLap") ? row["NhanVienLap"].ToString() : "";
                lblKhachHang.Text = row.Row.Table.Columns.Contains("KhachHang") ? row["KhachHang"].ToString() : "";
                lblThanhToan.Text = row.Row.Table.Columns.Contains("PhuongThucThanhToan") ? row["PhuongThucThanhToan"].ToString() : "";
                lblCuaHang.Text = row.Row.Table.Columns.Contains("TenCuaHang") ? row["TenCuaHang"].ToString() : "";
                
                // Populate summary
                double tongTienHD = 0;
                if (row.Row.Table.Columns.Contains("TongTien")) 
                    tongTienHD = ParseCurrency(row["TongTien"]);

                double khuyenMai = 0;
                if (row.Row.Table.Columns.Contains("KhuyenMai")) 
                    khuyenMai = ParseCurrency(row["KhuyenMai"]);
                
                try
                {
                    if (dgChiTiet != null)
                    {
                        var chiTiet = _bus.GetChiTiet(maHoaDon);
                        dgChiTiet.ItemsSource = chiTiet.DefaultView;
                        
                        double totalChiTiet = 0;
                        foreach(DataRow r in chiTiet.Rows) 
                        {
                            if (r.Table.Columns.Contains("ThanhTien")) 
                            {
                                totalChiTiet += ParseCurrency(r["ThanhTien"]);
                            }
                            else if (r.Table.Columns.Contains("DonGia") && r.Table.Columns.Contains("SoLuong"))
                            {
                                totalChiTiet += ParseCurrency(r["DonGia"]) * ParseCurrency(r["SoLuong"]);
                            }
                        }

                        // Nếu hóa đơn có Khuyến mãi, thì Tổng tiền gốc = Khách trả + Khuyến mãi
                        if (khuyenMai > 0 && tongTienHD > 0 && totalChiTiet == 0)
                        {
                            totalChiTiet = tongTienHD + khuyenMai;
                        }
                        else if (totalChiTiet > tongTienHD && khuyenMai == 0)
                        {
                            // Tự động suy ra khuyến mãi
                            khuyenMai = totalChiTiet - tongTienHD;
                        }
                        else if (totalChiTiet > 0 && tongTienHD == 0)
                        {
                            // Nếu không có cột tổng tiền hóa đơn, lấy từ chi tiết
                            tongTienHD = totalChiTiet - khuyenMai;
                        }

                        lblTongTien.Text = totalChiTiet.ToString("#,##0") + " đ";
                        lblKhuyenMai.Text = khuyenMai.ToString("#,##0") + " đ";
                        lblCanTra.Text = tongTienHD.ToString("#,##0") + " đ";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                detailPanel.Visibility = Visibility.Visible;
            }
        }

        private double ParseCurrency(object obj)
        {
            if (obj == null || obj == DBNull.Value) return 0;
            string str = obj.ToString().Replace("đ", "").Replace("₫", "").Replace(" ", "").Replace(",", "").Replace(".", "");
            if (double.TryParse(str, out double val)) return val;
            return 0;
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.PreviousPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.NextPage();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lblMaHD.Text) || dgChiTiet.ItemsSource == null) 
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để in!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PDF Document (*.pdf)|*.pdf";
            saveFileDialog.FileName = lblMaHD.Text;
            saveFileDialog.Title = "Lưu Hóa Đơn PDF";

            if (saveFileDialog.ShowDialog() == true)
            {
                try 
                {
                    var model = new LUnivers_Beaute.Services.InvoiceModel
                    {
                        MaHoaDon = lblMaHD.Text,
                        TenNhanVien = lblNhanVien.Text,
                        TenKhachHang = lblKhachHang.Text,
                        TenCuaHang = lblCuaHang.Text,
                        DiaChiCuaHang = "L'UNIVERS BEAUTÉ - Trụ sở chính", // Có thể lấy từ DB nếu cần
                        SdtCuaHang = "1900 9999",
                        PhuongThucThanhToan = lblThanhToan.Text,
                        Items = new System.Collections.Generic.List<LUnivers_Beaute.Services.InvoiceItem>()
                    };

                    if (DateTime.TryParse(lblNgayLap.Text, out DateTime dt)) 
                        model.NgayTao = dt;
                    else 
                        model.NgayTao = DateTime.Now;

                    double tongTien = ParseCurrency(lblTongTien.Text);
                    double khuyenMai = ParseCurrency(lblKhuyenMai.Text);
                    double canTra = ParseCurrency(lblCanTra.Text);

                    model.TamTinh = (decimal)tongTien;
                    model.GiamGia = (decimal)khuyenMai;
                    model.TongCong = (decimal)canTra;
                    model.TienKhachDua = (decimal)canTra; // Lịch sử hóa đơn mặc định coi như đã nhận đủ
                    model.TienThoi = 0;

                    foreach (DataRowView row in (DataView)dgChiTiet.ItemsSource)
                    {
                        int sl = Convert.ToInt32(ParseCurrency(row["SoLuong"]));
                        decimal thanhTien = (decimal)ParseCurrency(row["ThanhTien"]);
                        decimal donGia = sl > 0 ? thanhTien / sl : 0;
                        
                        if (row.Row.Table.Columns.Contains("DonGia"))
                            donGia = (decimal)ParseCurrency(row["DonGia"]);

                        model.Items.Add(new LUnivers_Beaute.Services.InvoiceItem
                        {
                            TenSanPham = row["TenSanPham"].ToString(),
                            SoLuong = sl,
                            DonGia = donGia
                        });
                    }

                    var pdfService = new LUnivers_Beaute.Services.PdfInvoiceService();
                    pdfService.GenerateInvoice(model, saveFileDialog.FileName);

                    MessageBox.Show("Đã lưu hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo PDF: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = _bus.GetAll();
                ExcelExportHelper.ExportToExcel(dt, "BaoCaoHoaDon.csv");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất báo cáo: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
