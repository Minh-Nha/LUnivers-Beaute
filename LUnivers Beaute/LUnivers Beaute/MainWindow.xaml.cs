using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LUnivers_Beaute.Views;
using LUnivers_Beaute.Services;

namespace LUnivers_Beaute
{
    public partial class MainWindow : Window
    {
        private string _hoTen;
        private string _vaiTro;
        private string _maCuaHang;
        private string _tenCuaHang;
        private string _maNhanVien;

        public string HoTen => _hoTen;
        public string TenCuaHang => _tenCuaHang;
        public string MaCuaHang => _maCuaHang;

        private bool _hasUnreadNotifications = false;
        private readonly Dictionary<string, UserControl> _viewCache = new Dictionary<string, UserControl>();

        private class SearchFeature
        {
            public string DisplayName { get; set; }
            public string Icon { get; set; }
            public string Tag { get; set; }
            public string Category { get; set; }
        }

        private readonly List<SearchFeature> _searchFeatures = new List<SearchFeature>
        {
            new SearchFeature { DisplayName = "Tổng quan (Dashboard)", Icon = "📊", Tag = "Dashboard", Category = "Chung" },
            new SearchFeature { DisplayName = "Quản lý Bán hàng", Icon = "🛒", Tag = "BanHang", Category = "Bán hàng" },
            new SearchFeature { DisplayName = "Danh sách Sản phẩm", Icon = "💄", Tag = "SanPham", Category = "Sản phẩm" },
            new SearchFeature { DisplayName = "Danh mục sản phẩm", Icon = "📂", Tag = "DanhMuc", Category = "Sản phẩm" },
            new SearchFeature { DisplayName = "Thương hiệu sản phẩm", Icon = "🏷️", Tag = "ThuongHieu", Category = "Sản phẩm" },
            new SearchFeature { DisplayName = "Báo cáo Tồn kho", Icon = "📦", Tag = "KhoHang", Category = "Kho hàng" },
            new SearchFeature { DisplayName = "Nhập kho sản phẩm", Icon = "📥", Tag = "NhapKho", Category = "Kho hàng" },
            new SearchFeature { DisplayName = "Kiểm kê kho hàng", Icon = "📋", Tag = "KiemKe", Category = "Kho hàng" },
            new SearchFeature { DisplayName = "Hủy sản phẩm lỗi", Icon = "🗑️", Tag = "HuySanPham", Category = "Kho hàng" },
            new SearchFeature { DisplayName = "Hóa đơn bán hàng", Icon = "🧾", Tag = "HoaDon", Category = "Bán hàng" },
            new SearchFeature { DisplayName = "Hóa đơn Trả hàng", Icon = "↩️", Tag = "TraHang", Category = "Bán hàng" },
            new SearchFeature { DisplayName = "Quản lý Khuyến mãi", Icon = "🎁", Tag = "KhuyenMai", Category = "Bán hàng" },
            new SearchFeature { DisplayName = "Hồ sơ Khách hàng", Icon = "👤", Tag = "KhachHang", Category = "Hệ thống" },
            new SearchFeature { DisplayName = "Nhà cung cấp đối tác", Icon = "🏭", Tag = "NhaCungCap", Category = "Sản phẩm" },
            new SearchFeature { DisplayName = "Danh sách Nhân viên", Icon = "👥", Tag = "NhanVien", Category = "Hệ thống" },
            new SearchFeature { DisplayName = "Bảng Chấm công", Icon = "⏰", Tag = "ChamCong", Category = "Hệ thống" },
            new SearchFeature { DisplayName = "Chi nhánh Cửa hàng", Icon = "🏪", Tag = "CuaHang", Category = "Hệ thống" },
            new SearchFeature { DisplayName = "Nhật ký Lịch sử truy cập", Icon = "⏳", Tag = "LichSuTruyCap", Category = "Hệ thống" },
            new SearchFeature { DisplayName = "Nhật ký Lịch sử chỉnh sửa", Icon = "📝", Tag = "LichSuChinhSua", Category = "Hệ thống" },
            new SearchFeature { DisplayName = "Cài đặt hệ thống", Icon = "⚙", Tag = "CaiDat", Category = "Chung" }
        };

