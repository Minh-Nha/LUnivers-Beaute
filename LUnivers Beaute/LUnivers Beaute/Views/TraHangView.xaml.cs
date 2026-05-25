using LUnivers_Beaute.Helpers;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;
using BUS;

namespace LUnivers_Beaute.Views
{
    public class ChiTietPhieuTraModel
    {
        public int MaLo { get; set; }
        public string TenSanPham { get; set; } = "";
        public int SoLuongTra { get; set; }
        public decimal SoTienHoan { get; set; }
    }

    public class ComboItemModel
    {
        public string Value { get; set; } = "";
        public string Display { get; set; } = "";
        public string ExtraData { get; set; } = "";

        public override string ToString()
        {
            return Display;
        }
    }

    public partial class TraHangView : UserControl
    {
        private PhieuTraHangBUS _bus = new PhieuTraHangBUS();
        private PagingHelper _pager;
        private ObservableCollection<ChiTietPhieuTraModel> _chiTietTemp = new ObservableCollection<ChiTietPhieuTraModel>();

        public TraHangView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
            if (dgData != null)
            {
                dgData.SelectionChanged += DgData_SelectionChanged;
            }
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) _pager.SetData(_bus.GetAllPhieuTraHang());
                
                // Binding temp list to dgChiTietPhieuTra_Temp
                if (dgChiTietPhieuTra_Temp != null) dgChiTietPhieuTra_Temp.ItemsSource = _chiTietTemp;

                // Load HoaDon for ComboBox
                DataTable dtHoaDon = new HoaDonBUS().GetAll();
                var listHoaDon = new System.Collections.Generic.List<ComboItemModel>();
                foreach (DataRow r in dtHoaDon.Rows)
                {
                    string date = r.Table.Columns.Contains("NgayLap") ? r["NgayLap"].ToString() : "";
                    string cus = r.Table.Columns.Contains("KhachHang") ? r["KhachHang"].ToString() : "";
                    listHoaDon.Add(new ComboItemModel
                    {
                        Value = r["MaHoaDon"].ToString(),
                        Display = $"{r["MaHoaDon"]} - Khách: {cus} ({date})"
                    });
                }
                cboHoaDon.ItemsSource = listHoaDon;
                cboHoaDon.DisplayMemberPath = "Display";
                cboHoaDon.SelectedValuePath = "Value";

                // Load Lô Sản Phẩm
                DataTable dtTonKho = new TonKhoBUS().GetAll();
                var listTonKho = new System.Collections.Generic.List<ComboItemModel>();
                foreach (DataRow r in dtTonKho.Rows)
                {
                    string soLo = r.Table.Columns.Contains("SoLo") ? r["SoLo"].ToString() : "?";
                    string maLo = r.Table.Columns.Contains("MaLo") ? r["MaLo"].ToString() : soLo;
                    string tenSp = r["TenSanPham"].ToString();
                    listTonKho.Add(new ComboItemModel
                    {
                        Value = maLo,
                        Display = $"[Lô {soLo}] {tenSp}",
                        ExtraData = tenSp
                    });
                }
                cboChiTietLo.ItemsSource = listTonKho;
                cboChiTietLo.DisplayMemberPath = "Display";
                cboChiTietLo.SelectedValuePath = "Value";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                string? maPhieuTra = row["MaPhieuTra"].ToString();
                try
                {
                    if (dgChiTiet != null && !string.IsNullOrEmpty(maPhieuTra))
                        dgChiTiet.ItemsSource = _bus.GetChiTietPhieuTra(maPhieuTra).DefaultView;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboHoaDon.SelectedValue == null || string.IsNullOrWhiteSpace(txtLyDo.Text) || _chiTietTemp.Count == 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin (Hóa đơn, Lý do) và ít nhất 1 chi tiết trả hàng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string maHoaDon = cboHoaDon.SelectedValue.ToString();
                string lyDo = txtLyDo.Text;
                DateTime ngayTra = dpNgayTra.SelectedDate ?? DateTime.Now;
                string maPhieuTra = "PT" + DateTime.Now.ToString("yyMMddHHmmss");

                string jsonChiTiet = JsonSerializer.Serialize(_chiTietTemp);

                if (_bus.InsertPhieuTraHang(maPhieuTra, maHoaDon, ngayTra, lyDo, jsonChiTiet))
                {
                    MessageBox.Show("Tạo phiếu trả hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    crudPanel.Visibility = Visibility.Collapsed;
                    _chiTietTemp.Clear();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu phiếu trả: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            if (cboChiTietLo.SelectedValue == null || string.IsNullOrWhiteSpace(txtChiTietSoLuong.Text) || string.IsNullOrWhiteSpace(txtChiTietTienHoan.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin chi tiết!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int maLo = Convert.ToInt32(cboChiTietLo.SelectedValue);
                int soLuong = Convert.ToInt32(txtChiTietSoLuong.Text);
                decimal tienHoan = Convert.ToDecimal(txtChiTietTienHoan.Text);
                
                ComboItemModel item = (ComboItemModel)cboChiTietLo.SelectedItem;
                string tenSanPham = item.ExtraData;

                _chiTietTemp.Add(new ChiTietPhieuTraModel
                {
                    MaLo = maLo,
                    TenSanPham = tenSanPham,
                    SoLuongTra = soLuong,
                    SoTienHoan = tienHoan
                });

                txtChiTietSoLuong.Clear();
                txtChiTietTienHoan.Clear();
            }
            catch
            {
                MessageBox.Show("Vui lòng nhập số liệu hợp lệ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnRemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ChiTietPhieuTraModel item)
            {
                _chiTietTemp.Remove(item);
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
