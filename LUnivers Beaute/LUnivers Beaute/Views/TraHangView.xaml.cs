using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class TraHangView : UserControl
    {
        private PhieuTraHangBUS _bus = new PhieuTraHangBUS();
        private PagingHelper _pager;

        public TraHangView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
            if (dgData != null)
            {
                dgData.SelectionChanged += DgData_SelectionChanged;
            }
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
            if (dgData.SelectedItem is DataRowView row)
            {
                string? maPhieuTra = row["MaPhieuTra"].ToString();
                try
                {
                    if (dgChiTiet != null && !string.IsNullOrEmpty(maPhieuTra))
                        dgChiTiet.ItemsSource = _bus.GetChiTiet(maPhieuTra).DefaultView;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
    }
}
