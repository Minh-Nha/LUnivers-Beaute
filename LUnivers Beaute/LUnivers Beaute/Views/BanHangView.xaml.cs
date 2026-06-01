using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS;
using System.ComponentModel;
using LUnivers_Beaute.Helpers;
using LUnivers_Beaute.Services;

namespace LUnivers_Beaute.Views
{
    public partial class BanHangView : UserControl
    {
        private HoaDonBUS _hoaDonBus = new HoaDonBUS();
        private DanhMucBUS _danhMucBus = new DanhMucBUS();
        private CuaHangBUS _cuaHangBus = new CuaHangBUS();
        private KhachHangBUS _khachHangBus = new KhachHangBUS();
        private KhuyenMaiBUS _khuyenMaiBus = new KhuyenMaiBUS();
        private NhanVienBUS _nhanVienBus = new NhanVienBUS();
        private ObservableCollection<SanPhamDTO> _products = new ObservableCollection<SanPhamDTO>();
        private ObservableCollection<CartItem> _cart = new ObservableCollection<CartItem>();
        private DataTable _dtKhachHang;

        private int _currentPage = 1;
        private int _pageSize = 8;
        private int _totalPages = 1;

        private string _currentCuaHang = "";
        private string _currentNhanVien = "";

        // Promotion cache
        private string _promoLoaiGiam = "";
        private double _promoGiaTriGiam = 0;

        public BanHangView(string maNhanVien = "", string maCuaHang = "")
        {
            InitializeComponent();
            _currentNhanVien = maNhanVien;
            _currentCuaHang = maCuaHang;

            icProducts.ItemsSource = _products;
            icCart.ItemsSource = _cart;
            _cart.CollectionChanged += (s, e) => UpdateCartTotal();

            this.Loaded += (s, e) =>
            {
                LoadCuaHang();
                LoadDanhMuc();
                LoadKhachHang();
                LoadKhuyenMai();
                LoadProducts();
            };
        }

        // ===== DATA LOADING =====

        private void LoadCuaHang()
        {
            try
            {
                var dt = _cuaHangBus.GetAll();
                if (!string.IsNullOrEmpty(_currentCuaHang))
                {
                    var rows = dt.Select($"MaCuaHang = '{_currentCuaHang}'");
                    if (rows.Length > 0)
                        dt = rows.CopyToDataTable();
                    else
                        dt.Rows.Clear();
                }

                cboCuaHang.ItemsSource = dt.DefaultView;
                cboCuaHang.SelectedValuePath = "MaCuaHang";
                if (dt.Rows.Count > 0)
                {
                    cboCuaHang.SelectedIndex = 0;
                    _currentCuaHang = dt.Rows[0]["MaCuaHang"]?.ToString() ?? "CH01";
                }
            }
            catch { }
        }

