using IMSBackend.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;

namespace IMSBackend.Services
{
    public class PdfService
    {
        public byte[] GenerateInvoicePdf(
            Invoice invoice)
        {
            using MemoryStream ms = new();

            PdfWriter writer =
                new PdfWriter(ms);

            PdfDocument pdf =
                new PdfDocument(writer);

            Document document =
                new Document(pdf);

            // =========================
            // TITLE
            // =========================

            document.Add(
             new Paragraph("IMS INVENTORY SYSTEM")
              .SetFontSize(22));

            document.Add(
                new Paragraph("Hyderabad, India"));

            document.Add(
                new Paragraph("GSTIN: 36ABCDE1234F1Z5"));

            document.Add(
                new Paragraph("Phone: +91 9876543210"));

            document.Add(new Paragraph(" "));

            document.Add(
                new Paragraph("Invoice")
                .SetFontSize(16));

            document.Add(new Paragraph(" "));

            // =========================
            // INVOICE DETAILS
            // =========================

            document.Add(
                new Paragraph(
                    $"Invoice Number: {invoice.InvoiceNumber}"));

            document.Add(
                new Paragraph(
                    $"Customer: {invoice.Customer?.Name}"));

            document.Add(
                new Paragraph(
                    $"Invoice Date: {invoice.InvoiceDate:dd-MM-yyyy}"));

            document.Add(new Paragraph(" "));

            // =========================
            // PRODUCT TABLE
            // =========================

            Table table =
                new Table(4);

            table.SetWidth(
                UnitValue.CreatePercentValue(100));

            // HEADER

            table.AddHeaderCell(
    new Cell().Add(
        new Paragraph("Product")));

            table.AddHeaderCell(
                new Cell().Add(
                    new Paragraph("Qty")));

            table.AddHeaderCell(
                new Cell().Add(
                    new Paragraph("Price")));

            table.AddHeaderCell(
                new Cell().Add(
                    new Paragraph("Total")));

            // DYNAMIC DATA

            foreach (var item in invoice.InvoiceItems!)
            {
                table.AddCell(
                    item.Product?.Name ?? "Product");

                table.AddCell(
                    item.Quantity.ToString());

                table.AddCell(
                    item.Price.ToString());

                table.AddCell(
                    item.Total.ToString());
            }

            document.Add(table);

            document.Add(new Paragraph(" "));

            // =========================
            // TOTALS
            // =========================

            decimal gst =
    invoice.TotalAmount * 0.18m;

            decimal finalAmount =
                invoice.TotalAmount + gst;

            document.Add(
    new Paragraph(
        $"Sub Total: ₹{invoice.TotalAmount:0.00}"));

            document.Add(
                new Paragraph(
                    $"GST (18%): ₹{gst:0.00}"));

            document.Add(
                new Paragraph(
                    $"Grand Total: ₹{finalAmount:0.00}")
                .SetFontSize(16));

            // =========================
            // FOOTER
            // =========================

            document.Add(
                new Paragraph(
                    "Thank you for doing business with IMS."));

            document.Close();

            return ms.ToArray();
        }
    }
}