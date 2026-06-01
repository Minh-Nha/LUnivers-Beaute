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
    public partial class LichSuChinhSuaView : UserControl
    {
        private List<EditLog> _allLogs;

        public LichSuChinhSuaView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _allLogs = LogService.GetEditLogs();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allLogs == null) return;

            string searchText = (txtSearch.Text ?? "").Trim().ToLower();
            var filtered = _allLogs.Where(l => 
                l.User.ToLower().Contains(searchText) || 
                l.Action.ToLower().Contains(searchText) ||
                l.Detail.ToLower().Contains(searchText)
            ).ToList();

            dgEditLogs.ItemsSource = filtered;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            txtSearch.Text = "";
            LUnivers_Beaute.Helpers.ModernMessageBox.Show("Đã làm mới danh sách nhật ký chỉnh sửa & hoạt động!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filteredLogs = dgEditLogs.ItemsSource as List<EditLog>;
                if (filteredLogs == null || filteredLogs.Count == 0)
                {
                    LUnivers_Beaute.Helpers.ModernMessageBox.Show("Không có dữ liệu lịch sử để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Chuyển đổi dữ liệu sang DataTable để dùng ExcelExportHelper
                var dt = new DataTable();
                dt.Columns.Add("Thời gian", typeof(string));
                dt.Columns.Add("Hành động", typeof(string));
                dt.Columns.Add("Chi tiết thao tác", typeof(string));
                dt.Columns.Add("Người thực hiện", typeof(string));

                foreach (var log in filteredLogs)
                {
                    dt.Rows.Add(log.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"), log.Action, log.Detail, log.User);
                }

                ExcelExportHelper.ExportToExcel(dt, "LichSuHoatDong_LUniversBeaute.xlsx");
            }
            catch (Exception ex)
            {
                LUnivers_Beaute.Helpers.ModernMessageBox.Show("Lỗi khi xuất lịch sử hoạt động: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
