using LUnivers_Beaute.Helpers;
using System.Windows;
using System.Windows.Controls;
using BUS;
using System.Data;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LUnivers_Beaute.Views
{
    public partial class KiemKeView : UserControl
    {
        private PagingHelper _pager;
        private KiemKeBUS _bus = new KiemKeBUS();
        private CuaHangBUS _chBus = new CuaHangBUS();
        private ObservableCollection<AuditItem> _auditItems = new ObservableCollection<AuditItem>();

        public KiemKeView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = _bus.GetAll();
                if (dgData != null)
                {
                    _pager.SetData(dt);
                }
                UpdateStatistics(dt);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateStatistics(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                txtKhop.Text = "0";
                txtThua.Text = "0";
                txtThieu.Text = "0";
                return;
            }

            int khop = dt.AsEnumerable().Count(r => r.Field<string>("TinhTrang") == "Khớp");
            int thua = dt.AsEnumerable().Count(r => r.Field<string>("TinhTrang") == "Thừa");
            int thieu = dt.AsEnumerable().Count(r => r.Field<string>("TinhTrang") == "Thiếu");

            txtKhop.Text = khop.ToString();
            txtThua.Text = thua.ToString();
            txtThieu.Text = thieu.ToString();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            try
            {
                DataTable dt = _bus.GetAll();
                string keyword = txtSearch?.Text?.Trim()?.ToLower() ?? "";
                string tinhTrang = (cmbTinhTrang?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả tình trạng";

                var rows = dt.AsEnumerable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    rows = rows.Where(r =>
                        (r.Field<string>("TenSanPham") ?? "").ToLower().Contains(keyword) ||
                        (r.Field<string>("TenCuaHang") ?? "").ToLower().Contains(keyword) ||
                        (r.Field<string>("SoLo") ?? "").ToLower().Contains(keyword));
                }

                if (tinhTrang != "Tất cả tình trạng")
                {
                    rows = rows.Where(r => r.Field<string>("TinhTrang") == tinhTrang);
                }

                DataTable filtered = rows.Any() ? rows.CopyToDataTable() : dt.Clone();
                _pager.SetData(filtered);
                UpdateStatistics(filtered);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        // ===== CREATE OVERLAY =====

        private void BtnOpenCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cboCuaHangKiemKe.ItemsSource = _chBus.GetAll().DefaultView;
                if (cboCuaHangKiemKe.Items.Count > 0) cboCuaHangKiemKe.SelectedIndex = 0;
            }
            catch { }

            _auditItems.Clear();
            icAuditItems.ItemsSource = _auditItems;
            createPanel.Visibility = Visibility.Visible;
        }

        private void BtnCloseCreate_Click(object sender, RoutedEventArgs e)
        {
            createPanel.Visibility = Visibility.Collapsed;
        }

        private void CboCuaHangKiemKe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStockForAudit();
        }

        private void BtnReloadStock_Click(object sender, RoutedEventArgs e)
        {
            LoadStockForAudit();
        }

        private void LoadStockForAudit()
        {
            if (cboCuaHangKiemKe.SelectedItem == null) return;

            try
            {
                string maCuaHang = ((DataRowView)cboCuaHangKiemKe.SelectedItem)["MaCuaHang"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(maCuaHang)) return;

                DataTable dt = _bus.GetTonKhoForKiemKe(maCuaHang);
                _auditItems.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    _auditItems.Add(new AuditItem
                    {
                        MaLo = System.Convert.ToInt32(row["MaLo"]),
                        TenSanPham = row["TenSanPham"]?.ToString() ?? "",
                        SoLo = row["SoLo"]?.ToString() ?? "",
                        HanSuDung = row["HanSuDung"]?.ToString() ?? "",
                        SoLuongHeThong = System.Convert.ToInt32(row["SoLuongHeThong"]),
                        SoLuongThucTe = System.Convert.ToInt32(row["SoLuongHeThong"]) // Default to system qty
                    });
                }

                icAuditItems.ItemsSource = _auditItems;
                lblItemCount.Text = _auditItems.Count + " sản phẩm";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu tồn kho: " + ex.Message);
            }
        }

        private void BtnXacNhanKiemKe_Click(object sender, RoutedEventArgs e)
        {
            if (_auditItems.Count == 0)
            {
                MessageBox.Show("Không có sản phẩm nào để kiểm kê!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cboCuaHangKiemKe.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn Cửa hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string maCuaHang = ((DataRowView)cboCuaHangKiemKe.SelectedItem)["MaCuaHang"]?.ToString() ?? "";

                var listObj = new System.Collections.Generic.List<object>();
                foreach (var item in _auditItems)
                {
                    listObj.Add(new
                    {
                        MaLo = item.MaLo,
                        SoLuongHeThong = item.SoLuongHeThong,
                        SoLuongThucTe = item.SoLuongThucTe
                    });
                }

                string json = System.Text.Json.JsonSerializer.Serialize(listObj);

                bool result = _bus.TaoPhieuKiemKe(maCuaHang, json);

                if (result)
                {
                    MessageBox.Show("Tạo phiếu kiểm kê thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    createPanel.Visibility = Visibility.Collapsed;
                    _auditItems.Clear();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Tạo phiếu kiểm kê thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                int maKiemKe = System.Convert.ToInt32(row["MaKiemKe"]);
                var result = MessageBox.Show($"Bạn có chắc muốn xóa phiếu kiểm kê #{maKiemKe}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(maKiemKe);
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
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

    public class AuditItem : INotifyPropertyChanged
    {
        public int MaLo { get; set; }
        public string TenSanPham { get; set; } = "";
        public string SoLo { get; set; } = "";
        public string HanSuDung { get; set; } = "";
        public int SoLuongHeThong { get; set; }

        private int _soLuongThucTe;
        public int SoLuongThucTe
        {
            get => _soLuongThucTe;
            set
            {
                _soLuongThucTe = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SoLuongThucTe)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