        public MainWindow(string hoTen = "", string vaiTro = "", string maCuaHang = "", string tenCuaHang = "", string maNhanVien = "")
        {
            InitializeComponent();
            _hoTen = hoTen;
            _vaiTro = vaiTro;
            _maCuaHang = maCuaHang;
            _tenCuaHang = tenCuaHang;
            _maNhanVien = maNhanVien;

            if (!string.IsNullOrEmpty(_hoTen))
            {
                txtUserName.Text = _hoTen;
                var parts = _hoTen.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                txtUserAvatar.Text = string.Join("", parts.Select(w => w[0])).ToUpper();
                if (txtUserAvatar.Text.Length > 2) txtUserAvatar.Text = txtUserAvatar.Text.Substring(txtUserAvatar.Text.Length - 2);
            }
            if (!string.IsNullOrEmpty(_vaiTro))
            {
                txtUserRole.Text = _vaiTro;
            }

            // Apply Role-Based Access Control
            ApplyAuthorization();

            // Load Dashboard by default
            SwitchView(GetOrCreateView("Dashboard"));
            LogService.LogAccess(_hoTen, "Chi nhánh: " + _tenCuaHang);
            LogService.LogEdit(_hoTen, "Đăng nhập hệ thống", "Đăng nhập thành công vào chi nhánh " + _tenCuaHang, "🔑");

            // Subscribe to real-time action logs to light up our notification badge
            LogService.OnNewEditLog += LogService_OnNewEditLog;

            // Trigger a seed check/load
            Dispatcher.BeginInvoke(new Action(() => {
                _hasUnreadNotifications = true;
                badgeNotification.Visibility = Visibility.Visible;
            }));
        }

        private void LogService_OnNewEditLog()
        {
            Dispatcher.Invoke(() =>
            {
                _hasUnreadNotifications = true;
                badgeNotification.Visibility = Visibility.Visible;
            });
        }

        private void ApplyAuthorization()
        {
            string role = (_vaiTro ?? "").Trim().ToLower();

            if (role != "admin")
            {
                if (navCaiDat != null)
                {
                    navCaiDat.Visibility = Visibility.Collapsed;
                }
                
                // Hide system-sensitive history panels for standard cashiers if needed
                if (role == "nhân viên bán hàng" || role == "nhân viên")
                {
                    if (navLichSuTruyCap != null) navLichSuTruyCap.Visibility = Visibility.Collapsed;
                    if (navLichSuChinhSua != null) navLichSuChinhSua.Visibility = Visibility.Collapsed;
                }
            }

            if (role == "admin")
            {
                if (btnToggleChatbot != null)
                {
                    btnToggleChatbot.Visibility = Visibility.Visible;
                }
                return;
            }

            if (role == "nhân viên kho")
            {
                // Hide Sales Group
                grpBanHangHeader.Visibility = Visibility.Collapsed;
                navBanHang.Visibility = Visibility.Collapsed;
                navHoaDon.Visibility = Visibility.Collapsed;
                navTraHang.Visibility = Visibility.Collapsed;
                navKhuyenMai.Visibility = Visibility.Collapsed;

                // Hide System Group
                grpHeThongHeader.Visibility = Visibility.Collapsed;
                navKhachHang.Visibility = Visibility.Collapsed;
                navNhanVien.Visibility = Visibility.Collapsed;
                navChamCong.Visibility = Visibility.Collapsed;
                navCuaHang.Visibility = Visibility.Collapsed;
                if (navLichSuTruyCap != null) navLichSuTruyCap.Visibility = Visibility.Collapsed;
                if (navLichSuChinhSua != null) navLichSuChinhSua.Visibility = Visibility.Collapsed;
            }
            else if (role == "nhân viên bán hàng" || role == "nhân viên")
            {
                // Hide Product Group
                grpSanPhamHeader.Visibility = Visibility.Collapsed;
                navSanPham.Visibility = Visibility.Collapsed;
                navDanhMuc.Visibility = Visibility.Collapsed;
                navThuongHieu.Visibility = Visibility.Collapsed;
                navNhaCungCap.Visibility = Visibility.Collapsed;

                // Hide Inventory Group
                grpKhoHangHeader.Visibility = Visibility.Collapsed;
                navKhoHang.Visibility = Visibility.Collapsed;
                navNhapKho.Visibility = Visibility.Collapsed;
                navKiemKe.Visibility = Visibility.Collapsed;
                navHuySanPham.Visibility = Visibility.Collapsed;

                // Hide most of System Group except Customer
                navNhanVien.Visibility = Visibility.Collapsed;
                navChamCong.Visibility = Visibility.Collapsed;
                navCuaHang.Visibility = Visibility.Collapsed;
            }
            else if (role == "quản lý")
            {
                // Quản lý has access to almost everything except Cửa hàng configuration
                navCuaHang.Visibility = Visibility.Collapsed;
            }
        }

