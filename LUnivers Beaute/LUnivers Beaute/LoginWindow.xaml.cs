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

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            // Reset error and status messages
            lblErrorMessage.Text = "";
            lblErrorMessage.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblErrorMessage.Text = "⚠️ Vui lòng nhập tên đăng nhập và mật khẩu!";
                lblErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            // Enter loading state
            txtUsername.IsEnabled = false;
            txtPassword.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnLogin.Content = "ĐANG XÁC THỰC...";
            loginProgressBar.Visibility = Visibility.Visible;
            lblStatusMessage.Text = "🔒 Đang kết nối tới máy chủ...";
            lblStatusMessage.Visibility = Visibility.Visible;

            try
            {
                // Run connection and credentials hash asynchronously to keep UI fluid
                string hashedPassword = LUnivers_Beaute.Helpers.HashHelper.ComputeSha256Hash(password);
                
                // Fetch data in background thread
                DataTable dt = await System.Threading.Tasks.Task.Run(() => _nhanVienBUS.Login(username, hashedPassword));

                if (dt != null && dt.Rows.Count > 0)
                {
                    lblStatusMessage.Text = "🔓 Xác thực thành công! Đang tải hệ thống...";
                    await System.Threading.Tasks.Task.Delay(800); // Premium visual delay so user can see it!

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
                    // Exit loading state and show error
                    txtUsername.IsEnabled = true;
                    txtPassword.IsEnabled = true;
                    btnLogin.IsEnabled = true;
                    btnLogin.Content = "ĐĂNG NHẬP";
                    loginProgressBar.Visibility = Visibility.Collapsed;
                    lblStatusMessage.Visibility = Visibility.Collapsed;

                    lblErrorMessage.Text = "❌ Tên đăng nhập hoặc mật khẩu không đúng!";
                    lblErrorMessage.Visibility = Visibility.Visible;
                }
            }
            catch (System.Exception ex)
            {
                // Handle database / connection errors gracefully
                txtUsername.IsEnabled = true;
                txtPassword.IsEnabled = true;
                btnLogin.IsEnabled = true;
                btnLogin.Content = "ĐĂNG NHẬP";
                loginProgressBar.Visibility = Visibility.Collapsed;
                lblStatusMessage.Visibility = Visibility.Collapsed;

                lblErrorMessage.Text = "⚠️ Lỗi kết nối: " + ex.Message;
                lblErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void TxtForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            var forgotPwWindow = new ForgotPasswordWindow();
            forgotPwWindow.Owner = this;
            forgotPwWindow.ShowDialog();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
