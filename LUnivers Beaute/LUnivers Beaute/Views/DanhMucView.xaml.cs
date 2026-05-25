using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class DanhMucView : UserControl
    {
        private DanhMucBUS _bus = new DanhMucBUS();
        private PagingHelper _pager;

        public DanhMucView()
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
            txtPanelTitle.Text = "✨ Thêm Danh mục";
            ClearForm();
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                txtPanelTitle.Text = "✏️ Cập nhật Danh mục";
                txtMaDanhMuc.Text = row["MaDanhMuc"].ToString();
                txtTenDanhMuc.Text = row["TenDanhMuc"].ToString();
                crudPanel.Visibility = Visibility.Visible;
            }
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
                txtPanelTitle.Text = "📋 Chi tiết Danh mục";
                txtMaDanhMuc.Text = row["MaDanhMuc"].ToString();
                txtTenDanhMuc.Text = row["TenDanhMuc"].ToString();
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void ClearForm()
        {
            txtMaDanhMuc.Text = "";
            txtTenDanhMuc.Text = "";
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
                if (string.IsNullOrWhiteSpace(txtTenDanhMuc.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên danh mục!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtMaDanhMuc.Text))
                {
                    _bus.Insert(txtTenDanhMuc.Text.Trim());
                    MessageBox.Show("Thêm danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _bus.Update(int.Parse(txtMaDanhMuc.Text), txtTenDanhMuc.Text.Trim());
                    MessageBox.Show("Cập nhật danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("UC_TenDanhMuc"))
                {
                    MessageBox.Show("Tên danh mục này đã tồn tại, vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa danh mục này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(int.Parse(row["MaDanhMuc"].ToString()));
                        MessageBox.Show("Xóa danh mục thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Không thể xóa danh mục: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
