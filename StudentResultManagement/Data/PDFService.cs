using iText.Kernel.Pdf;
using iText.Layout.Properties;
using iText.Layout;
using iText.Layout.Element;
using StudentResultManagement.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StudentResultManagement.Data
{
    
        public class PDFService
        {
            public byte[] GeneratePDF(IEnumerable<Students> all_students, Dictionary<int, List<string>> result_data,
               List<string> coursesName, string seriesName, string semesterName)
            {
                //Define your memory stream which will temporarily hold the PDF
                using (MemoryStream stream = new MemoryStream())
                {
                    //Initialize PDF writer
                    PdfWriter writer = new PdfWriter(stream);
                    //Initialize PDF document
                    PdfDocument pdf = new PdfDocument(writer);
                    // Initialize document
                    Document document = new Document(pdf);
                    // Add content to the document
                    // Header
                    
                    document.Add(new Paragraph(seriesName+" series, " + semesterName+
                        " semester Result")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18));
                  
                

                // Invoice data
                //document.Add(new Paragraph($"Invoice Number: {invoice.InvoiceNumber}"));
                //document.Add(new Paragraph($"Date: {invoice.Date.ToShortDateString()}"));
                //document.Add(new Paragraph($"Customer Name: {invoice.CustomerName}"));
                //document.Add(new Paragraph($"Payment Mode: {invoice.PaymentMode}"));
                // Table for invoice items
                int cnt = coursesName.Count;
                cnt += 2;
                float[] fl = new float[cnt];
                for (int i = 0; i < cnt; i++)
                {
                    fl[i] = 1;
                }

                var table = new iText.Layout.Element.Table(fl);

                    table.SetWidth(UnitValue.CreatePercentValue(100));
                    table.AddHeaderCell("Student Id");
                    table.AddHeaderCell("Student Name");
                    foreach(var course in coursesName)
                    {
                        table.AddHeaderCell(course);
                    }
                //table.AddHeaderCell("Unit Price");
                //table.AddHeaderCell("Total");

               

                  foreach (var data in all_students)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(data.Roll)));
                        table.AddCell(new Cell().Add(new Paragraph(data.Name)));
                    //table.AddCell(new Cell().Add(new Paragraph(item.UnitPrice.ToString("C"))));
                    //table.AddCell(new Cell().Add(new Paragraph(item.TotalPrice.ToString("C"))));




                    foreach (KeyValuePair<int, List<string>> kvp in result_data)
                    {
                        if (kvp.Key == data.Id)
                        {
                            List<string> list = kvp.Value;

                            foreach (string rec in list)
                            {
                                table.AddCell(new Cell().Add(new Paragraph(rec)));

                            }
                        }

                    }



                }

               


                //Add the Table to the PDF Document
                document.Add(table);
                    // Total Amount
                    //document.Add(new Paragraph($"Total Amount: {invoice.TotalAmount.ToString("C")}")
                    //    .SetTextAlignment(TextAlignment.RIGHT));
                    // Close the Document
                    document.Close();
                    return stream.ToArray();
                }
            }

        }
}

