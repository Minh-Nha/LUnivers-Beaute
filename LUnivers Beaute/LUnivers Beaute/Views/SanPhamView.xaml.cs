using LUnivers_Beaute.Helpers;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;
using System;
using LUnivers_Beaute.Services;
using System.Windows.Media.Imaging;

namespace LUnivers_Beaute.Views
{
    public partial class SanPhamView : UserControl
    {
        private SanPhamBUS _bus = new SanPhamBUS();
        private PagingHelper _pager;
        private bool _isUpdateMode = false;
        private string _uploadedImageUrl = "";
        private string _originalImageUrl = "";

        public SanPhamView()
        {
            InitializeComponent();
            _pager = new PagingHelper(dgData, txtPageInfo, 10);
            this.Loaded += (s, e) => 
            {
                LoadComboBoxes();
                LoadData();
            };
        }

        private void LoadComboBoxes()
        {
            try
            {
                var dm = new DanhMucBUS().GetAll();
                var th = new ThuongHieuBUS().GetAll();
                var ncc = new NhaCungCapBUS().GetAll();

                cboDanhMuc.ItemsSource = dm.DefaultView;
                cboDanhMuc.SelectedValuePath = "MaDanhMuc";

                cboThuongHieu.ItemsSource = th.DefaultView;
                cboThuongHieu.SelectedValuePath = "MaThuongHieu";

                cboNhaCungCap.ItemsSource = ncc.DefaultView;
                cboNhaCungCap.SelectedValuePath = "MaNhaCungCap";

                // Filters
                DataTable dmFilter = dm.Copy();
                DataRow drDm = dmFilter.NewRow();
                drDm["MaDanhMuc"] = 0;
                drDm["TenDanhMuc"] = "Tất cả danh mục";
                dmFilter.Rows.InsertAt(drDm, 0);
                cboFilterDanhMuc.ItemsSource = dmFilter.DefaultView;
                cboFilterDanhMuc.SelectedValuePath = "MaDanhMuc";
                cboFilterDanhMuc.SelectedIndex = 0;

                DataTable thFilter = th.Copy();
                DataRow drTh = thFilter.NewRow();
                drTh["MaThuongHieu"] = 0;
                drTh["TenThuongHieu"] = "Tất cả thương hiệu";
                thFilter.Rows.InsertAt(drTh, 0);
                cboFilterThuongHieu.ItemsSource = thFilter.DefaultView;
                cboFilterThuongHieu.SelectedValuePath = "MaThuongHieu";
                cboFilterThuongHieu.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu danh mục: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                string timKiem = txtSearch.Text.Trim();
                int? maDanhMuc = null;
                if (cboFilterDanhMuc.SelectedValue != null && cboFilterDanhMuc.SelectedIndex > 0)
                {
                    maDanhMuc = Convert.ToInt32(cboFilterDanhMuc.SelectedValue);
                }

                int? maThuongHieu = null;
                if (cboFilterThuongHieu.SelectedValue != null && cboFilterThuongHieu.SelectedIndex > 0)
                {
                    maThuongHieu = Convert.ToInt32(cboFilterThuongHieu.SelectedValue);
                }

                int? trangThai = null;
                if (cboFilterTrangThai.SelectedItem is ComboBoxItem cbi && cbi.Tag != null)
                {
                    trangThai = Convert.ToInt32(cbi.Tag);
                }

                if (dgData != null) 
                    _pager.SetData(_bus.GetAll(timKiem, maDanhMuc, maThuongHieu, trangThai));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtPanelTitle.Text = "✨ Thêm Sản phẩm";
            _isUpdateMode = false;
            ClearForm();
            
            // Tự động sinh mã sản phẩm và khóa không cho sửa
            txtMaSanPham.Text = "SP" + DateTime.Now.ToString("yyMMddHHmmss");
            txtMaSanPham.IsReadOnly = true;
            txtMaSanPham.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#F8F9FA");
            
            _originalImageUrl = "";
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.DataContext is DataRowView row)
            {
                PopulateForm(row);
            }
        }

