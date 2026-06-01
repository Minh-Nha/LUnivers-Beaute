using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

namespace LUnivers_Beaute.Services
{
    public class ReturnItem
    {
        public string TenSanPham { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class ReturnModel
    {
        public string MaPhieuTra { get; set; } = "";
        public string MaHoaDonGoc { get; set; } = "";
        public DateTime NgayTra { get; set; }
        public string TenNhanVien { get; set; } = "";
        public string TenCuaHang { get; set; } = "";
        public string DiaChiCuaHang { get; set; } = "";
        public string SdtCuaHang { get; set; } = "";
        public string LyDoTra { get; set; } = "";
        public string TinhTrang { get; set; } = "";
        
        public List<ReturnItem> Items { get; set; } = new List<ReturnItem>();
        public decimal TongTienHoan { get; set; }
    }

    public class PdfReturnService
    {
        private readonly string TextColor = "#000000";
        private readonly string MutedColor = "#555555";
        private readonly string BorderColor = "#AAAAAA";

        public PdfReturnService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GenerateReturnReceipt(ReturnModel model, string filePath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.ContinuousSize(80, Unit.Millimetre);
                    page.Margin(10, Unit.Point);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).FontColor(TextColor));

                    page.Content().Element(c => ComposeReceipt(c, model));
                });
            })
            .GeneratePdf(filePath);
        }

        private void ComposeReceipt(IContainer container, ReturnModel model)
        {
            container.Column(column =>
            {
                // HEADER
                column.Item().AlignCenter().Column(c =>
                {
                    c.Item().Text("L'UNIVERS BEAUTÉ")
                        .FontSize(14)
                        .Bold()
                        .AlignCenter();
                        
                    c.Item().PaddingTop(2).Text(model.TenCuaHang).FontSize(9).AlignCenter();
                    
                    if (!string.IsNullOrEmpty(model.DiaChiCuaHang))
                        c.Item().Text(model.DiaChiCuaHang).FontSize(8).FontColor(MutedColor).AlignCenter();
                    if (!string.IsNullOrEmpty(model.SdtCuaHang))
                        c.Item().Text($"Tel: {model.SdtCuaHang}").FontSize(8).FontColor(MutedColor).AlignCenter();
                });

                column.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(BorderColor);

                // TITLE & INFO
                column.Item().AlignCenter().Text("PHIẾU TRẢ HÀNG").FontSize(11).Bold();
                column.Item().PaddingTop(5).Column(c =>
                {
                    c.Item().Row(r => { r.ConstantItem(55).Text("Mã phiếu:").FontSize(8); r.RelativeItem().Text(model.MaPhieuTra).FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("Ngày trả:").FontSize(8); r.RelativeItem().Text($"{model.NgayTra:dd/MM/yyyy HH:mm}").FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("HĐ gốc:").FontSize(8); r.RelativeItem().Text(model.MaHoaDonGoc).FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("Lý do:").FontSize(8); r.RelativeItem().Text(model.LyDoTra).FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("Tình trạng:").FontSize(8); r.RelativeItem().Text(model.TinhTrang).FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("Thu ngân:").FontSize(8); r.RelativeItem().Text(model.TenNhanVien).FontSize(8); });
                });

                column.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(BorderColor);

                // ITEMS & SUMMARY
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(70); // Cột số lượng/giá tiền cố định 70pt
                    });

                    // Table header
                    table.Cell().PaddingBottom(4).Text("SẢN PHẨM").FontSize(8).SemiBold();
                    table.Cell().PaddingBottom(4).AlignRight().Text("TIỀN HOÀN").FontSize(8).SemiBold();

                    // Items
                    foreach (var item in model.Items)
                    {
                        table.Cell().ColumnSpan(2).Text(item.TenSanPham).FontSize(9);
                        table.Cell().PaddingBottom(4).Text($"{item.SoLuong} x {item.DonGia:N0}").FontSize(8).FontColor(MutedColor);
                        table.Cell().PaddingBottom(4).AlignRight().Text($"{item.ThanhTien:N0}").FontSize(9);
                    }

                    // Divider
                    table.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(0.5f).LineColor(BorderColor);

                    // Summary
                    table.Cell().PaddingTop(2).Text("TỔNG HOÀN TRẢ:").FontSize(10).Bold();
                    table.Cell().PaddingTop(2).AlignRight().Text($"{model.TongTienHoan:N0}").FontSize(11).Bold();
                });

                column.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(BorderColor);

                // FOOTER
                column.Item().AlignCenter().Column(c =>
                {
                    c.Item().AlignCenter().Text("CẢM ƠN QUÝ KHÁCH").FontSize(9).SemiBold();
                    c.Item().AlignCenter().Text("HẸN GẶP LẠI!").FontSize(9).SemiBold();
                    c.Item().PaddingTop(4).AlignCenter().Text("Hotline CSKH: 1900 xxxx").FontSize(8).FontColor(MutedColor);
                });
            });
        }
    }
}
