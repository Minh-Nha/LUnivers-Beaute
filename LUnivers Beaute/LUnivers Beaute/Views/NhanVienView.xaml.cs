using LUnivers_Beaute.Helpers;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class NhanVienView : UserControl
    {
        private NhanVienBUS _bus = new NhanVienBUS();
        private CuaHangBUS _chBus = new CuaHangBUS();
        private PagingHelper _pager;
        private bool _isEditMode = false;

        private string _currentRole = "";

        public NhanVienView(string currentRole = "")
        {
            InitializeComponent();
            _currentRole = currentRole;
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) =>
            {
                LoadFilters();
                LoadData();
                SetupRoleComboBox();
            };
        }

        private void SetupRoleComboBox()
        {
            try
            {
                if (_currentRole.Trim().ToLower() != "admin" && cboVaiTro != null && cboItemAdmin != null)
                {
                    cboVaiTro.Items.Remove(cboItemAdmin);
                }
            }
            catch { }
        }

        // ===== DATA LOADING =====

        private void LoadFilters()
        {
            try
            {
                // Store filter
                var dtCH = _chBus.GetAll().Copy();
                DataRow rowAll = dtCH.NewRow();
                rowAll["MaCuaHang"] = "";
                rowAll["TenCuaHang"] = "Tất cả cửa hàng";
                dtCH.Rows.InsertAt(rowAll, 0);
                cboFilterCuaHang.ItemsSource = dtCH.DefaultView;
                cboFilterCuaHang.SelectedIndex = 0;

                // Store for form modal
                cboCuaHang.ItemsSource = _chBus.GetAll().DefaultView;
                if (cboCuaHang.Items.Count > 0) cboCuaHang.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadData()
        {
            try
            {
                string? keyword = string.IsNullOrWhiteSpace(txtSearch?.Text) ? null : txtSearch.Text.Trim();
                string? maCuaHang = cboFilterCuaHang?.SelectedValue?.ToString();
                if (string.IsNullOrEmpty(maCuaHang)) maCuaHang = null;

                int? trangThai = GetSelectedTrangThai();

                DataTable dt = _bus.GetAll(keyword, maCuaHang, trangThai);
                if (dgData != null) _pager.SetData(dt);
                UpdateStatistics(dt);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int? GetSelectedTrangThai()
        {
            string selected = (cmbTrangThai?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả trạng thái";
            return selected switch
            {
                "Đang làm" => 1,
                "Nghỉ việc" => 0,
                _ => null
            };
        }

        private void UpdateStatistics(DataTable dt)
        {
            if (dt == null) { txtTongNV.Text = "0"; txtDangLam.Text = "0"; txtNghiViec.Text = "0"; return; }
            txtTongNV.Text = dt.Rows.Count.ToString();
            txtDangLam.Text = dt.AsEnumerable().Count(r => r.Field<string>("TrangThai") == "Đang làm").ToString();
            txtNghiViec.Text = dt.AsEnumerable().Count(r => r.Field<string>("TrangThai") == "Nghỉ việc").ToString();
        }

        // ===== SEARCH =====

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // ===== CRUD =====

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            txtPanelTitle.Text = "✨ Thêm Nhân viên";
            ClearForm();
            pnlMaNV.Visibility = Visibility.Collapsed;
            lblMatKhau.Visibility = Visibility.Visible;
            txtMatKhau.Visibility = Visibility.Visible;
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                _isEditMode = true;
                txtPanelTitle.Text = "✏️ Cập nhật Nhân viên";
                FillForm(row);
                pnlMaNV.Visibility = Visibility.Visible;
                lblMatKhau.Visibility = Visibility.Collapsed;
                txtMatKhau.Visibility = Visibility.Collapsed;
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtHoTen.Text))
                { MessageBox.Show("Vui lòng nhập họ tên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
                { MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
                { MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                if (cboCuaHang.SelectedValue == null)
                { MessageBox.Show("Vui lòng chọn cửa hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

                string hoTen = txtHoTen.Text.Trim();
                string sdt = txtSoDienThoai.Text.Trim();
                string vaiTro = (cboVaiTro.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Nhân viên bán hàng";
                string tenDN = txtTenDangNhap.Text.Trim();
                string maCH = cboCuaHang.SelectedValue?.ToString() ?? "";
                bool trangThai = cboTrangThai.SelectedIndex == 0;

                if (!_isEditMode)
                {
                    // INSERT - mã NV tự động tăng từ SP
                    if (string.IsNullOrEmpty(txtMatKhau.Password))
                    { MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

                    _bus.Insert(hoTen, sdt, vaiTro, tenDN, txtMatKhau.Password, maCH, trangThai);
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // UPDATE
                    string maNV = txtMaNhanVien.Text.Trim();
                    _bus.Update(maNV, hoTen, sdt, vaiTro, tenDN, maCH, trangThai);
                    MessageBox.Show("Cập nhật nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("PRIMARY KEY") || ex.Message.Contains("UNIQUE"))
                {
                    MessageBox.Show("Mã nhân viên hoặc tên đăng nhập đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                string hoTen = row["HoTen"]?.ToString() ?? "";
                string maNV = row["MaNhanVien"]?.ToString() ?? "";
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên \"{hoTen}\" ({maNV})?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(maNV);
                        MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        crudPanel.Visibility = Visibility.Collapsed;
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message.Contains("REFERENCE") || ex.Message.Contains("FK_"))
                        {
                            MessageBox.Show("Không thể xóa vì nhân viên này đã có dữ liệu hóa đơn/phiếu nhập liên quan!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        // ===== HELPERS =====

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => crudPanel.Visibility = Visibility.Collapsed;
        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => crudPanel.Visibility = Visibility.Collapsed;

        private void FillForm(DataRowView row)
        {
            txtMaNhanVien.Text = row["MaNhanVien"]?.ToString() ?? "";
            txtHoTen.Text = row["HoTen"]?.ToString() ?? "";
            txtSoDienThoai.Text = row["SoDienThoai"]?.ToString() ?? "";
            txtTenDangNhap.Text = row["TenDangNhap"]?.ToString() ?? "";
            txtMatKhau.Password = "";

            // Vai trò
            string vaiTro = row["VaiTro"]?.ToString() ?? "Nhân viên";
            for (int i = 0; i < cboVaiTro.Items.Count; i++)
            {
                if ((cboVaiTro.Items[i] as ComboBoxItem)?.Content?.ToString() == vaiTro)
                { cboVaiTro.SelectedIndex = i; break; }
            }

            // Cửa hàng
            string maCH = row["MaCuaHang"]?.ToString() ?? "";
            cboCuaHang.SelectedValue = maCH;

            // Trạng thái
            string tt = row["TrangThai"]?.ToString() ?? "Đang làm";
            cboTrangThai.SelectedIndex = (tt == "Nghỉ việc") ? 1 : 0;
        }

        private void ClearForm()
        {
            txtMaNhanVien.Text = "";
            txtHoTen.Text = "";
            txtSoDienThoai.Text = "";
            txtTenDangNhap.Text = "";
            txtMatKhau.Password = "";
            cboVaiTro.SelectedIndex = 0;
            cboTrangThai.SelectedIndex = 0;
            if (cboCuaHang.Items.Count > 0) cboCuaHang.SelectedIndex = 0;
        }

        // ===== PAGING =====

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e) => _pager?.PreviousPage();
        private void BtnNextPage_Click(object sender, RoutedEventArgs e) => _pager?.NextPage();
    }
}
