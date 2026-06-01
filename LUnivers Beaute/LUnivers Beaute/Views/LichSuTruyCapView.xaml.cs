using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LUnivers_Beaute.Helpers;
using LUnivers_Beaute.Services;

namespace LUnivers_Beaute.Views
{
    public partial class LichSuTruyCapView : UserControl
    {
        private List<AccessLog> _allLogs;

        public LichSuTruyCapView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _allLogs = LogService.GetAccessLogs();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allLogs == null) return;

            string searchText = (txtSearch.Text ?? "").Trim().ToLower();
            var filtered = _allLogs.Where(l => 
                (l.User ?? "").ToLower().Contains(searchText) || 
                (l.IpAddress ?? "").ToLower().Contains(searchText) ||
                (l.DeviceName ?? "").ToLower().Contains(searchText) ||
                (l.Location ?? "").ToLower().Contains(searchText)
            ).ToList();

            dgAccessLogs.ItemsSource = filtered;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            txtSearch.Text = "";
            LUnivers_Beaute.Helpers.ModernMessageBox.Show("Đã làm mới danh sách nhật ký truy cập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filteredLogs = dgAccessLogs.ItemsSource as List<AccessLog>;
                if (filteredLogs == null || filteredLogs.Count == 0)
                {
                    LUnivers_Beaute.Helpers.ModernMessageBox.Show("Không có dữ liệu lịch sử để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Chuyển đổi dữ liệu sang DataTable để dùng ExcelExportHelper
                var dt = new DataTable();
                dt.Columns.Add("Thời gian", typeof(string));
                dt.Columns.Add("Nhân viên thực hiện", typeof(string));
                dt.Columns.Add("Địa chỉ IP", typeof(string));
                dt.Columns.Add("Tên thiết bị", typeof(string));
                dt.Columns.Add("Địa điểm / Chi nhánh", typeof(string));

                foreach (var log in filteredLogs)
                {
                    dt.Rows.Add(
                        log.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"), 
                        log.User, 
                        log.IpAddress, 
                        log.DeviceName, 
                        log.Location
                    );
                }

                ExcelExportHelper.ExportToExcel(dt, "LichSuTruyCap_LUniversBeaute.xlsx");
            }
            catch (Exception ex)
            {
                LUnivers_Beaute.Helpers.ModernMessageBox.Show("Lỗi khi xuất lịch sử truy cập: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
