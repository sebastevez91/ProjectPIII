using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace AutoPartesRazor.Services;

public class PdfService : IPdfService
{
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

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("AutoPartes").FontSize(18).Bold();
                            col.Item().Text("Comprobante de pedido").FontSize(12).SemiBold().FontColor(Colors.Grey.Darken1);
                        });
                        row.ConstantItem(100).AlignRight().Text($"Pedido #{order.id}").FontSize(12).SemiBold();
                    });

                page.Content().Column(col =>
                {
                    col.Spacing(5);

                    col.Item().Text($"Fecha: {order.CreatedAt.ToLocalTime():g}").FontSize(10);
                    col.Item().Text($"Cliente: {order.CustomerName}").FontSize(10);
                    col.Item().Text($"Email: {order.CustomerEmail}").FontSize(10);
                    col.Item().Text($"Dirección: {order.ShippingAddress}").FontSize(10);

                    col.Item().PaddingVertical(10).Element(Container =>
                    {
                        Container.Row(row =>
                        {
                            row.RelativeItem().Text("Producto").Bold();
                            row.ConstantItem(80).AlignRight().Text("Precio u.").Bold();
                            row.ConstantItem(60).AlignCenter().Text("Cantidad").Bold();
                            row.ConstantItem(80).AlignRight().Text("Total").Bold();
                        });
                    });

                    foreach (var it in order.Items)
                    {
                        var name = it.Product?.name ?? $"Producto {it.ProductId}";
                        var unit = it.UnitPrice;
                        var total = unit * it.Quantity;

                        col.Item().PaddingVertical(4).Element(Container =>
                        {
                            Container.Row(row =>
                            {
                                row.RelativeItem().Text(name);
                                row.ConstantItem(80).AlignRight().Text(unit.ToString("C"));
                                row.ConstantItem(60).AlignCenter().Text(it.Quantity.ToString());
                                row.ConstantItem(80).AlignRight().Text(total.ToString("C"));
                            });
                        });
                    }
                });

                page.Footer()
                    .AlignRight()
                    .Column(col =>
                    {
                        col.Item().Text($"Método de pago: {order.PaymentMethod}").FontSize(10);
                        col.Item().Text($"Total: {order.Total.ToString("C")}").FontSize(12).Bold();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
