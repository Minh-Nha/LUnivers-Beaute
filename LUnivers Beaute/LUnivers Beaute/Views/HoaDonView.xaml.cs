using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class HoaDonView : UserControl
    {
        private HoaDonBUS _bus = new HoaDonBUS();
        private PagingHelper _pager;

        public HoaDonView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) 
                {
                    _pager.SetData(_bus.GetAll());
                    if (dgData.Items.Count > 0)
                    {
                        dgData.SelectedIndex = 0;
                    }
                    else
                    {
                        detailPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                string maHoaDon = row["MaHoaDon"].ToString();
                
                // Populate Invoice Info
                lblMaHD.Text = maHoaDon;
                lblNgayLap.Text = row["NgayLap"].ToString();
                lblNhanVien.Text = row["NhanVienLap"].ToString();
                lblKhachHang.Text = row["KhachHang"].ToString();
                lblThanhToan.Text = row["PhuongThucThanhToan"].ToString();
                lblCuaHang.Text = row["TenCuaHang"].ToString();
                
                // Populate summary
                double tongTien = 0, khachTra = 0;
                double.TryParse(row["TongTien"].ToString(), out tongTien);
                double.TryParse(row["KhachCanTra"].ToString(), out khachTra);
                
                lblTongTien.Text = tongTien.ToString("#,##0") + " đ";
                lblKhuyenMai.Text = (tongTien - khachTra).ToString("#,##0") + " đ";
                lblCanTra.Text = khachTra.ToString("#,##0") + " đ";
                
                try
                {
                    if (dgChiTiet != null)
                        dgChiTiet.ItemsSource = _bus.GetChiTiet(maHoaDon).DefaultView;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                detailPanel.Visibility = Visibility.Visible;
            }
        }


            private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.PreviousPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.NextPage();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lblMaHD.Text) || dgChiTiet.ItemsSource == null) 
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để in!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PDF Document (*.pdf)|*.pdf";
            saveFileDialog.FileName = lblMaHD.Text;
            saveFileDialog.Title = "Lưu Hóa Đơn PDF";

            if (saveFileDialog.ShowDialog() == true)
            {
                System.Text.StringBuilder itemsXaml = new System.Text.StringBuilder();
                foreach (DataRowView row in (DataView)dgChiTiet.ItemsSource)
                {
                    string ten = row["TenSanPham"].ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                    string sl = row["SoLuong"].ToString();
                    string tt = row["ThanhTien"].ToString();
                    
                    itemsXaml.Append($@"
                    <Grid Margin=""0,0,0,8"">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width=""*""/>
                            <ColumnDefinition Width=""40""/>
                            <ColumnDefinition Width=""80""/>
                        </Grid.ColumnDefinitions>
                        <TextBlock Text=""{ten}"" FontSize=""12"" TextWrapping=""Wrap""/>
                        <TextBlock Grid.Column=""1"" Text=""{sl}"" FontSize=""12"" TextAlignment=""Center""/>
                        <TextBlock Grid.Column=""2"" Text=""{tt}"" FontSize=""12"" FontWeight=""SemiBold"" TextAlignment=""Right""/>
                    </Grid>");
                }

                string xaml = $@"
                <Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" Width=""400"" Background=""White"" Margin=""20"">
                    <StackPanel>
                        <TextBlock Text=""L'UNIVERS BEAUTÉ"" FontSize=""24"" FontWeight=""Bold"" HorizontalAlignment=""Center"" Margin=""0,0,0,5""/>
                        <TextBlock Text=""Số 1, Đại Cồ Việt, Hai Bà Trưng, Hà Nội"" FontSize=""12"" HorizontalAlignment=""Center""/>
                        <TextBlock Text=""Hotline: 1900 9999"" FontSize=""12"" HorizontalAlignment=""Center"" Margin=""0,0,0,20""/>
                        
                        <TextBlock Text=""HÓA ĐƠN BÁN HÀNG"" FontSize=""20"" FontWeight=""Bold"" HorizontalAlignment=""Center"" Margin=""0,0,0,20""/>
                        
                        <Grid Margin=""0,0,0,5"">
                            <TextBlock Text=""Mã HĐ: {lblMaHD.Text}"" FontSize=""13""/>
                            <TextBlock Text=""Ngày: {lblNgayLap.Text}"" FontSize=""13"" HorizontalAlignment=""Right""/>
                        </Grid>
                        <Grid Margin=""0,0,0,5"">
                            <TextBlock Text=""Thu ngân: {lblNhanVien.Text}"" FontSize=""13""/>
                            <TextBlock Text=""Khách: {lblKhachHang.Text}"" FontSize=""13"" HorizontalAlignment=""Right""/>
                        </Grid>
                        <Grid Margin=""0,0,0,5"">
                            <TextBlock Text=""Phương thức: {lblThanhToan.Text}"" FontSize=""13""/>
                        </Grid>
                        
                        <Line X1=""0"" Y1=""0"" X2=""360"" Y2=""0"" Stroke=""#2D2D2D"" StrokeThickness=""1"" StrokeDashArray=""4 4"" Margin=""0,15""/>
                        
                        <Grid Margin=""0,0,0,10"">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width=""*""/>
                                <ColumnDefinition Width=""40""/>
                                <ColumnDefinition Width=""80""/>
                            </Grid.ColumnDefinitions>
                            <TextBlock Text=""SẢN PHẨM"" FontSize=""12"" FontWeight=""Bold"" Foreground=""#7A7A7A""/>
                            <TextBlock Grid.Column=""1"" Text=""SL"" FontSize=""12"" FontWeight=""Bold"" TextAlignment=""Center"" Foreground=""#7A7A7A""/>
                            <TextBlock Grid.Column=""2"" Text=""THÀNH TIỀN"" FontSize=""12"" FontWeight=""Bold"" TextAlignment=""Right"" Foreground=""#7A7A7A""/>
                        </Grid>
                        
                        {itemsXaml.ToString()}
                        
                        <Line X1=""0"" Y1=""0"" X2=""360"" Y2=""0"" Stroke=""#2D2D2D"" StrokeThickness=""1"" StrokeDashArray=""4 4"" Margin=""0,10,0,15""/>
                        
                        <Grid Margin=""0,0,0,6"">
                            <TextBlock Text=""Tổng tiền:"" FontSize=""13"" Foreground=""#7A7A7A""/>
                            <TextBlock Text=""{lblTongTien.Text}"" FontSize=""13"" HorizontalAlignment=""Right""/>
                        </Grid>
                        <Grid Margin=""0,0,0,12"">
                            <TextBlock Text=""Khuyến mãi:"" FontSize=""13"" Foreground=""#7A7A7A""/>
                            <TextBlock Text=""{lblKhuyenMai.Text}"" FontSize=""13"" HorizontalAlignment=""Right""/>
                        </Grid>
                        
                        <Grid Margin=""0,0,0,20"">
                            <TextBlock Text=""KHÁCH TRẢ:"" FontSize=""16"" FontWeight=""Bold""/>
                            <TextBlock Text=""{lblCanTra.Text}"" FontSize=""16"" FontWeight=""Bold"" HorizontalAlignment=""Right""/>
                        </Grid>
                        
                        <TextBlock Text=""Cảm ơn quý khách và hẹn gặp lại!"" FontSize=""13"" FontStyle=""Italic"" HorizontalAlignment=""Center"" Margin=""0,10,0,0""/>
                    </StackPanel>
                </Grid>";

                try 
                {
                    Grid receiptGrid = (Grid)System.Windows.Markup.XamlReader.Parse(xaml);
                    
                    // Render WPF Visual to Bitmap
                    receiptGrid.Measure(new Size(440, double.PositiveInfinity));
                    receiptGrid.Arrange(new Rect(0, 0, receiptGrid.DesiredSize.Width, receiptGrid.DesiredSize.Height));
                    receiptGrid.UpdateLayout();

                    System.Windows.Media.Imaging.RenderTargetBitmap rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(
                        (int)receiptGrid.DesiredSize.Width, (int)receiptGrid.DesiredSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Pbgra32);
                    rtb.Render(receiptGrid);

                    System.Windows.Media.Imaging.PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        encoder.Save(ms);
                        ms.Position = 0;

                        // Create PDF and draw image
                        using (PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument())
                        {
                            PdfSharp.Pdf.PdfPage page = document.AddPage();
                            page.Width = PdfSharp.Drawing.XUnit.FromPresentation(receiptGrid.DesiredSize.Width);
                            page.Height = PdfSharp.Drawing.XUnit.FromPresentation(receiptGrid.DesiredSize.Height);
                            
                            using (PdfSharp.Drawing.XGraphics gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page))
                            {
                                byte[] bytes = ms.ToArray();
                                using (System.IO.MemoryStream msImage = new System.IO.MemoryStream(bytes, 0, bytes.Length, true, true))
                                using (PdfSharp.Drawing.XImage xImage = PdfSharp.Drawing.XImage.FromStream(msImage))
                                {
                                    gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                                }
                            }
                            
                            document.Save(saveFileDialog.FileName);
                        }
                    }

                    MessageBox.Show("Đã lưu hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Mở file PDF tự động
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo PDF: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
