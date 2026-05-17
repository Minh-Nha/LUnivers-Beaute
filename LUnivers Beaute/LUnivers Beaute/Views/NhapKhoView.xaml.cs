using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class NhapKhoView : UserControl
    {
        private PhieuNhapKhoBUS _bus = new PhieuNhapKhoBUS();
        private SanPhamBUS _spBus = new SanPhamBUS();
        private CuaHangBUS _chBus = new CuaHangBUS();
        private NhanVienBUS _nvBus = new NhanVienBUS();
        private PagingHelper _pager;
        private System.Collections.ObjectModel.ObservableCollection<ImportItem> _importCart = new System.Collections.ObjectModel.ObservableCollection<ImportItem>();

        public NhapKhoView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            dgImportCart.ItemsSource = _importCart;
            _importCart.CollectionChanged += (s, e) => UpdateTongTienMoi();
            
            this.Loaded += (s, e) => 
            {
                LoadData();
                LoadComboBoxes();
            };
            if (dgData != null)
            {
                dgData.SelectionChanged += DgData_SelectionChanged;
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                cboSanPham.ItemsSource = _spBus.GetAll().DefaultView;
                cboCuaHang.ItemsSource = _chBus.GetAll().DefaultView;
                cboNhanVien.ItemsSource = _nvBus.GetAll().DefaultView;
                
                if (cboCuaHang.Items.Count > 0) cboCuaHang.SelectedIndex = 0;
                if (cboNhanVien.Items.Count > 0) cboNhanVien.SelectedIndex = 0;
                
                dpNgaySX.SelectedDate = System.DateTime.Now;
                dpHanSD.SelectedDate = System.DateTime.Now.AddYears(1);
            }
            catch { }
        }

        private void UpdateTongTienMoi()
        {
            double total = 0;
            foreach (var item in _importCart)
            {
                total += item.ThanhTien;
            }
            lblTongTienMoi.Text = total.ToString("#,##0") + " đ";
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) 
                {
                    _pager.SetData(_bus.GetAll());
                    if (dgData.Items.Count > 0)
                    {
                        dgData.SelectedIndex = 0;
                    }
                    else
                    {
                        detailPanel.Visibility = Visibility.Collapsed;
                    }
                }
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
                string? maPhieuNhap = row["MaPhieuNhap"].ToString();
                
                lblMaPhieuTieuDe.Text = maPhieuNhap;
                lblMaPhieu.Text = maPhieuNhap;
                lblNgayNhap.Text = row["NgayNhap"].ToString();
                lblNhanVien.Text = row["NhanVienNhap"].ToString();
                lblCuaHang.Text = row["TenCuaHang"].ToString();
                
                lblTongTien.Text = row["TongTienNhap"]?.ToString() ?? "0 đ";

                try
                {
                    if (dgChiTiet != null && !string.IsNullOrEmpty(maPhieuNhap))
                        dgChiTiet.ItemsSource = _bus.GetChiTiet(maPhieuNhap).DefaultView;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                detailPanel.Visibility = Visibility.Visible;
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

        private void BtnTaoPhieuNhap_Click(object sender, RoutedEventArgs e)
        {
            createPanel.Visibility = Visibility.Visible;
            txtSoLo.Clear();
            txtSoLuong.Text = "1";
            txtGiaNhap.Clear();
            _importCart.Clear();
        }

        private void BtnCloseCreate_Click(object sender, RoutedEventArgs e)
        {
            createPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnAddImportItem_Click(object sender, RoutedEventArgs e)
        {
            if (cboSanPham.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSoLo.Text) || !int.TryParse(txtSoLuong.Text, out int sl) || !double.TryParse(txtGiaNhap.Text, out double gia))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin hợp lệ (Số lượng, Giá nhập)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)cboSanPham.SelectedItem;
            
            _importCart.Add(new ImportItem
            {
                MaSanPham = row["MaSanPham"].ToString(),
                TenSanPham = row["TenSanPham"].ToString(),
                SoLo = txtSoLo.Text,
                NgaySX = dpNgaySX.SelectedDate ?? System.DateTime.Now,
                HanSD = dpHanSD.SelectedDate ?? System.DateTime.Now.AddYears(1),
                SoLuong = sl,
                GiaNhap = gia
            });

            txtSoLo.Clear();
            txtSoLuong.Text = "1";
            txtGiaNhap.Clear();
            cboSanPham.SelectedIndex = -1;
        }

        private void BtnRemoveImportItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ImportItem item)
            {
                _importCart.Remove(item);
            }
        }

        private void BtnXacNhanNhap_Click(object sender, RoutedEventArgs e)
        {
            if (_importCart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cboCuaHang.SelectedItem == null || cboNhanVien.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn Cửa hàng và Nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string maPhieuNhap = "PN" + System.DateTime.Now.ToString("yyMMddHHmmss");
                string maCuaHang = ((DataRowView)cboCuaHang.SelectedItem)["MaCuaHang"].ToString();
                string maNhanVien = ((DataRowView)cboNhanVien.SelectedItem)["MaNhanVien"].ToString();
                
                double tongTien = 0;
                var listObj = new System.Collections.Generic.List<object>();
                
                foreach (var item in _importCart)
                {
                    tongTien += item.ThanhTien;
                    listObj.Add(new 
                    {
                        MaSanPham = item.MaSanPham,
                        SoLuong = item.SoLuong,
                        GiaNhap = item.GiaNhap,
                        SoLo = item.SoLo,
                        NgaySanXuat = item.NgaySX.ToString("yyyy-MM-dd"),
                        HanSuDung = item.HanSD.ToString("yyyy-MM-dd")
                    });
                }
                
                string json = System.Text.Json.JsonSerializer.Serialize(listObj);
                
                bool result = _bus.TaoPhieuNhap(maPhieuNhap, maCuaHang, maNhanVien, tongTien, json);
                
                if (result)
                {
                    MessageBox.Show("Tạo phiếu nhập thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    createPanel.Visibility = Visibility.Collapsed;
                    _importCart.Clear();
                    LoadData(); // reload
                }
                else
                {
                    MessageBox.Show("Tạo phiếu nhập thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ImportItem : System.ComponentModel.INotifyPropertyChanged
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string SoLo { get; set; }
        public System.DateTime NgaySX { get; set; }
        public System.DateTime HanSD { get; set; }

        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set { _soLuong = value; OnPropertyChanged(nameof(SoLuong)); OnPropertyChanged(nameof(ThanhTien)); }
        }

        private double _giaNhap;
        public double GiaNhap
        {
            get => _giaNhap;
            set { _giaNhap = value; OnPropertyChanged(nameof(GiaNhap)); OnPropertyChanged(nameof(ThanhTien)); }
        }

        public double ThanhTien => SoLuong * GiaNhap;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}
