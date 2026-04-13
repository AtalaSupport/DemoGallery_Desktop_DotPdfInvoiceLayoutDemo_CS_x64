using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.PdfDoc.Generating;
using System.Data;
using Atalasoft.PdfDoc.Generating.Shapes;

namespace PDFInvoice
{
    public class PdfLineItemTable : PdfTable
    {

        private static int _lineTotalColumnNumber = 0;

        public PdfLineItemTable(PdfGeneratedDocument doc)
        {
            _pdf = doc;
        }

        public void AddSubtotalRow(int lineTotalColumnNumber)
        {
            _lineTotalColumnNumber = lineTotalColumnNumber;
            // for the line item table we want to add 2 rows, one for subtotal , and one for total
            double subtotal = CalculateSubtotal(lineTotalColumnNumber);
            double salesTax = 0;
            
            this.AddRow(new string[] { "", "", "", "Subtotal:\nSales Tax:\nTotal:\n", subtotal.ToString("c") + "\n" + salesTax.ToString("c") + "\n" + subtotal.ToString("c") + "\n" });
            SetColumnViewProperties(_rows[_rows.Count - 1]);
            this.ShouldAddBorder = false;
        }

        /// <summary>
        /// loop through the table rows and sum up the column        
        /// </summary>
        /// <param name="subtotalColumnIndex"></param>
        /// <returns></returns>
 
        private  double CalculateSubtotal(int subtotalColumnIndex)
        {
            double subtotal = 0;
            foreach (PdfTableRow subRow in this.Rows)
            {
                double linePrice = 0;
                string temp = subRow.GetColumnText(subtotalColumnIndex);
                bool isLineTotal = double.TryParse(temp.Replace("$", ""), out linePrice);
                if (isLineTotal)
                {
                    subtotal += linePrice;
                }
            }
            return subtotal;
        }

        private  void SetColumnViewProperties(PdfTableRow row)
        {
            row.SetRowColumnProperty(0,PdfTableRow.DrawGridLine, false);
            row.SetRowColumnProperty(0,PdfTableRow.DrawText, false);
            row.SetRowColumnProperty(1,PdfTableRow.DrawGridLine, false);
            row.SetRowColumnProperty(1,PdfTableRow.DrawText, false);
            row.SetRowColumnProperty(2,PdfTableRow.DrawGridLine, false);
            row.SetRowColumnProperty(2,PdfTableRow.DrawText, false);
            row.SetRowColumnProperty(3,PdfTableRow.DrawGridLine, false);
            row.SetRowColumnProperty(3,"Font", "Times New Roman Bold");
            row.SetRowColumnProperty(4,"Font", "Times New Roman Bold");
        }
    }
}