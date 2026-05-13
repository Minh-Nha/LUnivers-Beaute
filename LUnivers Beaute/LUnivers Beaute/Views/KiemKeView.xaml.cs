using LUnivers_Beaute.Helpers;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class KiemKeView : UserControl
    {
        private PagingHelper _pager;
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
                KiemKeBUS bus = new KiemKeBUS();
                if (dgData != null) _pager.SetData(bus.GetAll());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
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

