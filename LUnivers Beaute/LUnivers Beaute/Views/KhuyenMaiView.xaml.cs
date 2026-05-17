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
        private DanhMucBUS _dmBus = new DanhMucBUS();
        private SanPhamBUS _spBus = new SanPhamBUS();
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
                
                var dmData = _dmBus.GetAll();
                cboDanhMuc.ItemsSource = dmData.DefaultView;
                cboSanPham.ItemsSource = _spBus.GetAll().DefaultView;

                // Add "Tất cả danh mục" for search combobox
                var dtSearchDm = dmData.Copy();
                var row = dtSearchDm.NewRow();
                row["MaDanhMuc"] = 0;
                row["TenDanhMuc"] = "Tất cả danh mục";
                dtSearchDm.Rows.InsertAt(row, 0);
                cboSearchDanhMuc.ItemsSource = dtSearchDm.DefaultView;
                cboSearchDanhMuc.SelectedIndex = 0;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = txtSearch.Text.Trim();
            int? maDanhMuc = cboSearchDanhMuc.SelectedValue as int?;
            if (maDanhMuc == 0) maDanhMuc = null;

            string loaiGiam = null;
            if (cboSearchLoaiGiam.SelectedIndex == 1) loaiGiam = "%";
            else if (cboSearchLoaiGiam.SelectedIndex == 2) loaiGiam = "VND";

            System.DateTime? tuNgay = dpSearchTuNgay.SelectedDate;
            System.DateTime? denNgay = dpSearchDenNgay.SelectedDate;

            int trangThaiLoc = cboSearchTrangThai.SelectedIndex;

            _pager.SetData(_bus.GetAll(tuKhoa, maDanhMuc, loaiGiam, tuNgay, denNgay, trangThaiLoc));
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
                if (row.Row.Table.Columns.Contains("GiaTriGiam")) { txtMucGiam.Text = Convert.ToDouble(row["GiaTriGiam"]).ToString("G"); } 
                if (row.Row.Table.Columns.Contains("LoaiGiam")) { string lg = row["LoaiGiam"].ToString(); cboLoaiGiam.SelectedIndex = lg == "%" ? 0 : 1; }
                
                string apDungTheoDB = row["ApDungTheo"].ToString();
                if (apDungTheoDB == "DanhMuc") cboApDung.Text = "Danh mục";
                else if (apDungTheoDB == "SanPham") cboApDung.Text = "Sản phẩm";
                else cboApDung.Text = "Hóa đơn";
                
                string start = row["NgayBatDau"].ToString();
                dpNgayBatDau.Text = start.Length > 10 ? start.Substring(0, 10) : start;
                
                string end = row["NgayKetThuc"].ToString();
                dpNgayKetThuc.Text = end.Length > 10 ? end.Substring(0, 10) : end;
                
                if (row.Row.Table.Columns.Contains("MaDanhMuc") && row["MaDanhMuc"] != System.DBNull.Value)
                    cboDanhMuc.SelectedValue = row["MaDanhMuc"];
                else
                    cboDanhMuc.SelectedIndex = -1;

                if (row.Row.Table.Columns.Contains("MaSanPham") && row["MaSanPham"] != System.DBNull.Value)
                    cboSanPham.SelectedValue = row["MaSanPham"];
                else
                    cboSanPham.SelectedIndex = -1;
                    
                if (row.Row.Table.Columns.Contains("TinhTrang") && row["TinhTrang"] != System.DBNull.Value)
                {
                    string tinhTrang = row["TinhTrang"].ToString();
                    if (tinhTrang == "Đang diễn ra") cboTrangThai.SelectedIndex = 0;
                    else if (tinhTrang == "Chưa bắt đầu") cboTrangThai.SelectedIndex = 1;
                    else cboTrangThai.SelectedIndex = 2;
                }
                
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
            cboApDung.SelectedIndex = 0;
            cboDanhMuc.SelectedIndex = -1;
            cboSanPham.SelectedIndex = -1;
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
                if (string.IsNullOrWhiteSpace(txtTenChuongTrinh.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên chương trình khuyến mãi!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (!double.TryParse(txtMucGiam.Text, out double mucGiam))
                {
                    MessageBox.Show("Vui lòng nhập mức giảm hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string loaiGiam = cboLoaiGiam.Text;
                string apDungUI = cboApDung.Text;
                string apDungTheo = "HoaDon";
                if (apDungUI == "Danh mục") apDungTheo = "DanhMuc";
                else if (apDungUI == "Sản phẩm") apDungTheo = "SanPham";
                
                object maDanhMuc = null;
                object maSanPham = null;
                
                if (apDungTheo == "DanhMuc" && cboDanhMuc.SelectedValue != null)
                    maDanhMuc = cboDanhMuc.SelectedValue;
                if (apDungTheo == "SanPham" && cboSanPham.SelectedValue != null)
                    maSanPham = cboSanPham.SelectedValue;
                
                System.DateTime ngayBatDau = dpNgayBatDau.SelectedDate ?? System.DateTime.Now;
                System.DateTime ngayKetThuc = dpNgayKetThuc.SelectedDate ?? System.DateTime.Now.AddMonths(1);
                
                bool trangThai = cboTrangThai.SelectedIndex != 2;

                bool isUpdate = !string.IsNullOrEmpty(txtMaKhuyenMai.Text);
                bool result = false;

                if (isUpdate)
                {
                    int maKhuyenMai = int.Parse(txtMaKhuyenMai.Text);
                    result = _bus.Update(maKhuyenMai, txtTenChuongTrinh.Text, loaiGiam, mucGiam, apDungTheo, maDanhMuc, maSanPham, ngayBatDau, ngayKetThuc, trangThai);
                }
                else
                {
                    result = _bus.Insert(txtTenChuongTrinh.Text, loaiGiam, mucGiam, apDungTheo, maDanhMuc, maSanPham, ngayBatDau, ngayKetThuc, trangThai);
                }

                if (result)
                {
                    MessageBox.Show((isUpdate ? "Cập nhật" : "Thêm") + " khuyến mãi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    crudPanel.Visibility = Visibility.Collapsed;
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Thao tác thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa chương trình khuyến mãi này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        int maKhuyenMai = int.Parse(row["MaKhuyenMai"].ToString());
                        if (_bus.Delete(maKhuyenMai))
                        {
                            MessageBox.Show("Xóa khuyến mãi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
