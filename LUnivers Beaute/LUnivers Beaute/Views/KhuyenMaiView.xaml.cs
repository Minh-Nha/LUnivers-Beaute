using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class KhuyenMaiView : UserControl
    {
        private KhuyenMaiBUS _bus = new KhuyenMaiBUS();
        private PagingHelper _pager;

        public KhuyenMaiView()
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✨ Thêm Khuyến mãi";
            ClearForm();
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Khuyến mãi";
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                txtPanelTitle.Text = "📋 Chi tiết Khuyến mãi";
                txtMaKhuyenMai.Text = row["MaKhuyenMai"].ToString();
                txtTenChuongTrinh.Text = row["TenChuongTrinh"].ToString();
                string mucGiamStr = row["MucGiam"].ToString();
                if (mucGiamStr.EndsWith("%"))
                {
                    cboLoaiGiam.Text = "%";
                    txtMucGiam.Text = mucGiamStr.Replace("%", "").Trim();
                }
                else
                {
                    cboLoaiGiam.Text = "VND";
                    txtMucGiam.Text = mucGiamStr.Replace("₫", "").Replace(".", "").Replace(",", "").Trim();
                }
                
                cboApDung.Text = row["ApDungTheo"].ToString();
                
                string start = row["NgayBatDau"].ToString();
                dpNgayBatDau.Text = start.Length > 10 ? start.Substring(0, 10) : start;
                
                string end = row["NgayKetThuc"].ToString();
                dpNgayKetThuc.Text = end.Length > 10 ? end.Substring(0, 10) : end;
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void ClearForm()
        {
            txtMaKhuyenMai.Text = "";
            txtTenChuongTrinh.Text = "";
            txtMucGiam.Text = "0";
            dpNgayBatDau.Text = "";
            dpNgayKetThuc.Text = "";
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
