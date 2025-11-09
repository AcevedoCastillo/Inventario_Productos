using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Interfaces;
namespace SistemaVentas.API.Services
{
    public class ReporteService : IReporteService
    {
        public ReporteService()
        {
            // Configurar licencia de QuestPDF (Community)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerarReportePDFAsync(List<ReporteDetalladoVentaDTO> ventas, DateTime fechaInicio, DateTime fechaFin)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(40);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Encabezado
                        page.Header().Element(ComposeHeader);

                        // Contenido
                        page.Content().Element(content => ComposeContent(content, ventas, fechaInicio, fechaFin));

                        // Pie de página
                        page.Footer().Element(ComposeFooter);
                    });
                });

                return document.GeneratePdf();
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Sistema de Ventas").FontSize(20).Bold().FontColor(Colors.Red.Medium);
                    column.Item().Text("Reporte de Ventas").FontSize(14).SemiBold();
                    column.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Fecha de generación: ").SemiBold();
                        text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });
            });
        }

        private void ComposeContent(IContainer container, List<ReporteDetalladoVentaDTO> ventas, DateTime fechaInicio, DateTime fechaFin)
        {
            container.PaddingVertical(20).Column(column =>
            {
                // Período del reporte
                column.Item().Background(Colors.Grey.Lighten3).Padding(10).Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span("Período: ").SemiBold();
                        text.Span($"{fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}");
                    });

                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Total de registros: ").SemiBold();
                        text.Span(ventas.Count.ToString());
                    });
                });

                column.Item().PaddingTop(20);

                // Tabla de ventas
                column.Item().Table(table =>
                {
                    // Definir columnas
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);  // N° Venta
                        columns.ConstantColumn(60);  // Fecha
                        columns.RelativeColumn(2);   // Vendedor
                        columns.ConstantColumn(50);  // Código
                        columns.RelativeColumn(3);   // Producto
                        columns.ConstantColumn(40);  // Cant.
                        columns.ConstantColumn(60);  // Precio
                        columns.ConstantColumn(50);  // IVA
                        columns.ConstantColumn(60);  // Total
                    });

                    // Encabezado de tabla
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("N° Venta").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("Fecha").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("Vendedor").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("Código").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("Producto").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("Cant.").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("Precio").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("IVA").FontColor(Colors.White).SemiBold();
                        header.Cell().Element(CellStyle).Background(Colors.Red.Medium).Text("Total").FontColor(Colors.White).SemiBold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        }
                    });

                    // Datos
                    decimal totalGeneral = 0;

                    foreach (var venta in ventas)
                    {
                        totalGeneral += venta.TotalProducto;

                        table.Cell().Element(CellStyle).Text(venta.NoVenta.ToString());
                        table.Cell().Element(CellStyle).Text(venta.Fecha);
                        table.Cell().Element(CellStyle).Text(venta.Vendedor);
                        table.Cell().Element(CellStyle).Text(venta.Codigo);
                        table.Cell().Element(CellStyle).Text(venta.Producto);
                        table.Cell().Element(CellStyle).AlignCenter().Text(venta.Cantidad.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text($"${venta.PrecioUnitario:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"${venta.IVA:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"${venta.TotalProducto:N2}");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }

                    // Total
                    table.Cell().ColumnSpan(8).Element(TotalCellStyle).AlignRight().Text("TOTAL GENERAL:").Bold();
                    table.Cell().Element(TotalCellStyle).AlignRight().Text($"${totalGeneral:N2}").Bold().FontColor(Colors.Red.Medium);

                    static IContainer TotalCellStyle(IContainer container)
                    {
                        return container.Background(Colors.Grey.Lighten3).PaddingVertical(10).PaddingHorizontal(5);
                    }
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
            });
        }

        public async Task<byte[]> GenerarReporteExcelAsync(List<ReporteDetalladoVentaDTO> ventas, DateTime fechaInicio, DateTime fechaFin)
        {
            return await Task.Run(() =>
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Reporte de Ventas");

                    // Título
                    worksheet.Cells["A1:I1"].Merge = true;
                    worksheet.Cells["A1"].Value = "SISTEMA DE VENTAS - REPORTE DETALLADO";
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(220, 53, 69));
                    worksheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    // Información del reporte
                    worksheet.Cells["A2"].Value = "Período:";
                    worksheet.Cells["B2"].Value = $"{fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
                    worksheet.Cells["A3"].Value = "Fecha de generación:";
                    worksheet.Cells["B3"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells["A4"].Value = "Total de registros:";
                    worksheet.Cells["B4"].Value = ventas.Count;

                    worksheet.Cells["A2:A4"].Style.Font.Bold = true;

                    // Encabezados
                    int row = 6;
                    worksheet.Cells[row, 1].Value = "N° Venta";
                    worksheet.Cells[row, 2].Value = "Fecha";
                    worksheet.Cells[row, 3].Value = "Vendedor";
                    worksheet.Cells[row, 4].Value = "Código";
                    worksheet.Cells[row, 5].Value = "Producto";
                    worksheet.Cells[row, 6].Value = "Cantidad";
                    worksheet.Cells[row, 7].Value = "Precio Unitario";
                    worksheet.Cells[row, 8].Value = "IVA";
                    worksheet.Cells[row, 9].Value = "Total";

                    // Estilo de encabezados
                    using (var range = worksheet.Cells[row, 1, row, 9])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(220, 53, 69));
                        range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Datos
                    row = 7;
                    decimal totalGeneral = 0;

                    foreach (var venta in ventas)
                    {
                        worksheet.Cells[row, 1].Value = venta.NoVenta;
                        worksheet.Cells[row, 2].Value = venta.Fecha;
                        worksheet.Cells[row, 3].Value = venta.Vendedor;
                        worksheet.Cells[row, 4].Value = venta.Codigo;
                        worksheet.Cells[row, 5].Value = venta.Producto;
                        worksheet.Cells[row, 6].Value = venta.Cantidad;
                        worksheet.Cells[row, 7].Value = venta.PrecioUnitario;
                        worksheet.Cells[row, 8].Value = venta.IVA;
                        worksheet.Cells[row, 9].Value = venta.TotalProducto;

                        // Formato de moneda
                        worksheet.Cells[row, 7].Style.Numberformat.Format = "$#,##0.00";
                        worksheet.Cells[row, 8].Style.Numberformat.Format = "$#,##0.00";
                        worksheet.Cells[row, 9].Style.Numberformat.Format = "$#,##0.00";

                        // Bordes
                        worksheet.Cells[row, 1, row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        totalGeneral += venta.TotalProducto;
                        row++;
                    }

                    // Total
                    worksheet.Cells[row, 1, row, 8].Merge = true;
                    worksheet.Cells[row, 1].Value = "TOTAL GENERAL:";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, 9].Value = totalGeneral;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 9].Style.Font.Bold = true;
                    worksheet.Cells[row, 9].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(220, 53, 69));

                    // Fondo del total
                    worksheet.Cells[row, 1, row, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(248, 249, 250));
                    worksheet.Cells[row, 1, row, 9].Style.Border.BorderAround(ExcelBorderStyle.Medium);

                    // Ajustar anchos de columna
                    worksheet.Column(1).Width = 12;
                    worksheet.Column(2).Width = 12;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 12;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 15;
                    worksheet.Column(8).Width = 12;
                    worksheet.Column(9).Width = 15;

                    // Auto-filtro
                    worksheet.Cells[6, 1, row - 1, 9].AutoFilter = true;

                    return package.GetAsByteArray();
                }
            });
        }
    }
}
