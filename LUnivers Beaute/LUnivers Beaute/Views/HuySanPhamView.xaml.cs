using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;
using LUnivers_Beaute.Helpers;

namespace LUnivers_Beaute.Views
{
    public partial class HuySanPhamView : UserControl
    {
        private PagingHelper _pager;
        public HuySanPhamView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                HuySanPhamBUS bus = new HuySanPhamBUS();
                // Load DataGrid
                if (dgData != null) _pager.SetData(bus.GetAllHuySanPham());

                // 3. Load ComboBoxes cho form Thêm
                cboCuaHang.ItemsSource = new CuaHangBUS().GetAll().DefaultView;
                cboCuaHang.DisplayMemberPath = "TenCuaHang";
                cboCuaHang.SelectedValuePath = "MaCuaHang";

                cboNhanVien.ItemsSource = new NhanVienBUS().GetAll().DefaultView;
                cboNhanVien.DisplayMemberPath = "HoTen";
                cboNhanVien.SelectedValuePath = "MaNhanVien";

                // Mock list lô for now since there's no direct GetLoSanXuat()
                DataTable dtLo = new TonKhoBUS().GetAll(); 
                cboLoSanPham.ItemsSource = dtLo.DefaultView;
                cboLoSanPham.DisplayMemberPath = "TenSanPham"; // Using TonKho view which has TenSanPham
                cboLoSanPham.SelectedValuePath = "MaLo";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi tải dữ liệu");
            }
        }
        
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtSoLuong.Clear();
            txtLyDo.Text = "Sản phẩm quá hạn sử dụng";
            cboCuaHang.SelectedIndex = -1;
            cboNhanVien.SelectedIndex = -1;
            cboLoSanPham.SelectedIndex = -1;
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnChoHuy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HuySanPhamBUS bus = new HuySanPhamBUS();
                // Mock CuaHang ID for testing, ideally from login session
                string maCuaHang = "CH01"; 
                dgChoHuy.ItemsSource = bus.GetSanPhamChoHuy(maCuaHang).DefaultView;
                choHuyPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách chờ hủy: " + ex.Message);
            }
        }

        private void BtnDuyetHuy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                // Mở form tạo phiếu hủy và điền sẵn thông tin
                choHuyPanel.Visibility = Visibility.Collapsed;
                
                cboCuaHang.SelectedValue = row["MaCuaHang"];
                cboLoSanPham.SelectedValue = row["MaLo"];
                txtSoLuong.Text = row["SoLuongTon"].ToString();
                txtLyDo.Text = "Duyệt hủy: Sản phẩm đã hết hạn sử dụng";
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void OverlayChoHuy_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            choHuyPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboCuaHang.SelectedValue == null || cboNhanVien.SelectedValue == null || cboLoSanPham.SelectedValue == null || string.IsNullOrWhiteSpace(txtSoLuong.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string maCuaHang = cboCuaHang.SelectedValue.ToString();
                string maNhanVien = cboNhanVien.SelectedValue.ToString();
                int maLo = Convert.ToInt32(cboLoSanPham.SelectedValue);
                int soLuong = Convert.ToInt32(txtSoLuong.Text);
                DateTime ngayHuy = dpNgayHuy.SelectedDate ?? DateTime.Now;
                string lyDo = txtLyDo.Text;

                HuySanPhamBUS bus = new HuySanPhamBUS();
                if (bus.InsertHuySanPham(maCuaHang, maNhanVien, maLo, soLuong, ngayHuy, lyDo))
                {
                    MessageBox.Show("Đã lưu phiếu hủy sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    crudPanel.Visibility = Visibility.Collapsed;
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

