using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class ChamCongView : UserControl
    {
        private ChamCongBUS _bus = new ChamCongBUS();
        private PagingHelper _pager;
        private int _currentMaCC = 0;

        public ChamCongView()
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
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Chấm công";
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
                txtPanelTitle.Text = "📋 Chi tiết Chấm công";
                _currentMaCC = Convert.ToInt32(row["MaCC"]);
                txtMaNhanVien.Text = row["MaNhanVien"].ToString();
                
                string ngayLam = row["NgayLam"].ToString();
                dpNgayLam.Text = ngayLam.Length > 10 ? ngayLam.Substring(0, 10) : ngayLam;
                
                txtGioVao.Text = row["GioVao"].ToString();
                txtGioRa.Text = row["GioRa"].ToString();
                
                crudPanel.Visibility = Visibility.Visible;
            }
        }
        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string maNV = txtMaNhanVien.Text.Trim();
                if (string.IsNullOrEmpty(maNV) || !dpNgayLam.SelectedDate.HasValue || string.IsNullOrEmpty(txtGioVao.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Mã NV, Ngày làm và Giờ vào!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime ngayLam = dpNgayLam.SelectedDate.Value;
                if (!System.TimeSpan.TryParse(txtGioVao.Text, out System.TimeSpan gioVao))
                {
                    MessageBox.Show("Giờ vào không hợp lệ (định dạng HH:mm)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                System.TimeSpan? gioRa = null;
                if (!string.IsNullOrEmpty(txtGioRa.Text))
                {
                    if (System.TimeSpan.TryParse(txtGioRa.Text, out System.TimeSpan temp))
                    {
                        gioRa = temp;
                    }
                    else
                    {
                        MessageBox.Show("Giờ ra không hợp lệ (định dạng HH:mm)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (_currentMaCC == 0)
                {
                    _bus.Insert(maNV, ngayLam, gioVao, gioRa);
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _bus.Update(_currentMaCC, maNV, ngayLam, gioVao, gioRa);
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
}
