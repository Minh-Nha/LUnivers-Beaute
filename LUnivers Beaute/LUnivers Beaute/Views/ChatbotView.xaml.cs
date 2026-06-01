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

            StackPanel thinkingIndicator = null;
            try
            {
                // Disable input while waiting
                txtMessage.IsEnabled = false;
                
                // Show thinking
                thinkingIndicator = ShowThinkingIndicator();

                string response = await GeminiService.GenerateContentAsync(message);
                
                // Remove thinking
                if (thinkingIndicator != null)
                {
                    spChatArea.Children.Remove(thinkingIndicator);
                }

                AddMessage("AI", response, true);
            }
            catch (Exception ex)
            {
                if (thinkingIndicator != null)
                {
                    spChatArea.Children.Remove(thinkingIndicator);
                }
                AddMessage("System", "Lỗi: " + ex.Message, true);
            }
            finally
            {
                txtMessage.IsEnabled = true;
                txtMessage.Focus();
            }
        }

        private StackPanel ShowThinkingIndicator()
        {
            // We use a Grid as the container so TextWrapping works properly
            Grid messageGrid = new Grid
            {
                Margin = new Thickness(0, 10, 0, 20)
            };
            messageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Avatar
            messageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Content

            Border avatar = new Border
            {
                Width = 40,
                Height = 40,
                CornerRadius = new CornerRadius(20),
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 12, 0),
                Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE8A63")),
                BorderThickness = new Thickness(1)
            };
            avatar.Child = new TextBlock { Text = "✦", Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE8A63")), FontSize = 22, FontFamily = new FontFamily("Georgia"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(avatar, 0);

            StackPanel bubbleContent = new StackPanel { 
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 60, 0)
            };
            Grid.SetColumn(bubbleContent, 1);

            TextBlock nameLabel = new TextBlock
            {
                Text = "Cố Vấn AI",
                FontSize = 13,
                FontWeight = FontWeights.Normal,
                FontFamily = new FontFamily("Georgia"),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080")),
                Margin = new Thickness(4, 0, 0, 6)
            };

            Border bubble = new Border
            {
                CornerRadius = new CornerRadius(4, 12, 12, 12),
                Padding = new Thickness(16, 12, 16, 12),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EAEAEA")),
                BorderThickness = new Thickness(1)
            };

            TextBlock txtContent = new TextBlock
            {
                Text = "Đang suy nghĩ...",
                FontStyle = FontStyles.Italic,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0A0A0")),
                FontSize = 15,
                FontFamily = new FontFamily("Georgia"),
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = txtContent;
            bubbleContent.Children.Add(nameLabel);
            bubbleContent.Children.Add(bubble);

            messageGrid.Children.Add(avatar);
            messageGrid.Children.Add(bubbleContent);

            // We wrap it in a StackPanel so the return signature remains StackPanel (or we can just change return type)
            // But since BtnSend_Click expects a StackPanel to remove, let's wrap it.
            StackPanel wrapper = new StackPanel();
            wrapper.Children.Add(messageGrid);

            spChatArea.Children.Add(wrapper);
            scrollViewer.ScrollToBottom();

            return wrapper;
        }

        private void AddMessage(string senderName, string content, bool isAI)
        {
            Grid messageGrid = new Grid
            {
                Margin = new Thickness(0, 10, 0, 20)
            };

            if (isAI)
            {
                messageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                messageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            else
            {
                messageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                messageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            Border avatar = new Border
            {
                Width = 40,
                Height = 40,
                CornerRadius = new CornerRadius(20),
                VerticalAlignment = VerticalAlignment.Top,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(1)
            };

            if (isAI)
            {
                avatar.Margin = new Thickness(0, 0, 12, 0);
                avatar.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE8A63"));
                avatar.Child = new TextBlock { Text = "✦", Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE8A63")), FontSize = 22, FontFamily = new FontFamily("Georgia"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(avatar, 0);
            }
            else
            {
                avatar.Margin = new Thickness(12, 0, 0, 0);
                avatar.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
                avatar.Child = new TextBlock { Text = "👤", Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")), FontSize = 16, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(avatar, 1);
            }

            StackPanel bubbleContent = new StackPanel
            {
                HorizontalAlignment = isAI ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                Margin = isAI ? new Thickness(0, 0, 60, 0) : new Thickness(60, 0, 0, 0)
            };
            Grid.SetColumn(bubbleContent, isAI ? 1 : 0);

            TextBlock nameLabel = new TextBlock
            {
                Text = isAI ? "Cố Vấn AI" : "Quản Trị Viên",
                FontSize = 13,
                FontWeight = FontWeights.Normal,
                FontFamily = new FontFamily("Georgia"),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080")),
                Margin = new Thickness(4, 0, 0, 6),
                HorizontalAlignment = isAI ? HorizontalAlignment.Left : HorizontalAlignment.Right
            };

            Border bubble = new Border
            {
                CornerRadius = isAI ? new CornerRadius(4, 12, 12, 12) : new CornerRadius(12, 4, 12, 12),
                Padding = new Thickness(16, 12, 16, 12),
                Background = isAI ? Brushes.White 
                                  : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDF9F6")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isAI ? "#EAEAEA" : "#F6E3DA")),
                BorderThickness = new Thickness(1)
            };

            TextBlock txtContent = new TextBlock
            {
                Text = content,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 15,
                LineHeight = 24,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"))
            };

            bubble.Child = txtContent;
            
            bubbleContent.Children.Add(nameLabel);
            bubbleContent.Children.Add(bubble);

            messageGrid.Children.Add(avatar);
            messageGrid.Children.Add(bubbleContent);

            spChatArea.Children.Add(messageGrid);
            scrollViewer.ScrollToBottom();
        }

        private void TxtMessage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = true;
                BtnSend_Click(sender, null);
            }
        }
    }
}
