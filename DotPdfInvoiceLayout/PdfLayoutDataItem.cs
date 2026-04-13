using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.PdfDoc.Generating.Rendering;
using System.Data;
using Atalasoft.PdfDoc.Generating.Shapes;

namespace PDFInvoice
{
    public abstract class PdfLayoutDataItem : PdfLayoutItem , IPdfRenderable
    {
        public const string DrawGridLine = "DrawVerticalGridLine";
        public const string DrawText = "DrawText";
        public const string TextAlignmentProperty = "TextAlignment";
        public const string WidthProperty = "Width";


        private PdfTableColumnCollection _columns;
      //  public TableColumnCollection Columns { get; set; }
        public PdfTableColumnCollection Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
            }
        }
            
        public PdfTextAlignment TextAlignment { get; set; }

        public PdfLayoutDataItem()
        {
            //Columns = new List<LayoutItemColumn>();
        }

        protected int GetNumColumns()
        {
            return Columns.Count;
        }

        protected double CalculateHorizontalSpacer()
        {
            return (_width - LeftMargin - RightMargin) / GetNumColumns();
        }
        protected double CalculateNewXPositionBasedOnColumnWidth(double currentX, double columnWidth)
        {
            return currentX + columnWidth;
        }



        protected PdfTextAlignment GetTextAlignment(LayoutItemColumn dc)
        {
            if (dc.ExtendedProperties.ContainsKey(TextAlignmentProperty))
            {
                return (PdfTextAlignment)dc.ExtendedProperties[TextAlignmentProperty];
            }
            return TextAlignment;
        }

        protected string GetRowDataFontName(LayoutItemColumn dc)
        {
            if (dc.ExtendedProperties.ContainsKey("Font"))
            {
                return (string)dc.ExtendedProperties["Font"];
            }
            return RowDataFontName;
        }

        protected double GetColumnWidth(LayoutItemColumn dc)
        {
            double width = HorizontalSpacer;
            if (dc.ExtendedProperties.ContainsKey(_widthPropertyName))
            {
                width = double.Parse((string)dc.ExtendedProperties[_widthPropertyName]);
            }
            return width;
        }

        public  abstract void Render(PdfPageRenderer r);

    }
}
