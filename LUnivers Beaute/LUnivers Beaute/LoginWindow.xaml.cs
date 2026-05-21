using System.Windows;
using System.Data;
using BUS;

namespace LUnivers_Beaute
{
    public partial class LoginWindow : Window
    {
        private NhanVienBUS _nhanVienBUS = new NhanVienBUS();

        public LoginWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (s, e) => DragMove();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataTable dt = _nhanVienBUS.Login(username, password);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string maNhanVien = row["MaNhanVien"].ToString();
                string hoTen = row["HoTen"].ToString();
                string vaiTro = row["VaiTro"].ToString();
                string maCuaHang = row["MaCuaHang"].ToString();
                string tenCuaHang = row.Table.Columns.Contains("TenCuaHang") ? row["TenCuaHang"].ToString() : "";

                var mainWindow = new MainWindow(hoTen, vaiTro, maCuaHang, tenCuaHang, maNhanVien);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
