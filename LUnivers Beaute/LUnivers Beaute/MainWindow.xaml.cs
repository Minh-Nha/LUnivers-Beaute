using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using LUnivers_Beaute.Views;

namespace LUnivers_Beaute
{
    public partial class MainWindow : Window
    {
        private string _hoTen;
        private string _vaiTro;
        private string _maCuaHang;
        private string _tenCuaHang;
        private string _maNhanVien;

        public MainWindow(string hoTen = "", string vaiTro = "", string maCuaHang = "", string tenCuaHang = "", string maNhanVien = "")
        {
            InitializeComponent();
            _hoTen = hoTen;
            _vaiTro = vaiTro;
            _maCuaHang = maCuaHang;
            _tenCuaHang = tenCuaHang;
            _maNhanVien = maNhanVien;

            if (!string.IsNullOrEmpty(_hoTen))
            {
                txtUserName.Text = _hoTen;
                var parts = _hoTen.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                txtUserAvatar.Text = string.Join("", parts.Select(w => w[0])).ToUpper();
                if (txtUserAvatar.Text.Length > 2) txtUserAvatar.Text = txtUserAvatar.Text.Substring(txtUserAvatar.Text.Length - 2);
            }
            if (!string.IsNullOrEmpty(_vaiTro))
            {
                txtUserRole.Text = _vaiTro;
            }

            // Apply Role-Based Access Control
            ApplyAuthorization();

            // Load Dashboard by default
            ContentArea.Content = new DashboardView();
        }

        private void ApplyAuthorization()
        {
            string role = (_vaiTro ?? "").Trim().ToLower();

            if (role != "admin")
            {
                if (navCaiDat != null)
                {
                    navCaiDat.Visibility = Visibility.Collapsed;
                }
            }

            if (role == "admin")
            {
                // Full access
                return;
            }

            if (role == "nhân viên kho")
            {
                // Hide Sales Group
                grpBanHangHeader.Visibility = Visibility.Collapsed;
                navBanHang.Visibility = Visibility.Collapsed;
                navHoaDon.Visibility = Visibility.Collapsed;
                navTraHang.Visibility = Visibility.Collapsed;
                navKhuyenMai.Visibility = Visibility.Collapsed;

                // Hide System Group
                grpHeThongHeader.Visibility = Visibility.Collapsed;
                navKhachHang.Visibility = Visibility.Collapsed;
                navNhanVien.Visibility = Visibility.Collapsed;
                navChamCong.Visibility = Visibility.Collapsed;
                navCuaHang.Visibility = Visibility.Collapsed;
            }
            else if (role == "nhân viên bán hàng" || role == "nhân viên")
            {
                // Hide Product Group
                grpSanPhamHeader.Visibility = Visibility.Collapsed;
                navSanPham.Visibility = Visibility.Collapsed;
                navDanhMuc.Visibility = Visibility.Collapsed;
                navThuongHieu.Visibility = Visibility.Collapsed;
                navNhaCungCap.Visibility = Visibility.Collapsed;

                // Hide Inventory Group
                grpKhoHangHeader.Visibility = Visibility.Collapsed;
                navKhoHang.Visibility = Visibility.Collapsed;
                navNhapKho.Visibility = Visibility.Collapsed;
                navKiemKe.Visibility = Visibility.Collapsed;
                navHuySanPham.Visibility = Visibility.Collapsed;

                // Hide most of System Group except Customer
                navNhanVien.Visibility = Visibility.Collapsed;
                navChamCong.Visibility = Visibility.Collapsed;
                navCuaHang.Visibility = Visibility.Collapsed;
            }
            else if (role == "quản lý")
            {
                // Quản lý has access to almost everything except Cửa hàng configuration
                navCuaHang.Visibility = Visibility.Collapsed;
            }
        }

        private void Nav_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string tag)
            {
                ContentArea.Content = tag switch
                {
                    "Dashboard" => new DashboardView(),
                    "BanHang" => new BanHangView(_maNhanVien, _maCuaHang),
                    "SanPham" => new SanPhamView(),
                    "DanhMuc" => new DanhMucView(),
                    "ThuongHieu" => new ThuongHieuView(),
                    "KhoHang" => new TonKhoView(_vaiTro, _maCuaHang),
                    "NhapKho" => new NhapKhoView(_vaiTro, _maCuaHang, _maNhanVien),
                    "KiemKe" => new KiemKeView(_vaiTro, _maCuaHang),
                    "HuySanPham" => new HuySanPhamView(),
                    "HoaDon" => new HoaDonView(),
                    "TraHang" => new TraHangView(),
                    "KhuyenMai" => new KhuyenMaiView(),
                    "KhachHang" => new KhachHangView(_vaiTro),
                    "NhaCungCap" => new NhaCungCapView(),
                    "NhanVien" => new NhanVienView(_vaiTro),
                    "ChamCong" => new ChamCongView(),
                    "CuaHang" => new CuaHangView(),
                    "CaiDat" => new CaiDatView(),
                    _ => new DashboardView()
                };
            }
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                BtnMaximize_Click(sender, e);
            }
            else
            {
                DragMove();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MainBorder.CornerRadius = new CornerRadius(16);
            }
            else
            {
                WindowState = WindowState.Maximized;
                MainBorder.CornerRadius = new CornerRadius(0);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}