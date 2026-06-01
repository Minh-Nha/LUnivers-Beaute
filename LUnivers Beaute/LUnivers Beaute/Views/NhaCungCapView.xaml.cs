using LUnivers_Beaute.Helpers;
using LUnivers_Beaute.Services;
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
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                txtPanelTitle.Text = "✏️ Cập nhật Nhà cung cấp";
                txtMaNhaCungCap.Text = row["MaNhaCungCap"].ToString();
                txtTenNhaCungCap.Text = row["TenNhaCungCap"].ToString();
                txtSoDienThoai.Text = row["SoDienThoai"].ToString();
                txtDiaChi.Text = row["DiaChi"].ToString();
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
                string tenNCC = txtTenNhaCungCap.Text.Trim();
                string sdt = txtSoDienThoai.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();

                if (string.IsNullOrEmpty(tenNCC) || tenNCC.Length < 3)
                {
                    ModernMessageBox.Show("Tên nhà cung cấp phải từ 3 ký tự trở lên!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ValidationHelper.IsPhoneNumber(sdt))
                {
                    ModernMessageBox.Show("Số điện thoại nhà cung cấp không hợp lệ (phải là số di động Việt Nam gồm 10 chữ số, bắt đầu bằng 03, 05, 07, 08, hoặc 09)!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtMaNhaCungCap.Text))
                {
                    _bus.Insert(tenNCC, sdt, diaChi);
                    var currentUser = (Application.Current.MainWindow as MainWindow)?.HoTen ?? "Nhân viên";
                    LogService.LogEdit(currentUser, "Thêm nhà cung cấp", $"Thêm nhà cung cấp '{tenNCC}' thành công", "🏬");
                    ModernMessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    int maNCC = int.Parse(txtMaNhaCungCap.Text);
                    _bus.Update(maNCC, tenNCC, sdt, diaChi);
                    var currentUser = (Application.Current.MainWindow as MainWindow)?.HoTen ?? "Nhân viên";
                    LogService.LogEdit(currentUser, "Cập nhật nhà cung cấp", $"Cập nhật nhà cung cấp thành công (Mã: {maNCC}, Tên mới: '{tenNCC}')", "📝");
                    ModernMessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("UC_TenNCC"))
                {
                    ModernMessageBox.Show("Tên nhà cung cấp này đã tồn tại, vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.Message.Contains("UC_SDT_NCC"))
                {
                    ModernMessageBox.Show("Số điện thoại này đã được sử dụng cho một nhà cung cấp khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    ModernMessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhà cung cấp này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string tenNCC = row["TenNhaCungCap"].ToString();
                        string maNCC = row["MaNhaCungCap"].ToString();
                        _bus.Delete(int.Parse(maNCC));
                        var currentUser = (Application.Current.MainWindow as MainWindow)?.HoTen ?? "Nhân viên";
                        LogService.LogEdit(currentUser, "Xóa nhà cung cấp", $"Xóa nhà cung cấp '{tenNCC}' (Mã: {maNCC}) thành công", "🗑️");
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

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = _bus.GetAll(txtSearch?.Text?.Trim());
                ExcelExportHelper.ExportToExcel(dt, "DanhSachNhaCungCap.csv");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất danh sách nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