        private void PopulateForm(DataRowView row)
        {
            txtPanelTitle.Text = "✏️ Cập nhật Sản phẩm";
            _isUpdateMode = true;
            txtMaSanPham.IsReadOnly = true;
            txtMaSanPham.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#F8F9FA");

            txtMaSanPham.Text = row["MaSanPham"].ToString();
            txtTenSanPham.Text = row["TenSanPham"].ToString();
            txtDonViTinh.Text = row["DonViTinh"].ToString();

            if (decimal.TryParse(row["GiaNiemYet"].ToString(), out decimal giaNiemYet))
            {
                txtGiaNiemYet.Text = giaNiemYet.ToString("N0");
            }
            else
            {
                txtGiaNiemYet.Text = row["GiaNiemYet"].ToString();
            }
            
            cboDanhMuc.SelectedValue = row["MaDanhMuc"] != DBNull.Value ? row["MaDanhMuc"] : null;
            cboThuongHieu.SelectedValue = row["MaThuongHieu"] != DBNull.Value ? row["MaThuongHieu"] : null;
            cboNhaCungCap.SelectedValue = row["MaNhaCungCap"] != DBNull.Value ? row["MaNhaCungCap"] : null;
            
            if (row["TrangThai"] != DBNull.Value)
            {
                bool trangThai = Convert.ToBoolean(row["TrangThai"]);
                cboTrangThai.SelectedIndex = trangThai ? 0 : 1;
            }
            else
            {
                cboTrangThai.SelectedIndex = 0;
            }
            
            if (row.Row.Table.Columns.Contains("HinhAnh") && row["HinhAnh"] != DBNull.Value)
            {
                _uploadedImageUrl = row["HinhAnh"].ToString();
                if (!string.IsNullOrEmpty(_uploadedImageUrl))
                {
                    try
                    {
                        imgSanPham.Source = new BitmapImage(new Uri(_uploadedImageUrl));
                    }
                    catch
                    {
                        imgSanPham.Source = null;
                    }
                }
                else
                {
                    imgSanPham.Source = null;
                }
            }
            else
            {
                _uploadedImageUrl = "";
                imgSanPham.Source = null;
            }
            
            _originalImageUrl = _uploadedImageUrl;
            crudPanel.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            HandleCancelForm();
        }

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Keeping event signature just in case
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HandleCancelForm();
        }

