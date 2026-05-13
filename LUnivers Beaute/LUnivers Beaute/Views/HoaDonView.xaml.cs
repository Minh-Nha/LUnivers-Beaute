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
                if (dgData != null) _pager.SetData(_bus.GetAll());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Do nothing on single click so it doesn't pop up annoyingly
        }

        private void BtnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
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

        private void BtnCloseModal_Click(object sender, RoutedEventArgs e)
        {
            detailPanel.Visibility = Visibility.Collapsed;
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            detailPanel.Visibility = Visibility.Collapsed;
        }
            private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.PreviousPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.NextPage();
        }
    }
}