        private UserControl GetOrCreateView(string tag)
        {
            if (!_viewCache.TryGetValue(tag, out var view))
            {
                view = tag switch
                {
                    "Dashboard" => new DashboardView(),
                    "BanHang" => new BanHangView(_maNhanVien, _maCuaHang),
                    "SanPham" => new SanPhamView(),
                    "DanhMuc" => new DanhMucView(),
                    "ThuongHieu" => new ThuongHieuView(),
                    "KhoHang" => new TonKhoView(_vaiTro, _maCuaHang),
                    "NhapKho" => new NhapKhoView(_vaiTro, _maCuaHang, _maNhanVien),
                    "KiemKe" => new KiemKeView(_vaiTro, _maCuaHang),
                    "HuySanPham" => new HuySanPhamView(),
                    "HoaDon" => new HoaDonView(),
                    "TraHang" => new TraHangView(),
                    "KhuyenMai" => new KhuyenMaiView(),
                    "KhachHang" => new KhachHangView(_vaiTro),
                    "NhaCungCap" => new NhaCungCapView(),
                    "NhanVien" => new NhanVienView(_vaiTro),
                    "ChamCong" => new ChamCongView(),
                    "CuaHang" => new CuaHangView(),
                    "CaiDat" => new CaiDatView(),
                    "LichSuTruyCap" => new LichSuTruyCapView(),
                    "LichSuChinhSua" => new LichSuChinhSuaView(),
                    _ => new DashboardView()
                };
                _viewCache[tag] = view;
            }
            return view;
        }

