using System;
using System.Data;
using System.Windows;
using BUS;
using LUnivers_Beaute.Helpers;

namespace LUnivers_Beaute
{
    public partial class ForgotPasswordWindow : Window
    {
        private NhanVienBUS _nhanVienBUS = new NhanVienBUS();
        private string _generatedOtp = "";
        private string _confirmedEmail = "";

        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnSendOtp_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataTable dt = _nhanVienBUS.CheckEmailExists(email);
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Email này chưa được đăng ký trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Sinh OTP 6 số
            Random rnd = new Random();
            _generatedOtp = rnd.Next(100000, 999999).ToString();
            _confirmedEmail = email;

            try
            {
                EmailHelper.SendOtpEmail(email, _generatedOtp);
                MessageBox.Show("Đã gửi mã OTP đến email của bạn. Vui lòng kiểm tra hộp thư!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                
                pnlStep1.Visibility = Visibility.Collapsed;
                pnlStep2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể gửi email lúc này. Bạn đã cấu hình Gmail App Password chưa?\nChi tiết: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVerifyOtp_Click(object sender, RoutedEventArgs e)
        {
            if (txtOtp.Text.Trim() == _generatedOtp)
            {
                pnlStep2.Visibility = Visibility.Collapsed;
                pnlStep3.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Mã OTP không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            string pw1 = txtNewPassword.Password;
            string pw2 = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(pw1))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pw1 != pw2)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string hashedPassword = HashHelper.ComputeSha256Hash(pw1);
            _nhanVienBUS.UpdatePasswordByEmail(_confirmedEmail, hashedPassword);
            
            MessageBox.Show("Đổi mật khẩu thành công! Bạn có thể đăng nhập bằng mật khẩu mới.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
