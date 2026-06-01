using LUnivers_Beaute.Helpers;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;
using BUS;
using System.Linq;

namespace LUnivers_Beaute.Views
{
    public class ChiTietPhieuTraModel : System.ComponentModel.INotifyPropertyChanged
    {
        private int _soLuongTra;
        private decimal _soTienHoan;

        public int MaLo { get; set; }
        public string TenSanPham { get; set; } = "";
        public decimal DonGia { get; set; }
        public int MaxSoLuong { get; set; }

        public int SoLuongTra
        {
            get => _soLuongTra;
            set
            {
                int validValue = value;
                if (validValue < 0) validValue = 0;
                if (validValue > MaxSoLuong) validValue = MaxSoLuong;

                if (_soLuongTra != validValue)
                {
                    _soLuongTra = validValue;
                    OnPropertyChanged(nameof(SoLuongTra));
                    SoTienHoan = _soLuongTra * DonGia;
                }
            }
        }

        public decimal SoTienHoan
        {
            get => _soTienHoan;
            set
            {
                if (_soTienHoan != value)
                {
                    _soTienHoan = value;
                    OnPropertyChanged(nameof(SoTienHoan));
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }

    public class ComboItemModel
    {
        public string Value { get; set; } = "";
        public string Display { get; set; } = "";
        public string ExtraData { get; set; } = "";
        public decimal Price { get; set; }

        public override string ToString()
        {
            return Display;
        }
    }

    public partial class TraHangView : UserControl
    {
        private PhieuTraHangBUS _bus = new PhieuTraHangBUS();
        private PagingHelper _pager;
        private ObservableCollection<ChiTietPhieuTraModel> _chiTietTemp = new ObservableCollection<ChiTietPhieuTraModel>();

        public TraHangView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
            if (dgData != null)
            {
                dgData.SelectionChanged += DgData_SelectionChanged;
            }
            if (cboHoaDon != null)
            {
                cboHoaDon.SelectionChanged += CboHoaDon_SelectionChanged;
            }
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) _pager.SetData(_bus.GetAllPhieuTraHang());
                
                // Binding temp list to dgChiTietPhieuTra_Temp
                if (dgChiTietPhieuTra_Temp != null) dgChiTietPhieuTra_Temp.ItemsSource = _chiTietTemp;

                // Load HoaDon for ComboBox (Only today's invoices)
                DataTable dtHoaDon = new HoaDonBUS().GetAll();
                var listHoaDon = new System.Collections.Generic.List<ComboItemModel>();
                string todayStr = DateTime.Today.ToString("dd/MM/yyyy");
                foreach (DataRow r in dtHoaDon.Rows)
                {
                    string date = r.Table.Columns.Contains("NgayLap") ? r["NgayLap"].ToString() : "";
                    if (!string.IsNullOrEmpty(date) && date.StartsWith(todayStr))
                    {
                        string cus = r.Table.Columns.Contains("KhachHang") ? r["KhachHang"].ToString() : "";
                        listHoaDon.Add(new ComboItemModel
                        {
                            Value = r["MaHoaDon"].ToString(),
                            Display = $"{r["MaHoaDon"]} - Khách: {cus} ({date})",
                            ExtraData = date
                        });
                    }
                }
                cboHoaDon.ItemsSource = listHoaDon;
                cboHoaDon.DisplayMemberPath = "Display";
                cboHoaDon.SelectedValuePath = "Value";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CboHoaDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboHoaDon.SelectedItem is ComboItemModel selectedInvoice)
            {
                // 1. Set return date (NgayTra) to the invoice's date (NgayLap) and make it read-only (disabled)
                if (DateTime.TryParse(selectedInvoice.ExtraData, out DateTime invoiceDate))
                {
                    dpNgayTra.SelectedDate = invoiceDate;
                }
                else
                {
                    dpNgayTra.SelectedDate = DateTime.Now;
                }
                dpNgayTra.IsEnabled = false;

                // Update text label to "Chi tiết sản phẩm hoàn trả của hóa đơn " + MaHoaDon
                if (lblChiTietTitle != null)
                {
                    lblChiTietTitle.Text = "Chi tiết sản phẩm hoàn trả của hóa đơn " + selectedInvoice.Value;
                }

                // Clear previous temporary return details since we changed invoice
                _chiTietTemp.Clear();

                // 2. Load products from this invoice's details directly into return list
                try
                {
                    DataTable dtChiTiet = new HoaDonBUS().GetChiTiet(selectedInvoice.Value);
                    foreach (DataRow r in dtChiTiet.Rows)
                    {
                        string soLo = r.Table.Columns.Contains("SoLo") ? r["SoLo"].ToString() : "?";
                        int maLo = r.Table.Columns.Contains("MaLo") && r["MaLo"] != DBNull.Value ? Convert.ToInt32(r["MaLo"]) : 0;
                        string tenSp = r["TenSanPham"].ToString();
                        int purchasedQty = r.Table.Columns.Contains("SoLuong") && r["SoLuong"] != DBNull.Value ? Convert.ToInt32(r["SoLuong"]) : 0;
                        decimal donGiaNum = 0;
                        if (r.Table.Columns.Contains("DonGiaNum") && r["DonGiaNum"] != DBNull.Value)
                        {
                            donGiaNum = Convert.ToDecimal(r["DonGiaNum"]);
                        }
                        else if (r.Table.Columns.Contains("DonGia") && r["DonGia"] != DBNull.Value)
                        {
                            string rawPrice = r["DonGia"].ToString() ?? "";
                            string cleanPrice = "";
                            foreach (char c in rawPrice)
                            {
                                if (char.IsDigit(c)) cleanPrice += c;
                            }
                            if (!string.IsNullOrEmpty(cleanPrice))
                            {
                                decimal.TryParse(cleanPrice, out donGiaNum);
                            }
                        }

                        _chiTietTemp.Add(new ChiTietPhieuTraModel
                        {
                            MaLo = maLo,
                            TenSanPham = tenSp,
                            DonGia = donGiaNum,
                            MaxSoLuong = purchasedQty,
                            SoLuongTra = purchasedQty, // Default return quantity to purchased quantity
                            SoTienHoan = purchasedQty * donGiaNum // Default return refund amount
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải chi tiết hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                dpNgayTra.SelectedDate = null;
                dpNgayTra.IsEnabled = true;
                _chiTietTemp.Clear();
                if (lblChiTietTitle != null)
                {
                    lblChiTietTitle.Text = "Chi tiết sản phẩm hoàn trả";
                }
            }
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                string? maPhieuTra = row["MaPhieuTra"].ToString();
                try
                {
                    if (dgChiTiet != null && !string.IsNullOrEmpty(maPhieuTra))
                        dgChiTiet.ItemsSource = _bus.GetChiTietPhieuTra(maPhieuTra).DefaultView;
                    if (btnPrint != null)
                        btnPrint.IsEnabled = true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                if (btnPrint != null)
                    btnPrint.IsEnabled = false;
            }
        }
        
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Reset fields
            cboHoaDon.SelectedIndex = -1;
            dpNgayTra.SelectedDate = null;
            dpNgayTra.IsEnabled = true;
            txtLyDo.Clear();
            _chiTietTemp.Clear();
            if (lblChiTietTitle != null)
            {
                lblChiTietTitle.Text = "Chi tiết sản phẩm hoàn trả";
            }
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboHoaDon.SelectedValue == null || string.IsNullOrWhiteSpace(txtLyDo.Text) || _chiTietTemp.Count == 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin (Hóa đơn, Lý do) và ít nhất 1 chi tiết trả hàng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string maHoaDon = cboHoaDon.SelectedValue.ToString();
                string lyDo = txtLyDo.Text;
                DateTime ngayTra = dpNgayTra.SelectedDate ?? DateTime.Now;
                string maPhieuTra = "PT" + DateTime.Now.ToString("yyMMddHHmmss");

                string jsonChiTiet = JsonSerializer.Serialize(_chiTietTemp);

                if (_bus.InsertPhieuTraHang(maPhieuTra, maHoaDon, ngayTra, lyDo, jsonChiTiet))
                {
                    var currentUser = (Application.Current.MainWindow as MainWindow)?.HoTen ?? "Nhân viên";
                    LUnivers_Beaute.Services.LogService.LogEdit(currentUser, "Trả hàng", $"Tạo phiếu trả hàng {maPhieuTra} cho hóa đơn {maHoaDon} thành công", "↩️");
                    MessageBox.Show("Tạo phiếu trả hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    crudPanel.Visibility = Visibility.Collapsed;
                    _chiTietTemp.Clear();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu phiếu trả: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            if (cboChiTietLo.SelectedValue == null || string.IsNullOrWhiteSpace(txtChiTietSoLuong.Text) || string.IsNullOrWhiteSpace(txtChiTietTienHoan.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin chi tiết!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int maLo = Convert.ToInt32(cboChiTietLo.SelectedValue);
                int soLuong = Convert.ToInt32(txtChiTietSoLuong.Text);
                decimal tienHoan = Convert.ToDecimal(txtChiTietTienHoan.Text);
                
                ComboItemModel item = (ComboItemModel)cboChiTietLo.SelectedItem;
                string tenSanPham = item.ExtraData;

                _chiTietTemp.Add(new ChiTietPhieuTraModel
                {
                    MaLo = maLo,
                    TenSanPham = tenSanPham,
                    SoLuongTra = soLuong,
                    SoTienHoan = tienHoan
                });

                txtChiTietSoLuong.Clear();
                txtChiTietTienHoan.Clear();
            }
            catch
            {
                MessageBox.Show("Vui lòng nhập số liệu hợp lệ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDecreaseQty_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ChiTietPhieuTraModel item)
            {
                item.SoLuongTra--;
            }
        }

        private void BtnIncreaseQty_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ChiTietPhieuTraModel item)
            {
                item.SoLuongTra++;
            }
        }

        private void BtnRemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ChiTietPhieuTraModel item)
            {
                _chiTietTemp.Remove(item);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
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
            if (dgData.SelectedItem is DataRowView row)
            {
                string maPhieuTra = row["MaPhieuTra"]?.ToString() ?? "";
                string maHoaDon = row["MaHoaDon"]?.ToString() ?? "";
                string lyDo = row["LyDo"]?.ToString() ?? "";

                txtPrintMaPhieu.Text = maPhieuTra;
                txtPrintMaHoaDon.Text = maHoaDon;
                txtPrintLyDo.Text = lyDo;

                // Pre-select condition
                cboPrintTinhTrang.SelectedIndex = 0;

                // Calculate refund amount
                decimal tongTienHoan = 0;
                try
                {
                    if (dgChiTiet.ItemsSource is DataView dv)
                    {
                        foreach (DataRowView r in dv)
                        {
                            string tienStr = r["SoTienHoan"]?.ToString() ?? "0";
                            tienStr = tienStr.Replace(" ₫", "").Replace(",", "").Replace(".", "").Trim();
                            if (decimal.TryParse(tienStr, out decimal tien))
                            {
                                tongTienHoan += tien;
                            }
                        }
                    }
                }
                catch { }

                txtPrintTienHoan.Text = string.Format("{0:N0} ₫", tongTienHoan);
                printPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnPrintCancel_Click(object sender, RoutedEventArgs e)
        {
            printPanel.Visibility = Visibility.Collapsed;
        }

        private void PrintOverlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            printPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnPrintConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                try
                {
                    string maPhieuTra = txtPrintMaPhieu.Text;
                    string maHoaDon = txtPrintMaHoaDon.Text;
                    string lyDo = txtPrintLyDo.Text;
                    string tinhTrang = (cboPrintTinhTrang.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Còn nguyên seal";

                    // Retrieve details
                    var itemsList = new System.Collections.Generic.List<Services.ReturnItem>();
                    decimal tongTienHoanVal = 0;

                    if (dgChiTiet.ItemsSource is DataView dv)
                    {
                        foreach (DataRowView r in dv)
                        {
                            string tenSp = r["TenSanPham"]?.ToString() ?? "";
                            int qty = Convert.ToInt32(r["SoLuongTra"]);
                            
                            string tienStr = r["SoTienHoan"]?.ToString() ?? "0";
                            tienStr = tienStr.Replace(" ₫", "").Replace(",", "").Replace(".", "").Trim();
                            decimal.TryParse(tienStr, out decimal tienHoan);

                            decimal unitPrice = qty > 0 ? tienHoan / qty : 0;
                            tongTienHoanVal += tienHoan;

                            itemsList.Add(new Services.ReturnItem
                            {
                                TenSanPham = tenSp,
                                SoLuong = qty,
                                DonGia = unitPrice
                            });
                        }
                    }

                    // Get cashier & store info
                    string cashierName = "Nhân viên L'Univers";
                    string storeName = "L'UNIVERS BEAUTÉ";
                    string storeId = "CH01";

                    if (Application.Current.MainWindow is MainWindow mw)
                    {
                        if (!string.IsNullOrEmpty(mw.HoTen)) cashierName = mw.HoTen;
                        if (!string.IsNullOrEmpty(mw.TenCuaHang)) storeName = mw.TenCuaHang;
                        if (!string.IsNullOrEmpty(mw.MaCuaHang)) storeId = mw.MaCuaHang;
                    }

                    string diaChiCuaHang = "L'UNIVERS BEAUTÉ - Trụ sở chính";
                    string sdtCuaHang = "1900 9999";
                    try
                    {
                        CuaHangBUS cuaHangBus = new CuaHangBUS();
                        var dt = cuaHangBus.GetAll();
                        var chRow = dt.AsEnumerable().FirstOrDefault(r => r["MaCuaHang"]?.ToString() == storeId);
                        if (chRow != null)
                        {
                            if (dt.Columns.Contains("DiaChi")) diaChiCuaHang = chRow["DiaChi"]?.ToString() ?? diaChiCuaHang;
                            if (dt.Columns.Contains("SoDienThoai")) sdtCuaHang = chRow["SoDienThoai"]?.ToString() ?? sdtCuaHang;
                        }
                    }
                    catch { }

                    DateTime ngayTra = DateTime.Now;
                    if (row["NgayTra"] != DBNull.Value)
                    {
                        DateTime.TryParse(row["NgayTra"].ToString(), out ngayTra);
                    }

                    var model = new Services.ReturnModel
                    {
                        MaPhieuTra = maPhieuTra,
                        MaHoaDonGoc = maHoaDon,
                        NgayTra = ngayTra,
                        TenNhanVien = cashierName,
                        TenCuaHang = storeName,
                        DiaChiCuaHang = diaChiCuaHang,
                        SdtCuaHang = sdtCuaHang,
                        LyDoTra = lyDo,
                        TinhTrang = tinhTrang,
                        Items = itemsList,
                        TongTienHoan = tongTienHoanVal
                    };

                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                        FileName = $"{maPhieuTra}.pdf",
                        Title = "Chọn nơi lưu phiếu trả hàng"
                    };

                    if (saveFileDialog.ShowDialog() != true)
                    {
                        return; // Người dùng hủy bỏ lưu file
                    }

                    string pdfPath = saveFileDialog.FileName;

                    var pdfService = new Services.PdfReturnService();
                    pdfService.GenerateReturnReceipt(model, pdfPath);

                    printPanel.Visibility = Visibility.Collapsed;

                    MessageBox.Show("In phiếu trả hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = pdfPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi in phiếu trả: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
