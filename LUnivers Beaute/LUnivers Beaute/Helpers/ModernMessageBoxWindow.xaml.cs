using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LUnivers_Beaute.Helpers
{
    public partial class ModernMessageBoxWindow : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public ModernMessageBoxWindow(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            InitializeComponent();
            txtMessage.Text = message;
            txtTitle.Text = title;

            // Configure Icon and Colors based on MessageBoxImage
            ConfigureIcon(icon);

            // Configure Buttons
            ConfigureButtons(buttons);

            // Drag move support
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    try
                    {
                        this.DragMove();
                    }
                    catch { }
                }
            };
        }

        private void ConfigureIcon(MessageBoxImage icon)
        {
            switch (icon)
            {
                case MessageBoxImage.Error: // Đại diện cho giá trị 16 (Error, Hand, Stop)
                    txtIcon.Text = "❌";
                    iconBorder.Background = new LinearGradientBrush(
                        Color.FromRgb(254, 242, 242), // #FEF2F2
                        Color.FromRgb(254, 226, 226), // #FEE2E2
                        0);
                    break;

                case MessageBoxImage.Warning: // Đại diện cho giá trị 48 (Warning, Exclamation)
                    txtIcon.Text = "⚠️";
                    iconBorder.Background = new LinearGradientBrush(
                        Color.FromRgb(255, 251, 235), // #FFFBEB
                        Color.FromRgb(254, 243, 199), // #FEF3C7
                        0);
                    break;

                case MessageBoxImage.Question: // Đại diện cho giá trị 32
                    txtIcon.Text = "❓";
                    iconBorder.Background = new LinearGradientBrush(
                        Color.FromRgb(245, 243, 255), // #F5F3FF
                        Color.FromRgb(237, 233, 254), // #EDE9FE
                        0);
                    break;

                case MessageBoxImage.Information: // Đại diện cho giá trị 64 (Information, Asterisk)
                default:
                    txtIcon.Text = "✨";
                    iconBorder.Background = new LinearGradientBrush(
                        Color.FromRgb(245, 243, 255), // #F5F3FF
                        Color.FromRgb(237, 233, 254), // #EDE9FE
                        0);
                    break;
            }
        }

        private void ConfigureButtons(MessageBoxButton buttons)
        {
            buttonsPanel.Children.Clear();

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("Đồng ý", MessageBoxResult.OK, true);
                    break;

                case MessageBoxButton.OKCancel:
                    AddButton("Đồng ý", MessageBoxResult.OK, true);
                    AddButton("Hủy bỏ", MessageBoxResult.Cancel, false);
                    break;

                case MessageBoxButton.YesNo:
                    AddButton("Có", MessageBoxResult.Yes, true);
                    AddButton("Không", MessageBoxResult.No, false);
                    break;

                case MessageBoxButton.YesNoCancel:
                    AddButton("Có", MessageBoxResult.Yes, true);
                    AddButton("Không", MessageBoxResult.No, false);
                    AddButton("Hủy bỏ", MessageBoxResult.Cancel, false);
                    break;
            }
        }

        private void AddButton(string text, MessageBoxResult result, bool isPrimary)
        {
            var btn = new Button
            {
                Content = text,
                Margin = new Thickness(8, 0, 0, 0),
                Style = (Style)FindResource(isPrimary ? "DialogBtn" : "DialogBtnSecondary")
            };

            btn.Click += (s, e) =>
            {
                Result = result;
                this.DialogResult = true;
                this.Close();
            };

            buttonsPanel.Children.Add(btn);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            this.DialogResult = false;
            this.Close();
        }
    }
}
