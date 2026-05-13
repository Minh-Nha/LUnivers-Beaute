using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class DashboardView : UserControl
    {
        private HoaDonBUS _bus = new HoaDonBUS();

        public DashboardView()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) dgData.ItemsSource = _bus.GetDonHangGanDay(5).DefaultView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải Đơn hàng gần đây: " + ex.Message);
            }
        }
    }
}
