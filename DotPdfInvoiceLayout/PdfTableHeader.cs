using System;
using System.Collections.Generic;

using System.Text;

using Atalasoft.PdfDoc.Generating;
using Atalasoft.PdfDoc.Generating.Rendering;
using Atalasoft.PdfDoc.Geometry;
using Atalasoft.PdfDoc.Generating.Shapes;
using System.Data;

namespace PDFInvoice
{
    public class PdfTableHeader : PdfLayoutDataItem, IPdfRenderable
    {

        private static IPdfColor _headerFillColor = PdfColorFactory.FromColor(System.Drawing.Color.LightGray);
        private static IPdfColor _textFillColor = PdfColorFactory.FromColor(System.Drawing.Color.Black);
        private static IPdfColor _headerBoxFillColor = PdfColorFactory.FromColor(System.Drawing.Color.Black);


        public PdfTableHeader(PdfGeneratedDocument pdf, PdfTableColumnCollection columns)
        {
            Init(pdf, columns);
        }


        public PdfTableHeader(PdfGeneratedDocument pdf, DataColumnCollection dc)
        {
            Init(pdf, new PdfTableColumnCollection(dc));
        }

        private void Init(PdfGeneratedDocument pdf, PdfTableColumnCollection columns)
        {
            _name = "TableHeaderItem";
            _pdf = pdf;

            HeaderFontName = "Times New Roman Bold";
            RowDataFontName = "Times New Roman";
            FontSize = 10;
            LeftMargin = 20;
            RightMargin = 20;
            TopMargin = 50;
            BottomMargin = 50;
            HorizontalSpacer = 0;
            VerticalSpacer = 20;
            Position = new PdfPoint(0, 0);
            Bounds = new PdfBounds(Position.X, Position.Y, HorizontalSpacer, VerticalSpacer);
            Height = Bounds.Height;

            // by default, headers are Center Aligned
            TextAlignment = PdfTextAlignment.Center;
            Columns = columns;
        }

        public PdfTableHeader Clone()
        {
            PdfTableHeader newTable = new PdfTableHeader(_pdf, this.Columns);
            newTable.Bounds = this.Bounds;
            newTable.BottomMargin = this.BottomMargin;
            newTable.FontSize = this.FontSize;
            newTable.HeaderFontName = this.HeaderFontName;
            newTable.HorizontalSpacer = this.HorizontalSpacer;
            newTable.LeftMargin = this.LeftMargin;
            newTable.PageHeight = this.PageHeight;
            newTable.Position = this.Position;
            newTable.RightMargin = this.RightMargin;
            newTable.RowDataFontName = this.RowDataFontName;
            newTable.TextAlignment = this.TextAlignment;
            newTable.TopMargin = this.TopMargin;
            newTable.VerticalSpacer = this.VerticalSpacer;
            return newTable;
        }

        #region IPdfRenderable Members
        
        public override void Render(PdfPageRenderer r)
        {
            DrawTableHeader(r);
        }
        
        private void DrawTableHeader(PdfPageRenderer w)
        {
            // the spacer depends on how many columns there are
            HorizontalSpacer = CalculateHorizontalSpacer();
            double currentX = CalculateStartingX();
            double bottom = Position.Y;
            for (int j = 0; j < GetNumColumns(); j++)
            {
                double columnWidth = GetColumnWidth(Columns[j]);
                PdfRectangle headerBox = CreateHeaderBox(currentX, bottom, columnWidth);
                headerBox.FillColor = _headerFillColor;
                headerBox.Render(w);

                PdfTextBox headerText = CreateTextBox(currentX, bottom, Columns[j].ColumnName, columnWidth);
                headerText.Render(w);

                // move the X coordinate over the value of the width so it will go to the next column
                currentX = CalculateNewXPositionBasedOnColumnWidth(currentX, columnWidth);
            }
        }
       

        private PdfRectangle CreateHeaderBox(double left, double bottom, double width)
        {
            return new PdfRectangle(GetBoundsForColumn(left, bottom, width), _headerBoxFillColor, 1);
        }

        private PdfBounds GetBoundsForColumn(double left, double bottom, double width)
        {
            return (new PdfBounds(left, bottom, width, VerticalSpacer));
        }

        private PdfTextBox CreateTextBox(double left, double bottom, string text, double width)
        {
            PdfTextBox text1 = new PdfTextBox(GetBoundsForColumn(left, bottom, width), GetResourceName(HeaderFontName, _pdf.Resources), FontSize);
            text1.Text = text;                         
            text1.OutlineColor = _textFillColor;
            text1.FillColor = _textFillColor;
            text1.Alignment =  TextAlignment;
            return text1;
        }

        #endregion
    }
}
