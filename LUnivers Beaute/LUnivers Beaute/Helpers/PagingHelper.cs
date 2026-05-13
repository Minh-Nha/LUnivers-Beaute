using System;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace LUnivers_Beaute.Helpers
{
    public class PagingHelper
    {
        private DataTable _fullData;
        private DataGrid _dataGrid;
        private TextBlock _txtPageInfo;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;

        public PagingHelper(DataGrid dataGrid, TextBlock txtPageInfo, int pageSize = 10)
        {
            _dataGrid = dataGrid;
            _txtPageInfo = txtPageInfo;
            _pageSize = pageSize;
        }

        public void SetData(DataTable data)
        {
            if (data == null) return;
            _fullData = data;
            _totalPages = (int)Math.Ceiling((double)_fullData.Rows.Count / _pageSize);
            if (_totalPages == 0) _totalPages = 1;
            
            if (_currentPage > _totalPages)
                _currentPage = _totalPages;
                
            UpdateView();
        }

        public void NextPage()
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdateView();
            }
        }

        public void PreviousPage()
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            if (_fullData == null) return;
            
            var pagedRows = _fullData.AsEnumerable()
                                     .Skip((_currentPage - 1) * _pageSize)
                                     .Take(_pageSize);
            
            if (pagedRows.Any())
            {
                _dataGrid.ItemsSource = pagedRows.CopyToDataTable().DefaultView;
            }
            else
            {
                _dataGrid.ItemsSource = _fullData.Clone().DefaultView;
            }
            
            if (_txtPageInfo != null)
            {
                _txtPageInfo.Text = $"Trang {_currentPage} / {_totalPages}";
            }
        }
    }
}
