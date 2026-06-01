using LUnivers_Beaute.Helpers;
using System.Windows;
using System.Windows.Controls;
using BUS;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LUnivers_Beaute.Views
{
    public partial class TonKhoView : UserControl
    {
        private PagingHelper _pager;
        private DataTable _allData;
        private string _currentSortColumn = "SoLuongTon";
        private string _currentSortOrder = "ASC";
        
        private string _userRole = "";
        private string _userMaCuaHang = "";

        public TonKhoView(string userRole = "", string userMaCuaHang = "")
        {
            InitializeComponent();
            _userRole = userRole;
            _userMaCuaHang = userMaCuaHang;
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => LoadData();
        }

        private async void LoadData()
        {
            try
            {
                TonKhoBUS bus = new TonKhoBUS();
                CuaHangBUS chBus = new CuaHangBUS();
                DanhMucBUS dmBus = new DanhMucBUS();

                var allDataTask = Task.Run(() => bus.GetAll());
                var chTask = Task.Run(() => chBus.GetAll());
                var dmTask = Task.Run(() => dmBus.GetAll());

                await Task.WhenAll(allDataTask, chTask, dmTask);

                _allData = allDataTask.Result;
                var dtCH = chTask.Result.Copy();
                var dtDM = dmTask.Result.Copy();

                string role = (_userRole ?? "").Trim().ToLower();
                if (role != "admin" && !string.IsNullOrEmpty(_userMaCuaHang))
                {
                    if (_allData != null)
                    {
                        var filteredRows = _allData.AsEnumerable()
                            .Where(row => row.Field<string>("MaCuaHang") == _userMaCuaHang);
                        _allData = filteredRows.Any() ? filteredRows.CopyToDataTable() : _allData.Clone();
                    }
                }

                UpdateStatistics();
                
                // Load filters
                if (role != "admin" && !string.IsNullOrEmpty(_userMaCuaHang))
                {
                    for (int i = dtCH.Rows.Count - 1; i >= 0; i--)
                    {
                        if (dtCH.Rows[i]["MaCuaHang"]?.ToString() != _userMaCuaHang)
                        {
                            dtCH.Rows.RemoveAt(i);
                        }
                    }
                    cboSearchCuaHang.ItemsSource = dtCH.DefaultView;
                    cboSearchCuaHang.SelectedIndex = 0;
                }
                else
                {
                    var rowCH = dtCH.NewRow();
                    rowCH["MaCuaHang"] = "";
                    rowCH["TenCuaHang"] = "Tất cả cửa hàng";
                    dtCH.Rows.InsertAt(rowCH, 0);
                    cboSearchCuaHang.ItemsSource = dtCH.DefaultView;
                    cboSearchCuaHang.SelectedIndex = 0;
                }

                var rowDM = dtDM.NewRow();
                rowDM["MaDanhMuc"] = 0;
                rowDM["TenDanhMuc"] = "Tất cả danh mục";
                dtDM.Rows.InsertAt(rowDM, 0);
                cboSearchDanhMuc.ItemsSource = dtDM.DefaultView;
                cboSearchDanhMuc.SelectedIndex = 0;

                FilterData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateStatistics()
        {
            if (_allData == null) return;
            
            int tongSp = _allData.AsEnumerable().Sum(row => row.Field<int>("SoLuongTon"));
            int sapHetHang = _allData.AsEnumerable().Count(row => row.Field<string>("TinhTrangKho") == "Sắp hết hàng" || row.Field<string>("TinhTrangKho") == "Hết hàng");
            int sapHetHan = _allData.AsEnumerable().Count(row => row.Field<string>("TinhTrangKho") == "Sắp hết hạn" || row.Field<string>("TinhTrangKho") == "Đã hết hạn");
            
            txtTongSP.Text = tongSp.ToString("N0");
            txtSapHetHang.Text = sapHetHang.ToString();
            txtSapHetHan.Text = sapHetHan.ToString();
        }

        private void FilterData()
        {
            try
            {
                TonKhoBUS bus = new TonKhoBUS();
                string searchText = txtSearch?.Text?.Trim() ?? "";
                string selectedStatus = (cmbTrangThai?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";
                if (selectedStatus == "Tất cả trạng thái") selectedStatus = "Tất cả";

                string maCuaHang = cboSearchCuaHang?.SelectedValue?.ToString();
                if (string.IsNullOrEmpty(maCuaHang)) maCuaHang = null;

                int? maDanhMuc = cboSearchDanhMuc?.SelectedValue as int?;
                if (maDanhMuc == 0) maDanhMuc = null;

                int? tuSoLuong = null;
                int? denSoLuong = null;

                DataTable filteredData = bus.SearchAndSort(searchText, selectedStatus, _currentSortColumn, _currentSortOrder, maCuaHang, maDanhMuc, tuSoLuong, denSoLuong);
                
                if (dgData != null)
                {
                    _pager.SetData(filteredData);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterData();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cboSearchCuaHang.SelectedIndex = 0;
            cboSearchDanhMuc.SelectedIndex = 0;
            cmbTrangThai.SelectedIndex = 0;
            FilterData();
        }

        private void dgData_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true; // Chặn sort mặc định của WPF

            string columnProperty = "";
            // Ánh xạ tên cột trong DataGrid sang tên cột trong Database
            switch (e.Column.Header.ToString())
            {
                case "CỬA HÀNG": columnProperty = "TenCuaHang"; break;
                case "SẢN PHẨM": columnProperty = "TenSanPham"; break;
                case "SỐ LÔ": columnProperty = "SoLo"; break;
                case "NGÀY SX": columnProperty = "NgaySanXuat"; break;
                case "HẠN SD": columnProperty = "HanSuDung"; break;
                case "SỐ LƯỢNG TỒN": columnProperty = "SoLuongTon"; break;
                case "TRẠNG THÁI": columnProperty = "TinhTrangKho"; break;
                default: columnProperty = "SoLuongTon"; break;
            }

            // Đảo chiều sắp xếp
            if (_currentSortColumn == columnProperty)
            {
                _currentSortOrder = (_currentSortOrder == "ASC") ? "DESC" : "ASC";
            }
            else
            {
                _currentSortColumn = columnProperty;
                _currentSortOrder = "ASC";
            }

            // Cập nhật icon mũi tên trên Header (không bắt buộc nhưng giúp UI chuyên nghiệp hơn)
            e.Column.SortDirection = (_currentSortOrder == "ASC") ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending;

            FilterData();
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.PreviousPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.NextPage();
        }

        private void BtnExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TonKhoBUS bus = new TonKhoBUS();
                string searchText = txtSearch?.Text?.Trim() ?? "";
                string selectedStatus = (cmbTrangThai?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";
                if (selectedStatus == "Tất cả trạng thái") selectedStatus = "Tất cả";

                string maCuaHang = cboSearchCuaHang?.SelectedValue?.ToString();
                if (string.IsNullOrEmpty(maCuaHang)) maCuaHang = null;

                int? maDanhMuc = cboSearchDanhMuc?.SelectedValue as int?;
                if (maDanhMuc == 0) maDanhMuc = null;

                int? tuSoLuong = null;
                int? denSoLuong = null;

                DataTable dt = bus.SearchAndSort(searchText, selectedStatus, _currentSortColumn, _currentSortOrder, maCuaHang, maDanhMuc, tuSoLuong, denSoLuong);
                ExcelExportHelper.ExportToExcel(dt, "BaoCaoTonKho.csv");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất báo cáo tồn kho: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
