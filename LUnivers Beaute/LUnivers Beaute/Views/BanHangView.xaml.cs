using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS;
using System.ComponentModel;

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
                LoadNhanVien();
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
                    runCuaHang.Text = dt.Rows[0]["TenCuaHang"]?.ToString() ?? _currentCuaHang;
                }
            }
            catch { }
        }

        private void LoadNhanVien()
        {
            try
            {
                var dt = _nhanVienBus.GetAll();

                if (!dt.Columns.Contains("DisplayText"))
                    dt.Columns.Add("DisplayText", typeof(string));

                foreach (DataRow r in dt.Rows)
                {
                    r["DisplayText"] = r["MaNhanVien"]?.ToString() + " - " + r["HoTen"]?.ToString();
                }

                if (!string.IsNullOrEmpty(_currentCuaHang))
                {
                    var rows = dt.Select($"MaCuaHang = '{_currentCuaHang}'");
                    if (rows.Length > 0)
                        dt = rows.CopyToDataTable();
                    else
                        dt.Rows.Clear();
                }

                cboNhanVien.ItemsSource = dt.DefaultView;
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(_currentNhanVien))
                    {
                        var rowView = dt.DefaultView.Cast<DataRowView>().FirstOrDefault(r => r["MaNhanVien"].ToString() == _currentNhanVien);
                        if (rowView != null)
                            cboNhanVien.SelectedItem = rowView;
                        else
                            cboNhanVien.SelectedIndex = 0;
                    }
                    else
                    {
                        cboNhanVien.SelectedIndex = 0;
                        _currentNhanVien = dt.Rows[0]["MaNhanVien"]?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadNhanVien error: " + ex.Message);
            }
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
                var dt = _khachHangBus.GetAll(null, 1); // Only active customers

                // Add DisplayText column for ComboBox
                if (!dt.Columns.Contains("DisplayText"))
                    dt.Columns.Add("DisplayText", typeof(string));

                foreach (DataRow r in dt.Rows)
                {
                    string sdt = r["SoDienThoai"]?.ToString() ?? "";
                    r["DisplayText"] = r["HoTen"]?.ToString() + (string.IsNullOrEmpty(sdt) ? "" : " - " + sdt);
                }

                DataRow dr = dt.NewRow();
                dr["MaKhachHang"] = 0;
                dr["HoTen"] = "Khách vãng lai";
                dr["SoDienThoai"] = "";
                dr["DiemTichLuy"] = 0;
                dr["DisplayText"] = "Khách vãng lai";
                dt.Rows.InsertAt(dr, 0);

                cboKhachHang.ItemsSource = dt.DefaultView;
                cboKhachHang.SelectedIndex = 0;
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

                var dt = _hoaDonBus.GetSanPhamBanHang(_currentCuaHang, searchTerm, maDanhMuc);
                _products.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    _products.Add(new SanPhamDTO
                    {
                        MaSanPham = row["MaSanPham"]?.ToString() ?? "",
                        TenSanPham = row["TenSanPham"]?.ToString() ?? "",
                        TenThuongHieu = row["TenThuongHieu"]?.ToString() ?? "",
                        GiaNiemYet = Convert.ToDecimal(row["GiaNiemYet"]),
                        SoLuongTon = Convert.ToInt32(row["SoLuongTon"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tải sản phẩm", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== EVENT HANDLERS =====

        private void CboNhanVien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboNhanVien.SelectedItem is DataRowView row)
            {
                _currentNhanVien = row["MaNhanVien"]?.ToString() ?? "";
            }
        }

        private void CboCuaHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboCuaHang.SelectedItem is DataRowView row)
            {
                _currentCuaHang = row["MaCuaHang"]?.ToString() ?? "CH01";
                runCuaHang.Text = row["TenCuaHang"]?.ToString() ?? _currentCuaHang;
                _cart.Clear();
                LoadProducts();
            }
        }

        private void CboDanhMuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadProducts();
        }

        private void CboKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCartTotal();
        }

        private void BtnClearKhachHang_Click(object sender, RoutedEventArgs e)
        {
            cboKhachHang.SelectedIndex = 0; // Reset to "Khách vãng lai"
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
            if (cboKhachHang?.SelectedItem is DataRowView row)
            {
                int diem = 0;
                int.TryParse(row["DiemTichLuy"]?.ToString(), out diem);
                int maKH = 0;
                int.TryParse(row["MaKhachHang"]?.ToString(), out maKH);
                txtDiemTichLuy.Text = maKH > 0 ? $"⭐ {diem:N0}" : "⭐ 0";
            }

            btnThanhToan.IsEnabled = _cart.Count > 0;
        }

        // ===== CHECKOUT =====

        private void BtnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0) return;

            try
            {
                string maHoaDon = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string phuongThuc = (cboThanhToan.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tiền mặt";

                // Get selected customer
                int? maKhachHang = null;
                if (cboKhachHang.SelectedItem is DataRowView khRow)
                {
                    int maKH = 0;
                    int.TryParse(khRow["MaKhachHang"]?.ToString(), out maKH);
                    if (maKH > 0) maKhachHang = maKH;
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

                _hoaDonBus.TaoHoaDon(maHoaDon, _currentCuaHang, _currentNhanVien, maKhachHang, maKhuyenMai, phuongThuc, json);

                MessageBox.Show($"Thanh toán thành công!\nMã HĐ: {maHoaDon}\nTổng: {txtTongCong.Text}", "✅ Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                _cart.Clear();
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
        public string TenThuongHieu { get; set; } = "";
        public decimal GiaNiemYet { get; set; }
        public int SoLuongTon { get; set; }
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
