using System;
using System.Windows;
using System.Windows.Media.Imaging;
using LUnivers_Beaute.Services;

namespace LUnivers_Beaute
{
    public partial class VietQRWindow : Window
    {
        public VietQRWindow(decimal amount, string invoiceId)
        {
            InitializeComponent();
            GenerateQR(amount, invoiceId);
        }

        private void GenerateQR(decimal amount, string invoiceId)
        {
            try
            {
                string url = VietQRService.GenerateQRUrl(amount, invoiceId);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url, UriKind.Absolute);
                bitmap.EndInit();

                imgQR.Source = bitmap;
                txtStatus.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Vui lòng cấu hình"))
                {
                    MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                }
                else
                {
                    txtStatus.Text = "Lỗi tạo QR: " + ex.Message;
                    txtStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                }
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