        private void LoadDanhMuc()
        {
            try
            {
                var dt = _danhMucBus.GetAll();
                DataRow dr = dt.NewRow();
                dr["MaDanhMuc"] = 0;
                dr["TenDanhMuc"] = "Tất cả danh mục";
                dt.Rows.InsertAt(dr, 0);

                cboDanhMuc.ItemsSource = dt.DefaultView;
                cboDanhMuc.SelectedValuePath = "MaDanhMuc";
                cboDanhMuc.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadKhachHang()
        {
            try
            {
                _dtKhachHang = _khachHangBus.GetAll(null, 1); // Only active customers
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadKhachHang error: " + ex.Message);
            }
        }

        private void LoadKhuyenMai()
        {
            try
            {
                var dt = _khuyenMaiBus.GetAll();
                // Filter active promotions only (within date range)
                var activeRows = dt.AsEnumerable().Where(r =>
                {
                    var trangThai = r["TrangThai"]?.ToString();
                    return trangThai == "Đang diễn ra" || trangThai == "True" || trangThai == "1";
                });

                DataTable activeDt;
                if (activeRows.Any())
                    activeDt = activeRows.CopyToDataTable();
                else
                    activeDt = dt.Clone();

                DataRow dr = activeDt.NewRow();
                dr["MaKhuyenMai"] = 0;
                dr["TenChuongTrinh"] = "Không áp dụng KM";
                activeDt.Rows.InsertAt(dr, 0);

                cboKhuyenMai.ItemsSource = activeDt.DefaultView;
                cboKhuyenMai.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadProducts()
        {
            try
            {
                string searchTerm = txtSearch?.Text?.Trim() ?? "";
                int? maDanhMuc = null;
                if (cboDanhMuc?.SelectedValue != null && int.TryParse(cboDanhMuc.SelectedValue.ToString(), out int dm) && dm > 0)
                {
                    maDanhMuc = dm;
                }

                int totalRecords = 0;
                var dt = _hoaDonBus.GetSanPhamBanHangPaged(_currentCuaHang, searchTerm, maDanhMuc, _currentPage, _pageSize, out totalRecords);
                
                _totalPages = (int)Math.Ceiling((double)totalRecords / _pageSize);
                if (_totalPages < 1) _totalPages = 1;

                if (txtPageInfo != null)
                {
                    txtPageInfo.Text = $"Trang {_currentPage} / {_totalPages}";
                    btnPrevPage.IsEnabled = _currentPage > 1;
                    btnNextPage.IsEnabled = _currentPage < _totalPages;
                }

                _products.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    _products.Add(new SanPhamDTO
                    {
                        MaSanPham = row["MaSanPham"]?.ToString() ?? "",
                        TenSanPham = row["TenSanPham"]?.ToString() ?? "",
                        TenThuongHieu = row["TenThuongHieu"]?.ToString() ?? "",
                        GiaNiemYet = Convert.ToDecimal(row["GiaNiemYet"]),
                        SoLuongTon = Convert.ToInt32(row["SoLuongTon"]),
                        HinhAnh = row["HinhAnh"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tải sản phẩm", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== EVENT HANDLERS =====

        private void CboCuaHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboCuaHang.SelectedItem is DataRowView row)
            {
                _currentCuaHang = row["MaCuaHang"]?.ToString() ?? "CH01";
                _cart.Clear();
                _currentPage = 1;
                LoadProducts();
            }
        }

        private void CboDanhMuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            LoadProducts();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentPage = 1;
            LoadProducts();
        }

        private void TxtSoDienThoai_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCartTotal();
        }

        private void BtnClearKhachHang_Click(object sender, RoutedEventArgs e)
        {
            txtSoDienThoai.Text = "";
            UpdateCartTotal();
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void TxtTienKhachDua_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Format currency while typing
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length > 0)
            {
                string valueStr = textBox.Text.Replace(",", "").Replace(".", "");
                if (decimal.TryParse(valueStr, out decimal value))
                {
                    textBox.TextChanged -= TxtTienKhachDua_TextChanged;
                    textBox.Text = string.Format("{0:N0}", value);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.TextChanged += TxtTienKhachDua_TextChanged;
                }
            }
            UpdateCartTotal();
        }

        private void CboKhuyenMai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cache promotion info
            if (cboKhuyenMai.SelectedItem is DataRowView row)
            {
                int maKM = 0;
                int.TryParse(row["MaKhuyenMai"]?.ToString(), out maKM);
                if (maKM > 0)
                {
                    _promoLoaiGiam = row["LoaiGiam"]?.ToString() ?? "";
                    double.TryParse(row["GiaTriGiam"]?.ToString(), out _promoGiaTriGiam);
                }
                else
                {
                    _promoLoaiGiam = "";
                    _promoGiaTriGiam = 0;
                }
            }
            UpdateCartTotal();
        }

        private void CboThanhToan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCartTotal();
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadProducts();
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadProducts();
            }
        }

        // ===== CART OPERATIONS =====

        private void Product_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.DataContext is SanPhamDTO sp)
            {
                var existing = _cart.FirstOrDefault(c => c.MaSanPham == sp.MaSanPham);
                if (existing != null)
                {
                    if (existing.SoLuong < sp.SoLuongTon)
                    {
                        existing.SoLuong++;
                    }
                    else
                    {
                        MessageBox.Show("Số lượng tồn kho không đủ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    if (sp.SoLuongTon > 0)
                    {
                        var newItem = new CartItem
                        {
                            MaSanPham = sp.MaSanPham ?? "",
                            TenSanPham = sp.TenSanPham ?? "",
                            DonGia = sp.GiaNiemYet,
                            SoLuong = 1,
                            SoLuongTon = sp.SoLuongTon
                        };
                        newItem.PropertyChanged += (s, ev) => { if (ev.PropertyName == "ThanhTien") UpdateCartTotal(); };
                        _cart.Add(newItem);
                    }
                    else
                    {
                        MessageBox.Show("Sản phẩm đã hết hàng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void BtnGiam_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string maSp)
            {
                var item = _cart.FirstOrDefault(c => c.MaSanPham == maSp);
                if (item != null)
                {
                    if (item.SoLuong > 1) item.SoLuong--;
                    else _cart.Remove(item);
                }
            }
        }

        private void BtnTang_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string maSp)
            {
                var item = _cart.FirstOrDefault(c => c.MaSanPham == maSp);
                if (item != null)
                {
                    if (item.SoLuong < item.SoLuongTon) item.SoLuong++;
                    else MessageBox.Show("Số lượng tồn kho không đủ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string maSp)
            {
                var item = _cart.FirstOrDefault(c => c.MaSanPham == maSp);
                if (item != null) _cart.Remove(item);
            }
        }

        private void BtnXoaTatCa_Click(object sender, RoutedEventArgs e)
        {
            _cart.Clear();
        }

        // ===== CART TOTAL =====

        private void UpdateCartTotal()
        {
            if (txtTamTinh == null) return;

            decimal subtotal = _cart.Sum(c => c.ThanhTien);
            decimal discount = 0;

            // Calculate promotion discount
            if (_promoGiaTriGiam > 0)
            {
                if (_promoLoaiGiam == "%")
                    discount = subtotal * (decimal)_promoGiaTriGiam / 100m;
                else if (_promoLoaiGiam == "VND")
                    discount = (decimal)_promoGiaTriGiam;
            }

            if (discount > subtotal) discount = subtotal;
            decimal total = subtotal - discount;
            if (total < 0) total = 0;

            txtTamTinh.Text = $"{subtotal:N0} ₫";
            txtKhuyenMai.Text = discount > 0 ? $"-{discount:N0} ₫" : "-0 ₫";
            txtTongCong.Text = $"{total:N0} ₫";
            txtCartCount.Text = _cart.Count.ToString();

            // Show loyalty points for selected customer
            int diem = 0;
            string sdt = txtSoDienThoai.Text.Trim();
            if (_dtKhachHang != null && !string.IsNullOrEmpty(sdt))
            {
                var row = _dtKhachHang.AsEnumerable().FirstOrDefault(r => r["SoDienThoai"]?.ToString() == sdt);
                if (row != null)
                {
                    int.TryParse(row["DiemTichLuy"]?.ToString(), out diem);
                    txtDiemTichLuy.Text = $"⭐ {diem:N0}";
                    txtTenKhachHang.Text = row["HoTen"]?.ToString();
                    txtTenKhachHang.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80)); // #4CAF50
                }
                else
                {
                    txtDiemTichLuy.Text = "⭐ 0";
                    txtTenKhachHang.Text = "Không tìm thấy KH";
                    txtTenKhachHang.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(244, 67, 54)); // #F44336
                }
            }
            else
            {
                txtDiemTichLuy.Text = "⭐ 0";
                txtTenKhachHang.Text = "Khách vãng lai";
                txtTenKhachHang.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));
            }

            btnThanhToan.IsEnabled = _cart.Count > 0;

            // Calculate Tiền thối
            string tienKhachDuaStr = txtTienKhachDua.Text.Replace(",", "").Replace(".", "");

            // Auto fill for non-cash
            if (cboThanhToan?.SelectedItem is ComboBoxItem item)
            {
                string method = item.Content?.ToString();
                if (method == "Chuyển khoản" || method == "Thẻ tín dụng")
                {
                    txtTienKhachDua.TextChanged -= TxtTienKhachDua_TextChanged;
                    txtTienKhachDua.Text = total > 0 ? string.Format("{0:N0}", total) : "";
                    txtTienKhachDua.TextChanged += TxtTienKhachDua_TextChanged;
                    tienKhachDuaStr = total > 0 ? total.ToString() : "";
                }
            }

            if (decimal.TryParse(tienKhachDuaStr, out decimal tienKhachDua))
            {
                decimal tienThoi = tienKhachDua - total;
                if (tienThoi < 0)
                {
                    txtTienThoi.Text = $"Thiếu: {Math.Abs(tienThoi):N0} ₫";
                    txtTienThoi.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                }
                else
                {
                    txtTienThoi.Text = $"{tienThoi:N0} ₫";
                    txtTienThoi.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 45));
                }
            }
            else
            {
                txtTienThoi.Text = "0 ₫";
                txtTienThoi.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 45));
            }
        }

        // ===== CHECKOUT =====

        private void BtnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0) return;

            string tienKhachDuaStr = txtTienKhachDua.Text.Replace(",", "").Replace(".", "");
            if (!ValidationHelper.IsNonNegativeDecimal(tienKhachDuaStr, out decimal tienKhachDua))
            {
                ModernMessageBox.Show("Vui lòng nhập số tiền khách đưa hợp lệ!", "Lỗi thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string tongCongStr = txtTongCong.Text.Replace(" ₫", "").Replace(",", "").Replace(".", "");
            if (decimal.TryParse(tongCongStr, out decimal tongCong))
            {
                if (tienKhachDua < tongCong)
                {
                    ModernMessageBox.Show("Số tiền khách đưa chưa đủ để thanh toán hóa đơn!", "Lỗi thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                string maHoaDon = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string phuongThuc = (cboThanhToan.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tiền mặt";

                // Get selected customer
                int? maKhachHang = null;
                string sdt = txtSoDienThoai.Text.Trim();
                if (_dtKhachHang != null && !string.IsNullOrEmpty(sdt))
                {
                    var khRow = _dtKhachHang.AsEnumerable().FirstOrDefault(r => r["SoDienThoai"]?.ToString() == sdt);
                    if (khRow != null)
                    {
                        int maKH = 0;
                        int.TryParse(khRow["MaKhachHang"]?.ToString(), out maKH);
                        if (maKH > 0) maKhachHang = maKH;
                    }
                }

                // Get selected promotion
                int? maKhuyenMai = null;
                if (cboKhuyenMai.SelectedItem is DataRowView kmRow)
                {
                    int maKM = 0;
                    int.TryParse(kmRow["MaKhuyenMai"]?.ToString(), out maKM);
                    if (maKM > 0) maKhuyenMai = maKM;
                }

                var cartJsonList = _cart.Select(c => new { MaSanPham = c.MaSanPham, SoLuong = c.SoLuong, DonGia = c.DonGia }).ToList();
                string json = JsonSerializer.Serialize(cartJsonList);

                if (phuongThuc == "Chuyển khoản")
                {
                    var qrWindow = new VietQRWindow(tongCong, maHoaDon);
                    qrWindow.Owner = Window.GetWindow(this);
                    if (qrWindow.ShowDialog() != true)
                    {
                        // Hủy thanh toán
                        return;
                    }
                }

                _hoaDonBus.TaoHoaDon(maHoaDon, _currentCuaHang, _currentNhanVien, maKhachHang, maKhuyenMai, phuongThuc, json);

                // --- GENERATE PDF ---
                try
                {
                    string tenCuaHang = _currentCuaHang;
                    string diaChiCuaHang = "";
                    string sdtCuaHang = "";
                    if (cboCuaHang.ItemsSource is DataView dvCuaHang)
                    {
                        var chRow = dvCuaHang.Table.AsEnumerable().FirstOrDefault(r => r["MaCuaHang"]?.ToString() == _currentCuaHang);
                        if (chRow != null)
                        {
                            tenCuaHang = chRow["TenCuaHang"]?.ToString() ?? _currentCuaHang;
                            if (dvCuaHang.Table.Columns.Contains("DiaChi")) diaChiCuaHang = chRow["DiaChi"]?.ToString();
                            if (dvCuaHang.Table.Columns.Contains("SoDienThoai")) sdtCuaHang = chRow["SoDienThoai"]?.ToString();
                        }
                    }

                    string tenNhanVien = _currentNhanVien;
                    try
                    {
                        var dtNV = _nhanVienBus.GetAll();
                        var nvRow = dtNV.AsEnumerable().FirstOrDefault(r => r["MaNhanVien"]?.ToString() == _currentNhanVien);
                        if (nvRow != null) tenNhanVien = nvRow["HoTen"]?.ToString() ?? _currentNhanVien;
                    } catch { }

                    decimal tamTinh = _cart.Sum(c => c.ThanhTien);
                    decimal tongCongVal = 0;
                    decimal.TryParse(txtTongCong.Text.Replace(" ₫", "").Replace(",", "").Replace(".", ""), out tongCongVal);
                    decimal giamGiaVal = tamTinh - tongCongVal;
                    if (giamGiaVal < 0) giamGiaVal = 0;

                    var invoiceModel = new LUnivers_Beaute.Services.InvoiceModel
                    {
                        MaHoaDon = maHoaDon,
                        NgayTao = DateTime.Now,
                        TenNhanVien = tenNhanVien,
                        TenCuaHang = tenCuaHang,
                        DiaChiCuaHang = diaChiCuaHang,
                        SdtCuaHang = sdtCuaHang,
                        TenKhachHang = txtTenKhachHang.Text,
                        SdtKhachHang = txtSoDienThoai.Text.Trim(),
                        TamTinh = tamTinh,
                        GiamGia = giamGiaVal,
                        TongCong = tongCongVal,
                        TienKhachDua = tienKhachDua,
                        TienThoi = tienKhachDua - tongCongVal,
                        PhuongThucThanhToan = phuongThuc
                    };

                    foreach (var item in _cart)
                    {
                        invoiceModel.Items.Add(new LUnivers_Beaute.Services.InvoiceItem
                        {
                            TenSanPham = item.TenSanPham,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia
                        });
                    }

                    string invoicesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Invoices");
                    if (!System.IO.Directory.Exists(invoicesDir)) System.IO.Directory.CreateDirectory(invoicesDir);
                    string pdfPath = System.IO.Path.Combine(invoicesDir, $"{maHoaDon}.pdf");

                    var pdfService = new LUnivers_Beaute.Services.PdfInvoiceService();
                    pdfService.GenerateInvoice(invoiceModel, pdfPath);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = pdfPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception pdfEx)
                {
                    MessageBox.Show("Thanh toán thành công nhưng có lỗi khi in hóa đơn: " + pdfEx.Message, "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                // Ghi nhận lịch sử chỉnh sửa / hoạt động thực tế
                var currentUser = (Application.Current.MainWindow as MainWindow)?.HoTen ?? "Nhân viên";
                LUnivers_Beaute.Services.LogService.LogEdit(currentUser, "Tạo hóa đơn bán hàng", $"Thanh toán thành công hóa đơn {maHoaDon} trị giá {txtTongCong.Text}", "🛒");

                MessageBox.Show($"Thanh toán thành công!\nMã HĐ: {maHoaDon}\nTổng: {txtTongCong.Text}", "✅ Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                _cart.Clear();
                txtSoDienThoai.Text = "";
                txtTienKhachDua.Text = "";
                txtSearch.Text = "";
                cboKhuyenMai.SelectedIndex = 0;
                cboThanhToan.SelectedIndex = 0;
                LoadProducts(); // Refresh stock
                LoadKhachHang(); // Refresh points
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thanh toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class SanPhamDTO
    {
        public string MaSanPham { get; set; } = "";
        public string TenSanPham { get; set; } = "";
        public string TenThuongHieu { get; set; }
        public decimal GiaNiemYet { get; set; }
        public int SoLuongTon { get; set; }
        public string HinhAnh { get; set; }
    }

    public class CartItem : INotifyPropertyChanged
    {
        public string MaSanPham { get; set; } = "";
        public string TenSanPham { get; set; } = "";
        public decimal DonGia { get; set; }
        public int SoLuongTon { get; set; }

        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set
            {
                _soLuong = value;
                OnPropertyChanged(nameof(SoLuong));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public decimal ThanhTien => DonGia * SoLuong;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
