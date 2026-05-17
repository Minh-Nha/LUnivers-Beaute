using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;
using System;

namespace LUnivers_Beaute.Views
{
    public partial class SanPhamView : UserControl
    {
        private SanPhamBUS _bus = new SanPhamBUS();
        private PagingHelper _pager;
        private bool _isUpdateMode = false;

        public SanPhamView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => 
            {
                LoadComboBoxes();
                LoadData();
            };
        }

        private void LoadComboBoxes()
        {
            try
            {
                var dm = new DanhMucBUS().GetAll();
                var th = new ThuongHieuBUS().GetAll();
                var ncc = new NhaCungCapBUS().GetAll();

                cboDanhMuc.ItemsSource = dm.DefaultView;
                cboDanhMuc.SelectedValuePath = "MaDanhMuc";

                cboThuongHieu.ItemsSource = th.DefaultView;
                cboThuongHieu.SelectedValuePath = "MaThuongHieu";

                cboNhaCungCap.ItemsSource = ncc.DefaultView;
                cboNhaCungCap.SelectedValuePath = "MaNhaCungCap";

                // Filters
                DataTable dmFilter = dm.Copy();
                DataRow drDm = dmFilter.NewRow();
                drDm["MaDanhMuc"] = 0;
                drDm["TenDanhMuc"] = "Tất cả danh mục";
                dmFilter.Rows.InsertAt(drDm, 0);
                cboFilterDanhMuc.ItemsSource = dmFilter.DefaultView;
                cboFilterDanhMuc.SelectedValuePath = "MaDanhMuc";
                cboFilterDanhMuc.SelectedIndex = 0;

                DataTable thFilter = th.Copy();
                DataRow drTh = thFilter.NewRow();
                drTh["MaThuongHieu"] = 0;
                drTh["TenThuongHieu"] = "Tất cả thương hiệu";
                thFilter.Rows.InsertAt(drTh, 0);
                cboFilterThuongHieu.ItemsSource = thFilter.DefaultView;
                cboFilterThuongHieu.SelectedValuePath = "MaThuongHieu";
                cboFilterThuongHieu.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu danh mục: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                string timKiem = txtSearch.Text.Trim();
                int? maDanhMuc = null;
                if (cboFilterDanhMuc.SelectedValue != null && cboFilterDanhMuc.SelectedIndex > 0)
                {
                    maDanhMuc = Convert.ToInt32(cboFilterDanhMuc.SelectedValue);
                }

                int? maThuongHieu = null;
                if (cboFilterThuongHieu.SelectedValue != null && cboFilterThuongHieu.SelectedIndex > 0)
                {
                    maThuongHieu = Convert.ToInt32(cboFilterThuongHieu.SelectedValue);
                }

                int? trangThai = null;
                if (cboFilterTrangThai.SelectedItem is ComboBoxItem cbi && cbi.Tag != null)
                {
                    trangThai = Convert.ToInt32(cbi.Tag);
                }

                if (dgData != null) 
                    _pager.SetData(_bus.GetAll(timKiem, maDanhMuc, maThuongHieu, trangThai));
            }
            catch (Exception ex)
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
            txtPanelTitle.Text = "✨ Thêm Sản phẩm";
            _isUpdateMode = false;
            txtMaSanPham.IsReadOnly = false;
            txtMaSanPham.Background = System.Windows.Media.Brushes.White;
            ClearForm();
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.DataContext is DataRowView row)
            {
                PopulateForm(row);
            }
        }

        private void PopulateForm(DataRowView row)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Sản phẩm";
            _isUpdateMode = true;
            txtMaSanPham.IsReadOnly = true;
            txtMaSanPham.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#F8F9FA");

            txtMaSanPham.Text = row["MaSanPham"].ToString();
            txtTenSanPham.Text = row["TenSanPham"].ToString();
            txtDonViTinh.Text = row["DonViTinh"].ToString();
            txtGiaNiemYet.Text = row["GiaNiemYet"].ToString();
            
            cboDanhMuc.SelectedValue = row["MaDanhMuc"] != DBNull.Value ? row["MaDanhMuc"] : null;
            cboThuongHieu.SelectedValue = row["MaThuongHieu"] != DBNull.Value ? row["MaThuongHieu"] : null;
            cboNhaCungCap.SelectedValue = row["MaNhaCungCap"] != DBNull.Value ? row["MaNhaCungCap"] : null;
            
            if (row["TrangThai"] != DBNull.Value)
            {
                bool trangThai = Convert.ToBoolean(row["TrangThai"]);
                cboTrangThai.SelectedIndex = trangThai ? 0 : 1;
            }
            else
            {
                cboTrangThai.SelectedIndex = 0;
            }
            
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Keeping event signature just in case
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            txtMaSanPham.Text = "";
            txtTenSanPham.Text = "";
            txtDonViTinh.Text = "Cái";
            txtGiaNiemYet.Text = "";
            txtMoTa.Text = "";
            
            cboDanhMuc.SelectedIndex = -1;
            cboThuongHieu.SelectedIndex = -1;
            cboNhaCungCap.SelectedIndex = -1;
            cboTrangThai.SelectedIndex = 0;
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
                if (string.IsNullOrWhiteSpace(txtMaSanPham.Text) || string.IsNullOrWhiteSpace(txtTenSanPham.Text) || string.IsNullOrWhiteSpace(txtGiaNiemYet.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Mã sản phẩm, Tên sản phẩm và Giá niêm yết!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cboDanhMuc.SelectedValue == null || cboThuongHieu.SelectedValue == null || cboNhaCungCap.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Danh mục, Thương hiệu và Nhà cung cấp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtGiaNiemYet.Text, out decimal giaNiemYet) || giaNiemYet <= 0)
                {
                    MessageBox.Show("Giá niêm yết không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int maDanhMuc = Convert.ToInt32(cboDanhMuc.SelectedValue);
                int maThuongHieu = Convert.ToInt32(cboThuongHieu.SelectedValue);
                int maNhaCungCap = Convert.ToInt32(cboNhaCungCap.SelectedValue);
                bool trangThai = cboTrangThai.SelectedIndex == 0;
                string donVi = string.IsNullOrWhiteSpace(txtDonViTinh.Text) ? "Cái" : txtDonViTinh.Text.Trim();

                if (!_isUpdateMode)
                {
                    _bus.Insert(txtMaSanPham.Text.Trim(), txtTenSanPham.Text.Trim(), maDanhMuc, maThuongHieu, maNhaCungCap, null, donVi, giaNiemYet, trangThai);
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _bus.Update(txtMaSanPham.Text.Trim(), txtTenSanPham.Text.Trim(), maDanhMuc, maThuongHieu, maNhaCungCap, null, donVi, giaNiemYet, trangThai);
                    MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UC_TenSanPham"))
                {
                    MessageBox.Show("Tên sản phẩm đã tồn tại, vui lòng chọn tên khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.Message.Contains("PRIMARY KEY"))
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại, vui lòng chọn mã khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.DataContext is DataRowView row)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(row["MaSanPham"].ToString());
                        MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
