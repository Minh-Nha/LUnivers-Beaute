using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LUnivers_Beaute.Services;

namespace LUnivers_Beaute.Views
{
    public partial class ChatbotView : UserControl
    {
        public ChatbotView()
        {
            InitializeComponent();
            AddMessage("AI", "Xin chào Admin! Tôi là trợ lý AI Gemini. Tôi có thể giúp gì cho bạn hôm nay?", true);
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            txtMessage.Text = "";
            AddMessage("You", message, false);

            try
            {
                // Disable input while waiting
                txtMessage.IsEnabled = false;
                
                string response = await GeminiService.GenerateContentAsync(message);
                AddMessage("AI", response, true);
            }
            catch (Exception ex)
            {
                AddMessage("System", "Lỗi: " + ex.Message, true);
            }
            finally
            {
                txtMessage.IsEnabled = true;
                txtMessage.Focus();
            }
        }

        private void AddMessage(string senderName, string content, bool isAI)
        {
            Border border = new Border
            {
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 8, 0, 8),
                MaxWidth = 800,
                HorizontalAlignment = isAI ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                Background = isAI ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F4F8")) 
                                  : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8956D"))
            };

            TextBlock txtContent = new TextBlock
            {
                Text = content,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Foreground = isAI ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White)
            };

            border.Child = txtContent;
            spChatArea.Children.Add(border);

            // Auto scroll to bottom
            scrollViewer.ScrollToBottom();
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = true;
                BtnSend_Click(sender, null);
            }
        }
    }
}