        private void OverlayDetail_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            detailPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnCloseDetail_Click(object sender, RoutedEventArgs e)
        {
            detailPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                txtDetailTenSanPham.Text = row["TenSanPham"].ToString();
                txtDetailMaSanPham.Text = "Mã SP: " + row["MaSanPham"].ToString();
                txtDetailDanhMuc.Text = row["TenDanhMuc"].ToString();
                txtDetailThuongHieu.Text = row["TenThuongHieu"].ToString();
                
                decimal gia = 0;
                if (decimal.TryParse(row["GiaNiemYet"].ToString(), out gia))
                {
                    txtDetailGia.Text = string.Format("{0:N0} đ", gia);
                }
                else
                {
                    txtDetailGia.Text = row["GiaNiemYet"].ToString();
                }
                
                txtDetailDonVi.Text = row["DonViTinh"].ToString();
                txtDetailNhaCungCap.Text = row["TenNhaCungCap"].ToString();
                
                string trangThai = row["TrangThaiStr"].ToString();
                txtDetailTrangThai.Text = trangThai;
                if (trangThai == "Đang bán")
                {
                    tagDetailTrangThai.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#E3FCEF");
                    tagDetailTrangThai.BorderBrush = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#36B37E");
                    txtDetailTrangThai.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#006644");
                }
                else
                {
                    tagDetailTrangThai.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FFE3E3");
                    tagDetailTrangThai.BorderBrush = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FA5252");
                    txtDetailTrangThai.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#C92A2A");
                }

                // Load image
                if (row.Row.Table.Columns.Contains("HinhAnh") && row["HinhAnh"] != DBNull.Value)
                {
                    string imgUrl = row["HinhAnh"].ToString();
                    if (!string.IsNullOrEmpty(imgUrl))
                    {
                        try
                        {
                            System.Windows.Media.Imaging.BitmapImage bitmap = new System.Windows.Media.Imaging.BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(imgUrl, UriKind.Absolute);
                            bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            imgDetailSanPham.Source = bitmap;
                        }
                        catch
                        {
                            imgDetailSanPham.Source = null;
                        }
                    }
                    else
                    {
                        imgDetailSanPham.Source = null;
                    }
                }
                else
                {
                    imgDetailSanPham.Source = null;
                }

                detailPanel.Visibility = Visibility.Visible;
            }
        }

        private void HandleCancelForm()
        {
            // Nếu có up ảnh mới nhưng lại ấn Hủy, thì phải xóa ảnh mồ côi trên Cloudinary đi
            if (!string.IsNullOrEmpty(_uploadedImageUrl) && _uploadedImageUrl != _originalImageUrl)
            {
                string urlToDelete = _uploadedImageUrl;
                _ = System.Threading.Tasks.Task.Run(() => new CloudinaryService().DeleteImageAsync(urlToDelete));
            }
            crudPanel.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            txtMaSanPham.Text = "";
            txtTenSanPham.Text = "";
            txtDonViTinh.Text = "Cái";
            txtGiaNiemYet.Text = "";
            txtMoTa.Text = "";
            
            cboDanhMuc.SelectedIndex = -1;
            cboThuongHieu.SelectedIndex = -1;
            cboNhaCungCap.SelectedIndex = -1;
            cboTrangThai.SelectedIndex = 0;
            _uploadedImageUrl = "";
            imgSanPham.Source = null;
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.PreviousPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _pager?.NextPage();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSanPham.Text) || string.IsNullOrWhiteSpace(txtTenSanPham.Text) || string.IsNullOrWhiteSpace(txtGiaNiemYet.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Mã sản phẩm, Tên sản phẩm và Giá niêm yết!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cboDanhMuc.SelectedValue == null || cboThuongHieu.SelectedValue == null || cboNhaCungCap.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Danh mục, Thương hiệu và Nhà cung cấp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string cleanGiaNiemYet = txtGiaNiemYet.Text.Replace(",", "").Replace(".", "").Trim();
                if (!decimal.TryParse(cleanGiaNiemYet, out decimal giaNiemYet) || giaNiemYet <= 0)
                {
                    MessageBox.Show("Giá niêm yết không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int maDanhMuc = Convert.ToInt32(cboDanhMuc.SelectedValue);
                int maThuongHieu = Convert.ToInt32(cboThuongHieu.SelectedValue);
                int maNhaCungCap = Convert.ToInt32(cboNhaCungCap.SelectedValue);
                bool trangThai = cboTrangThai.SelectedIndex == 0;
                string donVi = string.IsNullOrWhiteSpace(txtDonViTinh.Text) ? "Cái" : txtDonViTinh.Text.Trim();

                string hinhAnh = string.IsNullOrEmpty(_uploadedImageUrl) ? null : _uploadedImageUrl;

                if (!_isUpdateMode)
                {
                    _bus.Insert(txtMaSanPham.Text.Trim(), txtTenSanPham.Text.Trim(), maDanhMuc, maThuongHieu, maNhaCungCap, hinhAnh, donVi, giaNiemYet, trangThai);
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _bus.Update(txtMaSanPham.Text.Trim(), txtTenSanPham.Text.Trim(), maDanhMuc, maThuongHieu, maNhaCungCap, hinhAnh, donVi, giaNiemYet, trangThai);
                    MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Nếu cập nhật thành công và ảnh đã thay đổi, xóa ảnh cũ trên Cloudinary
                    if (!string.IsNullOrEmpty(_originalImageUrl) && _originalImageUrl != _uploadedImageUrl)
                    {
                        string urlToDelete = _originalImageUrl;
                        _ = System.Threading.Tasks.Task.Run(() => new CloudinaryService().DeleteImageAsync(urlToDelete));
                    }
                }
                
                // Đặt lại original để tránh bị xóa ở hàm Cancel
                _originalImageUrl = _uploadedImageUrl;
                crudPanel.Visibility = Visibility.Collapsed;
                LoadData();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UC_TenSanPham"))
                {
                    MessageBox.Show("Tên sản phẩm đã tồn tại, vui lòng chọn tên khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (ex.Message.Contains("PRIMARY KEY"))
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại, vui lòng chọn mã khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.DataContext is DataRowView row)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _bus.Delete(row["MaSanPham"].ToString());
                        MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Nếu sản phẩm bị xóa có ảnh, xóa luôn ảnh trên Cloudinary
                        if (row.Row.Table.Columns.Contains("HinhAnh") && row["HinhAnh"] != DBNull.Value)
                        {
                            string urlToDelete = row["HinhAnh"].ToString();
                            if (!string.IsNullOrEmpty(urlToDelete))
                            {
                                _ = System.Threading.Tasks.Task.Run(() => new CloudinaryService().DeleteImageAsync(urlToDelete));
                            }
                        }

                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp;*.bmp",
                Title = "Chọn ảnh sản phẩm"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var btn = sender as Button;
                    string oldContent = btn.Content.ToString();
                    btn.Content = "Đang tải lên...";
                    btn.IsEnabled = false;

                    CloudinaryService cloudinary = new CloudinaryService();
                    string url = await cloudinary.UploadImageAsync(openFileDialog.FileName);

                    if (!string.IsNullOrEmpty(url))
                    {
                        _uploadedImageUrl = url;
                        imgSanPham.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                        MessageBox.Show("Tải ảnh lên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể tải ảnh lên. Vui lòng kiểm tra lại cấu hình API Key.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                    btn.Content = oldContent;
                    btn.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    var btn = sender as Button;
                    btn.Content = "Tải ảnh lên";
                    btn.IsEnabled = true;
                }
            }
        }

        private void TxtCurrency_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string value = textBox.Text.Replace(",", "").Replace(".", "").Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    if (decimal.TryParse(value, out decimal amount))
                    {
                        textBox.TextChanged -= TxtCurrency_TextChanged;
                        textBox.Text = amount.ToString("N0");
                        textBox.CaretIndex = textBox.Text.Length;
                        textBox.TextChanged += TxtCurrency_TextChanged;
                    }
                }
            }
        }
    }
}
