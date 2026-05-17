using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class NhaCungCapView : UserControl
    {
        private NhaCungCapBUS _bus = new NhaCungCapBUS();
        private PagingHelper _pager;

        public NhaCungCapView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) _pager.SetData(_bus.GetAll(txtSearch?.Text?.Trim()));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✨ Thêm Nhà cung cấp";
            ClearForm();
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Nhà cung cấp";
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
                txtPanelTitle.Text = "📋 Chi tiết Nhà cung cấp";
                txtMaNhaCungCap.Text = row["MaNhaCungCap"].ToString();
                txtTenNhaCungCap.Text = row["TenNhaCungCap"].ToString();
                txtSoDienThoai.Text = row["SoDienThoai"].ToString();
                txtDiaChi.Text = row["DiaChi"].ToString();
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void ClearForm()
        {
            txtMaNhaCungCap.Text = "";
            txtTenNhaCungCap.Text = "";
            txtSoDienThoai.Text = "";
            txtDiaChi.Text = "";
        }
        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.PreviousPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.NextPage();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTenNhaCungCap.Text) || string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên nhà cung cấp và số điện thoại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtMaNhaCungCap.Text))
                {
                    _bus.Insert(txtTenNhaCungCap.Text.Trim(), txtSoDienThoai.Text.Trim(), txtDiaChi.Text.Trim());
                    MessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _bus.Update(int.Parse(txtMaNhaCungCap.Text), txtTenNhaCungCap.Text.Trim(), txtSoDienThoai.Text.Trim(), txtDiaChi.Text.Trim());
                    MessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("UC_TenNCC"))
                {
                    MessageBox.Show("Tên nhà cung cấp này đã tồn tại, vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.Message.Contains("UC_SDT_NCC"))
                {
                    MessageBox.Show("Số điện thoại này đã được sử dụng cho một nhà cung cấp khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhà cung cấp này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(int.Parse(row["MaNhaCungCap"].ToString()));
                        MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Không thể xóa nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
