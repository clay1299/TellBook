using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;

namespace TellBook
{
    static class Converter
    {
        /// <summary>
        /// Конвертирует файл json в файл PDF
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <param name="pdfFilePath"></param>
        public static void ConvertToPDF(string jsonFilePath, string pdfFilePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                JArray jsonArray = JArray.Parse(jsonContent);

                BaseFont baseFont = BaseFont.CreateFont(
                    "c:/windows/fonts/arial.ttf",
                    BaseFont.IDENTITY_H,
                    BaseFont.EMBEDDED
                );
                Font headerFont = new Font(baseFont, 16, Font.BOLD, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#7b9095")));
                Font cellFont = new Font(baseFont, 10, Font.NORMAL);

                using (Document doc = new Document(PageSize.A4, 10, 10, 10, 10))
                using (PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath, FileMode.Create)))
                {
                    doc.Open();

                    doc.Add(new iTextSharp.text.Paragraph("Список контактов", headerFont));
                    doc.Add(new iTextSharp.text.Paragraph("\n") { SpacingAfter = 20f });

                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;

                    table.AddCell(new Phrase("Имя", cellFont));
                    table.AddCell(new Phrase("Телефон", cellFont));
                    table.AddCell(new Phrase("Описание", cellFont));

                    foreach (JObject item in jsonArray)
                    {
                        table.AddCell(new Phrase(item["Name"]?.ToString() ?? "N/A", cellFont));
                        table.AddCell(new Phrase(item["Phone"]?.ToString() ?? "N/A", cellFont));
                        table.AddCell(new Phrase(item["Description"]?.ToString() ?? "N/A", cellFont));
                    }

                    doc.Add(table);
                    doc.Close();
                }
            }
            catch (FileNotFoundException)
            { throw new IOException("file just open"); }

            catch (IOException)
            { throw new IOException("error opening file"); }

            catch (Exception ex)
            { throw new Exception("unknown error: " + ex.Message); }
        }
        
        /// <summary>
        /// Конвертирует файл json в Excel таблицу
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <param name="xlsxFilePath"></param>
        public static void ConvertToExel(string jsonFilePath, string xlsxFilePath)
        {
            try
            {

                string jsonContent = File.ReadAllText(jsonFilePath);
                JArray jsonArray = JArray.Parse(jsonContent);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Контакты");

                    var headerStyle = workbook.Style;
                    headerStyle.Fill.BackgroundColor = XLColor.BlueGray;
                    headerStyle.Font.Bold = true;
                    headerStyle.Font.FontColor = XLColor.White;
                    headerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    if (jsonArray.Count > 0)
                    {
                        JObject firstItem = (JObject)jsonArray[0];
                        int column = 1;

                        foreach (var property in firstItem.Properties())
                        {
                            var cell = worksheet.Cell(1, column);
                            cell.Value = property.Name;
                            cell.Style = headerStyle;
                            column++;
                        }

                        int row = 2;
                        foreach (JObject item in jsonArray)
                        {
                            column = 1;
                            foreach (var property in item.Properties())
                            {
                                worksheet.Cell(row, column).Value = property.Value?.ToString();
                                column++;
                            }
                            row++;
                        }
                    }

                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(xlsxFilePath);
                }
            }
            catch (FileNotFoundException)
            { throw new IOException("file just open"); }

            catch (IOException)
            { throw new IOException("error opening file"); }

            catch (Exception ex)
            { throw new Exception("unknown error: " + ex.Message); }
        }
    }
}
