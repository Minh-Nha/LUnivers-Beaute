using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class CuaHangView : UserControl
    {
        private CuaHangBUS _bus = new CuaHangBUS();
        private PagingHelper _pager;

        public CuaHangView()
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
            txtPanelTitle.Text = "✨ Thêm Cửa hàng";
            ClearForm();
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Cửa hàng";
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
                txtPanelTitle.Text = "📋 Chi tiết Cửa hàng";
                txtMaCuaHang.Text = row["MaCuaHang"].ToString();
                txtTenCuaHang.Text = row["TenCuaHang"].ToString();
                txtDiaChi.Text = row["DiaChi"].ToString();
                txtSoDienThoai.Text = row["SoDienThoai"].ToString();
                
                string status = row["TrangThai"].ToString();
                cboTrangThai.SelectedIndex = (status == "Hoạt động") ? 0 : 1;
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void ClearForm()
        {
            txtMaCuaHang.Text = "";
            txtTenCuaHang.Text = "";
            txtDiaChi.Text = "";
            txtSoDienThoai.Text = "";
            cboTrangThai.SelectedIndex = 0;
        }
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ten = txtTenCuaHang.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string sdt = txtSoDienThoai.Text.Trim();
                bool trangThai = cboTrangThai.SelectedIndex == 0;

                if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(diaChi) || string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtMaCuaHang.Text))
                {
                    _bus.Insert(ten, diaChi, sdt, trangThai);
                    MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _bus.Update(txtMaCuaHang.Text, ten, diaChi, sdt, trangThai);
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa cửa hàng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(row["MaCuaHang"].ToString() ?? "");
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
