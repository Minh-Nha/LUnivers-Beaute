using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System;
using BUS;

namespace LUnivers_Beaute.Views
{
    public partial class DashboardView : UserControl
    {
        private HoaDonBUS _bus = new HoaDonBUS();
        private ThongKeBUS _thongKeBUS = new ThongKeBUS();

        public DashboardView()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (dgData != null) dgData.ItemsSource = _bus.GetDonHangGanDay(5).DefaultView;
                if (icTopKhachHang != null) icTopKhachHang.ItemsSource = _thongKeBUS.GetTopKhachHangMuaNhieuNhat();
                DrawWavyChart();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu Dashboard: " + ex.Message);
            }
        }

        private void DrawWavyChart()
        {
            if (ChartCanvas == null || XAxisLabels == null) return;
            
            var data = _thongKeBUS.GetDoanhThu7NgayQua();
            if (data == null || data.Count == 0) return;

            ChartCanvas.Children.Clear();
            XAxisLabels.Children.Clear();

            // Kích thước thật của Canvas sẽ được cập nhật sau khi Loaded, dùng ActualWidth
            double canvasWidth = ChartCanvas.ActualWidth > 0 ? ChartCanvas.ActualWidth : 600; 
            double canvasHeight = 180;
            double maxDoanhThu = (double)(data.Max(d => d.DoanhThu));
            if (maxDoanhThu == 0) maxDoanhThu = 1;

            double stepX = canvasWidth / (data.Count > 1 ? data.Count - 1 : 1);

            PathFigure figure = new PathFigure();
            // Start point
            double startY = canvasHeight - ((double)data[0].DoanhThu / maxDoanhThu) * canvasHeight;
            figure.StartPoint = new Point(0, startY);

            // Draw Bezier curves for smooth wave
            for (int i = 1; i < data.Count; i++)
            {
                double prevX = (i - 1) * stepX;
                double prevY = canvasHeight - ((double)data[i - 1].DoanhThu / maxDoanhThu) * canvasHeight;
                double currX = i * stepX;
                double currY = canvasHeight - ((double)data[i].DoanhThu / maxDoanhThu) * canvasHeight;

                // Control points for bezier (smooth curve)
                Point cp1 = new Point(prevX + (currX - prevX) / 2, prevY);
                Point cp2 = new Point(prevX + (currX - prevX) / 2, currY);

                figure.Segments.Add(new BezierSegment(cp1, cp2, new Point(currX, currY), true));
            }

            // Path for the line
            PathGeometry geometryLine = new PathGeometry();
            geometryLine.Figures.Add(figure);
            
            Path pathLine = new Path
            {
                Data = geometryLine,
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8956D")),
                StrokeThickness = 3,
                StrokeLineJoin = PenLineJoin.Round
            };

            // Area under curve
            PathFigure areaFigure = new PathFigure { StartPoint = new Point(0, canvasHeight) };
            areaFigure.Segments.Add(new LineSegment(new Point(0, startY), false));
            for (int i = 1; i < data.Count; i++)
            {
                double prevX = (i - 1) * stepX;
                double prevY = canvasHeight - ((double)data[i - 1].DoanhThu / maxDoanhThu) * canvasHeight;
                double currX = i * stepX;
                double currY = canvasHeight - ((double)data[i].DoanhThu / maxDoanhThu) * canvasHeight;

                Point cp1 = new Point(prevX + (currX - prevX) / 2, prevY);
                Point cp2 = new Point(prevX + (currX - prevX) / 2, currY);

                areaFigure.Segments.Add(new BezierSegment(cp1, cp2, new Point(currX, currY), false));
            }
            areaFigure.Segments.Add(new LineSegment(new Point((data.Count - 1) * stepX, canvasHeight), false));
            areaFigure.IsClosed = true;

            PathGeometry geometryArea = new PathGeometry();
            geometryArea.Figures.Add(areaFigure);
            
            Path pathArea = new Path
            {
                Data = geometryArea,
                Fill = new SolidColorBrush(Color.FromArgb(50, 232, 149, 109)) // Transparent orange
            };

            ChartCanvas.Children.Add(pathArea);
            ChartCanvas.Children.Add(pathLine);

            // Add dots & Labels
            for (int i = 0; i < data.Count; i++)
            {
                double x = i * stepX;
                double y = canvasHeight - ((double)data[i].DoanhThu / maxDoanhThu) * canvasHeight;

                Ellipse dot = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.White,
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8956D")),
                    StrokeThickness = 2,
                    ToolTip = $"{data[i].LabelNgay}: {data[i].DoanhThu:N0}đ"
                };
                Canvas.SetLeft(dot, x - 5);
                Canvas.SetTop(dot, y - 5);
                ChartCanvas.Children.Add(dot);

                XAxisLabels.Children.Add(new TextBlock 
                { 
                    Text = data[i].LabelNgay, 
                    FontSize = 11, 
                    Foreground = Brushes.Gray, 
                    HorizontalAlignment = HorizontalAlignment.Center 
                });
            }
        }
    }
}
