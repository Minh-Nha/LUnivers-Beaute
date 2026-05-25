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

        private string _userRole = "";
        private string _userMaCuaHang = "";
        private string _userMaNhanVien = "";

        public NhapKhoView(string userRole = "", string userMaCuaHang = "", string userMaNhanVien = "")
        {
            InitializeComponent();
            _userRole = userRole;
            _userMaCuaHang = userMaCuaHang;
            _userMaNhanVien = userMaNhanVien;
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
                
                var dtCH = _chBus.GetAll().Copy();
                string role = (_userRole ?? "").Trim().ToLower();
                if (role != "admin" && !string.IsNullOrEmpty(_userMaCuaHang))
                {
                    for (int i = dtCH.Rows.Count - 1; i >= 0; i--)
                    {
                        if (dtCH.Rows[i]["MaCuaHang"]?.ToString() != _userMaCuaHang)
                        {
                            dtCH.Rows.RemoveAt(i);
                        }
                    }
                }
                cboCuaHang.ItemsSource = dtCH.DefaultView;
                if (cboCuaHang.Items.Count > 0) cboCuaHang.SelectedIndex = 0;

                var dtNV = _nvBus.GetAll().Copy();
                if (role != "admin" && !string.IsNullOrEmpty(_userMaNhanVien))
                {
                    for (int i = dtNV.Rows.Count - 1; i >= 0; i--)
                    {
                        if (dtNV.Rows[i]["MaNhanVien"]?.ToString() != _userMaNhanVien)
                        {
                            dtNV.Rows.RemoveAt(i);
                        }
                    }
                }
                cboNhanVien.ItemsSource = dtNV.DefaultView;
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
                    DataTable dt = _bus.GetAll();
                    string role = (_userRole ?? "").Trim().ToLower();
                    if (role != "admin" && !string.IsNullOrEmpty(_userMaCuaHang))
                    {
                        if (dt != null)
                        {
                            var filteredRows = dt.AsEnumerable()
                                .Where(row => row.Field<string>("MaCuaHang") == _userMaCuaHang);
                            dt = filteredRows.Any() ? filteredRows.CopyToDataTable() : dt.Clone();
                        }
                    }

                    _pager.SetData(dt);
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

        private void cboSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSanPham.SelectedItem is DataRowView row)
            {
                if (row["GiaNiemYet"] != DBNull.Value)
                {
                    if (decimal.TryParse(row["GiaNiemYet"].ToString(), out decimal gia))
                    {
                        txtGiaNiemYet.Text = gia.ToString("N0");
                    }
                    else
                    {
                        txtGiaNiemYet.Text = row["GiaNiemYet"].ToString();
                    }
                }
                else
                {
                    txtGiaNiemYet.Text = "0";
                }
            }
            else
            {
                txtGiaNiemYet.Clear();
            }
        }

        private void cboNhanVien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboNhanVien.SelectedItem is DataRowView row)
            {
                if (row.Row.Table.Columns.Contains("MaCuaHang") && row["MaCuaHang"] != DBNull.Value)
                {
                    cboCuaHang.SelectedValue = row["MaCuaHang"].ToString();
                }
            }
        }

        private void BtnAddImportItem_Click(object sender, RoutedEventArgs e)
        {
            if (cboSanPham.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string cleanGiaNhap = txtGiaNhap.Text.Replace(",", "").Replace(".", "").Trim();
            if (!int.TryParse(txtSoLuong.Text, out int sl) || !double.TryParse(cleanGiaNhap, out double gia))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin hợp lệ (Số lượng, Giá nhập)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double giaNiemYet = 0;
            if (!string.IsNullOrWhiteSpace(txtGiaNiemYet.Text))
            {
                string cleanGiaNiemYet = txtGiaNiemYet.Text.Replace(",", "").Replace(".", "").Trim();
                if (!double.TryParse(cleanGiaNiemYet, out giaNiemYet))
                {
                    MessageBox.Show("Giá bán niêm yết không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DataRowView row = (DataRowView)cboSanPham.SelectedItem;
            string maSp = row["MaSanPham"].ToString();
            
            // Auto-generate SoLo if empty
            string soLo = txtSoLo.Text.Trim();
            if (string.IsNullOrWhiteSpace(soLo))
            {
                soLo = "LO" + System.DateTime.Now.ToString("yyMMddHHmmss");
            }
            
            _importCart.Add(new ImportItem
            {
                MaSanPham = maSp,
                TenSanPham = row["TenSanPham"].ToString(),
                SoLo = soLo,
                NgaySX = dpNgaySX.SelectedDate ?? System.DateTime.Now,
                HanSD = dpHanSD.SelectedDate ?? System.DateTime.Now.AddYears(1),
                SoLuong = sl,
                GiaNhap = gia,
                GiaNiemYet = giaNiemYet
            });

            txtSoLo.Clear();
            txtSoLuong.Text = "1";
            txtGiaNhap.Clear();
            txtGiaNiemYet.Clear();
            cboSanPham.SelectedIndex = -1;
        }

        private void BtnAddAllItems_Click(object sender, RoutedEventArgs e)
        {
            if (cboSanPham.Items.Count == 0) return;

            string soLo = "LOTEST" + System.DateTime.Now.ToString("MMddHHmmss");
            
            foreach (System.Data.DataRowView row in cboSanPham.Items)
            {
                string maSp = row["MaSanPham"].ToString();
                
                _importCart.Add(new ImportItem
                {
                    MaSanPham = maSp,
                    TenSanPham = row["TenSanPham"].ToString(),
                    SoLo = soLo,
                    NgaySX = System.DateTime.Now.AddMonths(-1),
                    HanSD = System.DateTime.Now.AddYears(2),
                    SoLuong = 500, // Số lượng test
                    GiaNhap = 150000, // Giá nhập test
                    GiaNiemYet = 350000 // Giá bán test
                });
            }
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
                        GiaNiemYet = item.GiaNiemYet,
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

        private void TxtCurrency_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Remove existing commas, dots and non-digit characters (except maybe dot for decimal)
                string value = new string(textBox.Text.Where(char.IsDigit).ToArray());
                
                if (!string.IsNullOrEmpty(value))
                {
                    if (decimal.TryParse(value, out decimal amount))
                    {
                        // Format the number with commas
                        textBox.TextChanged -= TxtCurrency_TextChanged;
                        textBox.Text = amount.ToString("N0");
                        textBox.CaretIndex = textBox.Text.Length;
                        textBox.TextChanged += TxtCurrency_TextChanged;
                    }
                }
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lblMaPhieu.Text)) return;

            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"PhieuNhapKho_{lblMaPhieu.Text}.pdf",
                    DefaultExt = ".pdf",
                    Filter = "PDF documents (.pdf)|*.pdf"
                };

                if (dialog.ShowDialog() == true)
                {
                    string ttStr = new string(lblTongTien.Text.Where(c => char.IsDigit(c)).ToArray());
                    var model = new LUnivers_Beaute.Services.ImportReceiptModel
                    {
                        MaPhieuNhap = lblMaPhieu.Text,
                        TenNhanVien = lblNhanVien.Text,
                        TenCuaHang = lblCuaHang.Text,
                        NgayNhap = DateTime.TryParse(lblNgayNhap.Text, out DateTime dt) ? dt : DateTime.Now,
                        TongTienNhap = decimal.TryParse(ttStr, out decimal tt) ? tt : 0
                    };

                    if (dgChiTiet.ItemsSource is System.Data.DataView dataView)
                    {
                        foreach (System.Data.DataRowView row in dataView)
                        {
                            string hsdStr = row["HanSuDung"].ToString();
                            if (DateTime.TryParse(hsdStr, out DateTime hsdDate))
                                hsdStr = hsdDate.ToString("dd/MM/yyyy");

                            string gnStr = new string(row["GiaNhap"].ToString().Where(c => char.IsDigit(c)).ToArray());

                            model.Items.Add(new LUnivers_Beaute.Services.ImportReceiptItem
                            {
                                TenSanPham = row["TenSanPham"].ToString(),
                                SoLo = row["SoLo"].ToString(),
                                HanSuDung = hsdStr,
                                SoLuong = int.TryParse(row["SoLuong"].ToString(), out int sl) ? sl : 0,
                                GiaNhap = decimal.TryParse(gnStr, out decimal gn) ? gn : 0
                            });
                        }
                    }

                    var pdfService = new LUnivers_Beaute.Services.PdfImportReceiptService();
                    pdfService.GenerateReceipt(model, dialog.FileName);

                    // Mở file PDF sau khi tạo thành công
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi in phiếu nhập: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private double _giaNiemYet;
        public double GiaNiemYet
        {
            get => _giaNiemYet;
            set { _giaNiemYet = value; OnPropertyChanged(nameof(GiaNiemYet)); }
        }

        public double ThanhTien => SoLuong * GiaNhap;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}
