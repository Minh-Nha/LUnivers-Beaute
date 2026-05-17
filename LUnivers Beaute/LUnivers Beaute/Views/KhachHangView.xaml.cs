using LUnivers_Beaute.Helpers;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class KhachHangView : UserControl
    {
        private KhachHangBUS _bus = new KhachHangBUS();
        private PagingHelper _pager;

        public KhachHangView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
        }

        // ===== DATA LOADING =====

        private void LoadData()
        {
            try
            {
                string? keyword = string.IsNullOrWhiteSpace(txtSearch?.Text) ? null : txtSearch.Text.Trim();
                int? trangThai = GetSelectedTrangThai();

                DataTable dt = _bus.GetAll(keyword, trangThai);
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
                "Hoạt động" => 1,
                "Khóa" => 0,
                _ => null
            };
        }

        private void UpdateStatistics(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                txtTongKH.Text = "0";
                txtHoatDong.Text = "0";
                txtTongDiem.Text = "0";
                return;
            }

            txtTongKH.Text = dt.Rows.Count.ToString();
            txtHoatDong.Text = dt.AsEnumerable().Count(r => r.Field<string>("TrangThai") == "Hoạt động").ToString();
            
            int tongDiem = dt.AsEnumerable().Sum(r =>
            {
                int diem = 0;
                int.TryParse(r["DiemTichLuy"]?.ToString(), out diem);
                return diem;
            });
            txtTongDiem.Text = tongDiem.ToString("N0");
        }

        // ===== SEARCH & FILTER =====

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // ===== CRUD OPERATIONS =====

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✨ Thêm Khách hàng";
            ClearForm();
            txtDiemTichLuy.IsReadOnly = false;
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                txtPanelTitle.Text = "✏️ Cập nhật Khách hàng";
                FillForm(row);
                txtDiemTichLuy.IsReadOnly = false;
                crudPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtHoTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên khách hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string hoTen = txtHoTen.Text.Trim();
                string sdt = txtSoDienThoai.Text.Trim();
                int diem = 0;
                int.TryParse(txtDiemTichLuy.Text, out diem);
                bool trangThai = cboTrangThai.SelectedIndex == 0; // 0 = Hoạt động, 1 = Khóa

                if (string.IsNullOrEmpty(txtMaKhachHang.Text))
                {
                    // INSERT
                    _bus.Insert(hoTen, sdt, diem, trangThai);
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // UPDATE
                    int maKH = int.Parse(txtMaKhachHang.Text);
                    _bus.Update(maKH, hoTen, sdt, diem, trangThai);
                    MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("UC_SDT_KhachHang"))
                {
                    MessageBox.Show("Số điện thoại này đã được sử dụng cho khách hàng khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa khách hàng \"{hoTen}\"?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        int maKH = System.Convert.ToInt32(row["MaKhachHang"]);
                        _bus.Delete(maKH);
                        MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        crudPanel.Visibility = Visibility.Collapsed;
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message.Contains("REFERENCE") || ex.Message.Contains("FK_"))
                        {
                            MessageBox.Show("Không thể xóa vì khách hàng này đã có lịch sử mua hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        // ===== MODAL HELPERS =====

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void FillForm(DataRowView row)
        {
            txtMaKhachHang.Text = row["MaKhachHang"]?.ToString() ?? "";
            txtHoTen.Text = row["HoTen"]?.ToString() ?? "";
            txtSoDienThoai.Text = row["SoDienThoai"]?.ToString() ?? "";
            txtDiemTichLuy.Text = row["DiemTichLuy"]?.ToString() ?? "0";
            string trangThai = row["TrangThai"]?.ToString() ?? "Hoạt động";
            cboTrangThai.SelectedIndex = (trangThai == "Khóa") ? 1 : 0;
        }

        private void ClearForm()
        {
            txtMaKhachHang.Text = "";
            txtHoTen.Text = "";
            txtSoDienThoai.Text = "";
            txtDiemTichLuy.Text = "0";
            cboTrangThai.SelectedIndex = 0;
        }

        // ===== PAGING =====

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
