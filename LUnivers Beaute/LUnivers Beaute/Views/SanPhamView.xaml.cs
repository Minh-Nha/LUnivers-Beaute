using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class SanPhamView : UserControl
    {
        private SanPhamBUS _bus = new SanPhamBUS();
        private PagingHelper _pager;

        public SanPhamView()
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
            txtPanelTitle.Text = "✨ Thêm Sản phẩm";
            ClearForm();
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Sản phẩm";
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                txtPanelTitle.Text = "📋 Chi tiết Sản phẩm";
                txtMaSanPham.Text = row["MaSanPham"].ToString();
                txtTenSanPham.Text = row["TenSanPham"].ToString();
                txtDonViTinh.Text = row["DonViTinh"].ToString();
                txtGiaNiemYet.Text = row["GiaNiemYet"].ToString();
                
                cboDanhMuc.Text = row["TenDanhMuc"].ToString();
                cboThuongHieu.Text = row["TenThuongHieu"].ToString();
                cboNhaCungCap.Text = row["TenNhaCungCap"].ToString();
                cboTrangThai.Text = row["TrangThai"].ToString();
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            txtMaSanPham.Text = "";
            txtTenSanPham.Text = "";
            txtDonViTinh.Text = "";
            txtGiaNiemYet.Text = "";
            txtMoTa.Text = "";
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
