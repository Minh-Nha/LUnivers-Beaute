using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

namespace LUnivers_Beaute.Services
{
    public class InvoiceItem
    {
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class InvoiceModel
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayTao { get; set; }
        public string TenNhanVien { get; set; }
        public string TenCuaHang { get; set; }
        public string DiaChiCuaHang { get; set; }
        public string SdtCuaHang { get; set; }
        
        public string TenKhachHang { get; set; }
        public string SdtKhachHang { get; set; }
        
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public decimal TamTinh { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongCong { get; set; }
        public decimal TienKhachDua { get; set; }
        public decimal TienThoi { get; set; }
        public string PhuongThucThanhToan { get; set; }
    }

    public class PdfInvoiceService
    {
        private readonly string TextColor = "#000000";
        private readonly string MutedColor = "#555555";
        private readonly string BorderColor = "#AAAAAA";

        public PdfInvoiceService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GenerateInvoice(InvoiceModel model, string filePath)
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

        private void ComposeReceipt(IContainer container, InvoiceModel model)
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
                column.Item().AlignCenter().Text("PHIẾU THANH TOÁN").FontSize(11).Bold();
                column.Item().PaddingTop(5).Column(c =>
                {
                    c.Item().Row(r => { r.ConstantItem(55).Text("Mã HĐ:").FontSize(8); r.RelativeItem().Text(model.MaHoaDon).FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("Ngày:").FontSize(8); r.RelativeItem().Text($"{model.NgayTao:dd/MM/yyyy HH:mm}").FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("Khách:").FontSize(8); r.RelativeItem().Text(string.IsNullOrEmpty(model.TenKhachHang) ? "Khách vãng lai" : model.TenKhachHang).FontSize(8); });
                    c.Item().Row(r => { r.ConstantItem(55).Text("Thu ngân:").FontSize(8); r.RelativeItem().Text(model.TenNhanVien).FontSize(8); });
                });

                column.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(BorderColor);

                // ITEMS & SUMMARY (USING TABLE FOR PERFECT ALIGNMENT)
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(70); // Cột số lượng/giá tiền cố định 70pt
                    });

                    // Table header
                    table.Cell().PaddingBottom(4).Text("SẢN PHẨM").FontSize(8).SemiBold();
                    table.Cell().PaddingBottom(4).AlignRight().Text("THÀNH TIỀN").FontSize(8).SemiBold();

                    // Items
                    foreach (var item in model.Items)
                    {
                        // Tên sản phẩm chiếm trọn 2 cột
                        table.Cell().ColumnSpan(2).Text(item.TenSanPham).FontSize(9);
                        
                        // Số lượng x Đơn giá
                        table.Cell().PaddingBottom(4).Text($"{item.SoLuong} x {item.DonGia:N0}").FontSize(8).FontColor(MutedColor);
                        // Thành tiền
                        table.Cell().PaddingBottom(4).AlignRight().Text($"{item.ThanhTien:N0}").FontSize(9);
                    }

                    // Divider
                    table.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(0.5f).LineColor(BorderColor);

                    // Summary
                    table.Cell().Text("Tạm tính:").FontSize(8);
                    table.Cell().AlignRight().Text($"{model.TamTinh:N0}").FontSize(8);

                    if (model.GiamGia > 0)
                    {
                        table.Cell().Text("Khuyến mãi:").FontSize(8);
                        table.Cell().AlignRight().Text($"-{model.GiamGia:N0}").FontSize(8);
                    }

                    table.Cell().PaddingTop(2).Text("TỔNG CỘNG:").FontSize(10).Bold();
                    table.Cell().PaddingTop(2).AlignRight().Text($"{model.TongCong:N0}").FontSize(11).Bold();

                    table.Cell().PaddingTop(4).Text("Khách đưa:").FontSize(8);
                    table.Cell().PaddingTop(4).AlignRight().Text($"{model.TienKhachDua:N0}").FontSize(8);

                    table.Cell().Text("Tiền thối:").FontSize(8);
                    table.Cell().AlignRight().Text($"{model.TienThoi:N0}").FontSize(8);
                });

                column.Item().PaddingTop(10).AlignCenter().Text($"PTTT: {model.PhuongThucThanhToan}").FontSize(8).FontColor(MutedColor).Italic();

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
