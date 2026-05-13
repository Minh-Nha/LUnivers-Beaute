using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LUnivers_Beaute.Views;

namespace LUnivers_Beaute
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Load Dashboard by default
            ContentArea.Content = new DashboardView();
        }

        private void Nav_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string tag)
            {
                ContentArea.Content = tag switch
                {
                    "Dashboard" => new DashboardView(),
                    "BanHang" => new BanHangView(),
                    "SanPham" => new SanPhamView(),
                    "DanhMuc" => new DanhMucView(),
                    "ThuongHieu" => new ThuongHieuView(),
                    "KhoHang" => new KhoHangView(),
                    "NhapKho" => new NhapKhoView(),
                    "KiemKe" => new KiemKeView(),
                    "HuySanPham" => new HuySanPhamView(),
                    "HoaDon" => new HoaDonView(),
                    "TraHang" => new TraHangView(),
                    "KhuyenMai" => new KhuyenMaiView(),
                    "KhachHang" => new KhachHangView(),
                    "NhaCungCap" => new NhaCungCapView(),
                    "NhanVien" => new NhanVienView(),
                    "ChamCong" => new ChamCongView(),
                    "CuaHang" => new CuaHangView(),
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}