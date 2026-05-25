using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

namespace LUnivers_Beaute.Services
{
    public class ImportReceiptItem
    {
        public string TenSanPham { get; set; }
        public string SoLo { get; set; }
        public string HanSuDung { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal ThanhTien => SoLuong * GiaNhap;
    }

    public class ImportReceiptModel
    {
        public string MaPhieuNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        public string TenNhanVien { get; set; }
        public string TenCuaHang { get; set; }
        public List<ImportReceiptItem> Items { get; set; } = new List<ImportReceiptItem>();
        public decimal TongTienNhap { get; set; }
    }

    public class PdfImportReceiptService
    {
        private readonly string TextColor = "#000000";
        private readonly string MutedColor = "#555555";
        private readonly string BorderColor = "#AAAAAA";

        public PdfImportReceiptService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GenerateReceipt(ImportReceiptModel model, string filePath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial).FontColor(TextColor));

                    page.Header().Element(c => ComposeHeader(c, model));
                    page.Content().Element(c => ComposeContent(c, model));
                    page.Footer().Element(ComposeFooter);
                });
            })
            .GeneratePdf(filePath);
        }

        private void ComposeHeader(IContainer container, ImportReceiptModel model)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("L'UNIVERS BEAUTÉ").FontSize(20).SemiBold().FontColor("#D98A7B");
                    column.Item().Text("HỆ THỐNG PHÂN PHỐI MỸ PHẨM CHÍNH HÃNG").FontSize(10).FontColor(MutedColor);
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("PHIẾU NHẬP KHO").FontSize(20).Bold().AlignRight();
                    column.Item().Text($"Mã phiếu: {model.MaPhieuNhap}").FontSize(10).AlignRight();
                    column.Item().Text($"Ngày nhập: {model.NgayNhap:dd/MM/yyyy HH:mm}").FontSize(10).AlignRight();
                });
            });
        }

        private void ComposeContent(IContainer container, ImportReceiptModel model)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(20);

                // Info Section
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("THÔNG TIN NHẬP KHO").SemiBold().FontSize(12).Underline();
                        c.Item().PaddingTop(5).Text($"Nhân viên lập phiếu: {model.TenNhanVien}");
                        c.Item().Text($"Cửa hàng nhận: {model.TenCuaHang}");
                    });
                });

                // Table Section
                column.Item().Element(c => ComposeTable(c, model));

                // Summary Section
                column.Item().Row(row =>
                {
                    row.RelativeItem();
                    row.ConstantItem(250).Column(c =>
                    {
                        c.Item().BorderBottom(1).BorderColor(BorderColor).PaddingBottom(5).Row(r =>
                        {
                            r.RelativeItem().Text("TỔNG TIỀN NHẬP:").Bold().FontSize(12);
                            r.RelativeItem().AlignRight().Text($"{model.TongTienNhap:N0} đ").Bold().FontSize(14).FontColor("#D98A7B");
                        });
                    });
                });
                
                // Signatures
                column.Item().PaddingTop(30).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Column(c =>
                    {
                        c.Item().Text("Người lập phiếu").Bold();
                        c.Item().Text("(Ký, ghi rõ họ tên)").FontSize(9).FontColor(MutedColor);
                        c.Item().PaddingTop(60).Text(model.TenNhanVien).SemiBold();
                    });
                    
                    row.RelativeItem().AlignCenter().Column(c =>
                    {
                        c.Item().Text("Thủ kho / Đại diện cửa hàng").Bold();
                        c.Item().Text("(Ký, ghi rõ họ tên)").FontSize(9).FontColor(MutedColor);
                    });
                });
            });
        }

        private void ComposeTable(IContainer container, ImportReceiptModel model)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);     // STT
                    columns.RelativeColumn(4);      // Sản phẩm
                    columns.RelativeColumn(1.2f);   // Lô
                    columns.RelativeColumn(1.8f);   // HSD
                    columns.ConstantColumn(35);     // SL
                    columns.RelativeColumn(2.2f);   // Đơn giá
                    columns.RelativeColumn(2.5f);   // Thành tiền
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Sản phẩm");
                    header.Cell().Element(CellStyle).Text("Lô");
                    header.Cell().Element(CellStyle).Text("HSD");
                    header.Cell().Element(CellStyle).AlignRight().Text("SL");
                    header.Cell().Element(CellStyle).AlignRight().Text("Đơn giá");
                    header.Cell().Element(CellStyle).AlignRight().Text("Thành tiền");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold().FontSize(10)).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                var index = 1;
                foreach (var item in model.Items)
                {
                    table.Cell().Element(CellStyle).Text($"{index++}").FontSize(10);
                    table.Cell().Element(CellStyle).Text(item.TenSanPham).FontSize(10);
                    table.Cell().Element(CellStyle).Text(item.SoLo).FontSize(10);
                    table.Cell().Element(CellStyle).Text(item.HanSuDung).FontSize(10);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.SoLuong}").FontSize(10);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.GiaNhap:N0}").FontSize(10);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.ThanhTien:N0}").FontSize(10);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Trang ").FontSize(10).FontColor(MutedColor);
                x.CurrentPageNumber().FontSize(10).FontColor(MutedColor);
                x.Span(" / ").FontSize(10).FontColor(MutedColor);
                x.TotalPages().FontSize(10).FontColor(MutedColor);
            });
        }
    }
}