        private void Nav_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string tag)
            {
                SwitchView(GetOrCreateView(tag));

                // Track Access Logs
                string screenName = tag switch
                {
                    "Dashboard" => "Tổng quan",
                    "BanHang" => "Bán hàng",
                    "SanPham" => "Sản phẩm",
                    "DanhMuc" => "Danh mục",
                    "ThuongHieu" => "Thương hiệu",
                    "KhoHang" => "Tồn kho",
                    "NhapKho" => "Nhập kho",
                    "KiemKe" => "Kiểm kê",
                    "HuySanPham" => "Hủy sản phẩm",
                    "HoaDon" => "Hóa đơn",
                    "TraHang" => "Trả hàng",
                    "KhuyenMai" => "Khuyến mãi",
                    "KhachHang" => "Khách hàng",
                    "NhaCungCap" => "Nhà cung cấp",
                    "NhanVien" => "Nhân viên",
                    "ChamCong" => "Chấm công",
                    "CuaHang" => "Cửa hàng",
                    "CaiDat" => "Cài đặt",
                    "LichSuTruyCap" => "Lịch sử truy cập",
                    "LichSuChinhSua" => "Lịch sử chỉnh sửa",
                    _ => tag
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

        private void BtnToggleChatbot_Click(object sender, RoutedEventArgs e)
        {
            if (pnlChatbot.Visibility == Visibility.Collapsed)
            {
                pnlChatbot.Visibility = Visibility.Visible;
            }
            else
            {
                pnlChatbot.Visibility = Visibility.Collapsed;
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

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        // Global Search Mechanics
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string term = searchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(term))
            {
                popupSearch.IsOpen = false;
                return;
            }

            var matching = _searchFeatures.Where(f => 
                f.DisplayName.ToLower().Contains(term) || 
                f.Category.ToLower().Contains(term) ||
                f.Tag.ToLower().Contains(term)
            ).ToList();

            if (matching.Count > 0)
            {
                lstSearchResults.ItemsSource = matching.Select(f => new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(4),
                    Children = 
                    {
                        new TextBlock { Text = f.Icon, FontSize = 15, Margin = new Thickness(0,0,10,0), VerticalAlignment = VerticalAlignment.Center },
                        new StackPanel
                        {
                            Children = 
                            {
                                new TextBlock { Text = f.DisplayName, FontSize = 13.5, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F2937")) },
                                new TextBlock { Text = "Nhóm: " + f.Category, FontSize = 11, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")) }
                            }
                        }
                    },
                    Tag = f.Tag
                }).ToList();

                lstSearchResults.Visibility = Visibility.Visible;
                txtNoResults.Visibility = Visibility.Collapsed;
            }
            else
            {
                lstSearchResults.Visibility = Visibility.Collapsed;
                txtNoResults.Visibility = Visibility.Visible;
            }

            popupSearch.IsOpen = true;
        }

        private void LstSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSearchResults.SelectedItem is StackPanel sp && sp.Tag is string tag)
            {
                NavigateToTag(tag);
                popupSearch.IsOpen = false;
                searchBox.Text = "";
            }
            lstSearchResults.SelectedItem = null;
        }

        public void NavigateToTag(string tag)
        {
            var rb = FindVisualChildren<RadioButton>(this).FirstOrDefault(r => r.Tag as string == tag);
            if (rb != null)
            {
                rb.IsChecked = true;
            }
            else
            {
                // Force load direct view if sidebar button is collapsed/hidden by authorization roles
                SwitchView(tag switch
                {
                    "Dashboard" => new DashboardView(),
                    "BanHang" => new BanHangView(_maNhanVien, _maCuaHang),
                    "SanPham" => new SanPhamView(),
                    "DanhMuc" => new DanhMucView(),
                    "ThuongHieu" => new ThuongHieuView(),
                    "KhoHang" => new TonKhoView(_vaiTro, _maCuaHang),
                    "NhapKho" => new NhapKhoView(_vaiTro, _maCuaHang, _maNhanVien),
                    "KiemKe" => new KiemKeView(_vaiTro, _maCuaHang),
                    "HuySanPham" => new HuySanPhamView(),
                    "HoaDon" => new HoaDonView(),
                    "TraHang" => new TraHangView(),
                    "KhuyenMai" => new KhuyenMaiView(),
                    "KhachHang" => new KhachHangView(_vaiTro),
                    "NhaCungCap" => new NhaCungCapView(),
                    "NhanVien" => new NhanVienView(_vaiTro),
                    "ChamCong" => new ChamCongView(),
                    "CuaHang" => new CuaHangView(),
                    "CaiDat" => new CaiDatView(),
                    "LichSuTruyCap" => new LichSuTruyCapView(),
                    "LichSuChinhSua" => new LichSuChinhSuaView(),
                    _ => new DashboardView()
                });
            }
        }

        private void SwitchView(UIElement newView)
        {
            if (ContentArea.Content == null)
            {
                ContentArea.Content = newView;
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.35)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
                var slideIn = new DoubleAnimation(20, 0, TimeSpan.FromSeconds(0.35)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
                var transform = new TranslateTransform();
                newView.RenderTransform = transform;
                newView.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                transform.BeginAnimation(TranslateTransform.YProperty, slideIn);
                return;
            }

            var oldView = ContentArea.Content as UIElement;
            if (oldView != null)
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.18)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn } };
                var slideOut = new DoubleAnimation(0, -15, TimeSpan.FromSeconds(0.18)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn } };
                var transform = oldView.RenderTransform as TranslateTransform ?? new TranslateTransform();
                oldView.RenderTransform = transform;

                fadeOut.Completed += (s, e) =>
                {
                    ContentArea.Content = newView;
                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.32)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
                    var slideIn = new DoubleAnimation(20, 0, TimeSpan.FromSeconds(0.32)) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
                    var newTransform = new TranslateTransform();
                    newView.RenderTransform = newTransform;
                    newView.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    newTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);
                };

                oldView.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                transform.BeginAnimation(TranslateTransform.YProperty, slideOut);
            }
            else
            {
                ContentArea.Content = newView;
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        // Dynamic Bell Notification Dropdown Mechanics
        private void BtnBell_Click(object sender, RoutedEventArgs e)
        {
            LoadBellNotifications();
            popupNotifications.IsOpen = true;
        }

        private void LoadBellNotifications()
        {
            pnlNotificationsList.Children.Clear();
            var editLogs = LogService.GetEditLogs().Take(5).ToList();

            if (editLogs.Count == 0)
            {
                pnlNotificationsList.Children.Add(new TextBlock
                {
                    Text = "Không có hoạt động mới nào.",
                    FontSize = 13,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 16, 0, 16)
                });
                return;
            }

            foreach (var log in editLogs)
            {
                var timeSpan = DateTime.Now - log.Timestamp;
                string timeString = "Vừa xong";
                if (timeSpan.TotalDays >= 1) timeString = $"{(int)timeSpan.TotalDays} ngày trước";
                else if (timeSpan.TotalHours >= 1) timeString = $"{(int)timeSpan.TotalHours} giờ trước";
                else if (timeSpan.TotalMinutes >= 1) timeString = $"{(int)timeSpan.TotalMinutes} phút trước";

                var border = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9FAFB")),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 8),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6"))
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Icon Badge
                var iconBorder = new Border
                {
                    Width = 24,
                    Height = 24,
                    CornerRadius = new CornerRadius(12),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3E8FF")),
                    VerticalAlignment = VerticalAlignment.Top
                };
                iconBorder.Child = new TextBlock
                {
                    Text = log.Icon,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(iconBorder, 0);
                grid.Children.Add(iconBorder);

                // Text detail stack
                var textPanel = new StackPanel { Margin = new Thickness(6, 0, 0, 0) };
                textPanel.Children.Add(new TextBlock
                {
                    Text = log.Action,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12.5,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F2937"))
                });
                textPanel.Children.Add(new TextBlock
                {
                    Text = log.Detail,
                    FontSize = 11.5,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4B5563")),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 2, 0, 2)
                });

                var subPanel = new StackPanel { Orientation = Orientation.Horizontal };
                subPanel.Children.Add(new TextBlock
                {
                    Text = $"👤 {log.User}",
                    FontSize = 10,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6D28D9")),
                    Margin = new Thickness(0, 0, 8, 0)
                });
                subPanel.Children.Add(new TextBlock
                {
                    Text = $"🕒 {timeString}",
                    FontSize = 10,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF"))
                });
                textPanel.Children.Add(subPanel);

                Grid.SetColumn(textPanel, 1);
                grid.Children.Add(textPanel);

                border.Child = grid;
                pnlNotificationsList.Children.Add(border);
            }
        }

        private void BtnMarkRead_Click(object sender, RoutedEventArgs e)
        {
            _hasUnreadNotifications = false;
            badgeNotification.Visibility = Visibility.Collapsed;
        }

        private void BtnViewAllLogs_Click(object sender, RoutedEventArgs e)
        {
            popupNotifications.IsOpen = false;
            NavigateToTag("LichSuChinhSua");
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(searchBox.Text))
            {
                popupSearch.IsOpen = true;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                await System.Threading.Tasks.Task.Delay(250);
                if (!searchBox.IsFocused && !lstSearchResults.IsFocused)
                {
                    popupSearch.IsOpen = false;
                }
            }));
        }
    }
}