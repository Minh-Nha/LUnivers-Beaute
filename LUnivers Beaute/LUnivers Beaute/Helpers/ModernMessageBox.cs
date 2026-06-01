using System.Windows;

namespace LUnivers_Beaute.Helpers
{
    public static class ModernMessageBox
    {
        public static MessageBoxResult Show(string message, string title = "Thông báo", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            if (Application.Current != null)
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    var win = new ModernMessageBoxWindow(message, title, button, icon);
                    
                    // Thử gán Owner cho Window chính để căn giữa tuyệt đối theo cửa sổ chính
                    if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsVisible)
                    {
                        win.Owner = Application.Current.MainWindow;
                    }
                    else
                    {
                        win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }

                    win.ShowDialog();
                    return win.Result;
                });
            }
            return MessageBoxResult.None;
        }
    }
}
