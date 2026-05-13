using System.Windows;

namespace LUnivers_Beaute
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (s, e) => DragMove();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Chỉ giao diện - chuyển thẳng sang MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
