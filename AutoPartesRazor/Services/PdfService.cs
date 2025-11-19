using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
<<<<<<< HEAD
=======
using System.IO;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

namespace AutoPartesRazor.Services;

public class PdfService : IPdfService
{
    private readonly IWebHostEnvironment _environment;

    public PdfService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public byte[] GenerateOrderPdf(Order order)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Header con logo y datos de la empresa
                page.Header()
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingBottom(10)
                    .Row(row =>
                    {
                        // Columna izquierda: Logo y nombre de empresa
                        row.RelativeItem().Column(col =>
                        {
                            // Intentar cargar el logo
                            var logoPath = Path.Combine(_environment.WebRootPath, "img", "logoAzul.jpg");

                            if (File.Exists(logoPath))
                            {
                                col.Item().Width(80).Height(60).Image(logoPath);
                            }
                            else
                            {
                                // Si no existe la imagen, mostrar un placeholder
                                col.Item()
                                    .Height(60)
                                    .Width(60)
                                    .Background(Colors.Blue.Lighten3)
                                    .AlignCenter()
                                    .AlignMiddle()
                                    .Text("AP")
                                    .FontSize(24)
                                    .Bold()
                                    .FontColor(Colors.White);
                            }

                            col.Item().PaddingTop(5).Text("AutoPartes S.A")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            col.Item().Text("Repuestos y accesorios")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken1);
                        });

                        // Columna derecha: Número de pedido y fecha
                        row.ConstantItem(200).Column(col =>
                        {
                            col.Item().AlignRight().Text("COMPROBANTE DE PEDIDO")
                                .FontSize(10)
                                .Bold()
                                .FontColor(Colors.Grey.Darken2);

<<<<<<< HEAD
                            col.Item().PaddingTop(5).AlignRight().Text($"Pedido N° {order.Id}")
=======
                            col.Item().PaddingTop(5).AlignRight().Text($"Pedido N° {order.id}")
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                                .FontSize(16)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            col.Item().PaddingTop(2).AlignRight().Text($"Fecha: {order.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken1);
                        });
                    });

                // Content
                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Spacing(8);

                    // Información del cliente
                    col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(clientCol =>
                    {
                        clientCol.Item().Text("DATOS DEL CLIENTE")
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.Grey.Darken2);

                        clientCol.Item().PaddingTop(5).Row(clientRow =>
                        {
                            clientRow.RelativeItem().Column(c =>
                            {
                                c.Item().Text(text =>
                                {
                                    text.Span("Nombre: ").Bold().FontSize(10);
                                    text.Span(order.CustomerName).FontSize(10);
                                });
                                c.Item().Text(text =>
                                {
                                    text.Span("Email: ").Bold().FontSize(10);
                                    text.Span(order.CustomerEmail).FontSize(10);
                                });
                            });

                            clientRow.RelativeItem().Column(c =>
                            {
                                c.Item().Text(text =>
                                {
                                    text.Span("Dirección: ").Bold().FontSize(10);
                                    text.Span(order.ShippingAddress).FontSize(10);
                                });
                                c.Item().Text(text =>
                                {
                                    text.Span("Método de pago: ").Bold().FontSize(10);
                                    text.Span(order.PaymentMethod).FontSize(10);
                                });
                            });
                        });
                    });

                    // Espacio
                    col.Item().PaddingVertical(10);

                    // Encabezado de la tabla
                    col.Item().Background(Colors.Blue.Darken2).Padding(8).Row(row =>
                    {
                        row.RelativeItem().Text("Producto").Bold().FontColor(Colors.White).FontSize(11);
                        row.ConstantItem(80).AlignRight().Text("Precio u.").Bold().FontColor(Colors.White).FontSize(11);
                        row.ConstantItem(60).AlignCenter().Text("Cantidad").Bold().FontColor(Colors.White).FontSize(11);
                        row.ConstantItem(90).AlignRight().Text("Total").Bold().FontColor(Colors.White).FontSize(11);
                    });

                    // Items del pedido
                    var isAlternate = false;
                    foreach (var item in order.Items)
                    {
<<<<<<< HEAD
                        var productName = item.Product?.Name ?? $"Producto #{item.ProductId}";
=======
                        var productName = item.Product?.name ?? $"Producto #{item.ProductId}";
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                        var unitPrice = item.UnitPrice;
                        var totalPrice = unitPrice * item.Quantity;

                        var backgroundColor = isAlternate ? Colors.Grey.Lighten5 : Colors.White;
                        isAlternate = !isAlternate;

                        col.Item().Background(backgroundColor).Padding(8).Row(row =>
                        {
                            row.RelativeItem().Text(productName).FontSize(10);
                            row.ConstantItem(80).AlignRight().Text(unitPrice.ToString("C")).FontSize(10);
                            row.ConstantItem(60).AlignCenter().Text(item.Quantity.ToString()).FontSize(10);
                            row.ConstantItem(90).AlignRight().Text(totalPrice.ToString("C")).FontSize(10).Bold();
                        });
                    }

                    // Total
                    col.Item().PaddingTop(10).AlignRight().Column(totalCol =>
                    {
                        totalCol.Item().Background(Colors.Blue.Darken2).Padding(10).Row(totalRow =>
                        {
                            totalRow.ConstantItem(100).Text("TOTAL:").Bold().FontColor(Colors.White).FontSize(14);
                            totalRow.ConstantItem(120).AlignRight().Text(order.Total.ToString("C"))
                                .Bold()
                                .FontColor(Colors.White)
                                .FontSize(16);
                        });
                    });
                });

                // Footer
                page.Footer()
                    .BorderTop(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingTop(10)
                    .AlignCenter()
                    .Column(col =>
                    {
                        col.Item().Text("Gracias por su compra")
                            .FontSize(10)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);

                        col.Item().Text("AutoPartes S.A. - Todos los derechos reservados")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium);
                    });
            });
        });

        return document.GeneratePdf();
    }
}