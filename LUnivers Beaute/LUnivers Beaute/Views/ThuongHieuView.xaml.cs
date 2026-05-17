using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class ThuongHieuView : UserControl
    {
        private ThuongHieuBUS _bus = new ThuongHieuBUS();
        private PagingHelper _pager;

        public ThuongHieuView()
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
            txtPanelTitle.Text = "✨ Thêm Thương hiệu";
            ClearForm();
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Thương hiệu";
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
                txtPanelTitle.Text = "📋 Chi tiết Thương hiệu";
                txtMaThuongHieu.Text = row["MaThuongHieu"].ToString();
                txtTenThuongHieu.Text = row["TenThuongHieu"].ToString();
                txtQuocGia.Text = row["QuocGia"].ToString();
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void ClearForm()
        {
            txtMaThuongHieu.Text = "";
            txtTenThuongHieu.Text = "";
            txtQuocGia.Text = "";
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
                if (string.IsNullOrWhiteSpace(txtTenThuongHieu.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên thương hiệu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtMaThuongHieu.Text))
                {
                    _bus.Insert(txtTenThuongHieu.Text.Trim(), txtQuocGia.Text.Trim());
                    MessageBox.Show("Thêm thương hiệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _bus.Update(int.Parse(txtMaThuongHieu.Text), txtTenThuongHieu.Text.Trim(), txtQuocGia.Text.Trim());
                    MessageBox.Show("Cập nhật thương hiệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("UC_TenThuongHieu"))
                {
                    MessageBox.Show("Tên thương hiệu này đã tồn tại, vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa thương hiệu này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(int.Parse(row["MaThuongHieu"].ToString()));
                        MessageBox.Show("Xóa thương hiệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Không thể xóa thương hiệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
