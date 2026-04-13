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
    public class PdfTableRow : PdfLayoutDataItem
    {
        private PdfTable _parentTable = null;
        private object[] _data;
        private static IPdfColor _lineFillColor = PdfColorFactory.FromColor(System.Drawing.Color.Black);
        private static IPdfColor _textFillColor = PdfColorFactory.FromColor(System.Drawing.Color.Black);
        public double TextPaddingLeft { get; set; }
        public double TextPaddingRight { get; set; }
        public bool ShouldAddGridLines { get; set; }
        public bool IsLastRow { get; set; }
               
        private StyleTextInput _textStyle;

        public void SetRowText(StyleTextInput text1)
        {
            _textStyle = text1;
        }

        public PdfTableRow(PdfGeneratedDocument pdf, PdfTableColumnCollection cols, object[] data, PdfTable parentTable)
        {
            Init(pdf, cols, data, parentTable);
        }

        public PdfTableRow(PdfGeneratedDocument pdf, PdfTableColumnCollection cols, DataRow data, PdfTable parentTable)
        {
            Init( pdf,  cols,  data.ItemArray,  parentTable);
        }
        public PdfTableRow(PdfGeneratedDocument pdf,DataColumnCollection cols,DataRow data,PdfTable parentTable)
        {
            Init(pdf, new PdfTableColumnCollection(cols), data.ItemArray, parentTable);
        }

        private void Init(PdfGeneratedDocument pdf, PdfTableColumnCollection cols, object[] data, PdfTable parentTable)
        {
            _name = "TableRowShape";
            _parentTable = parentTable;
            _pdf = pdf;
            _data = data;
            Columns = cols.Clone();
            HeaderFontName = "Times New Roman";
            RowDataFontName = "Times New Roman";
            FontSize = 10;
            LeftMargin = 20;
            RightMargin = 20;
            TopMargin = 50;
            BottomMargin = 50;
            TextPaddingLeft = 0;
            HorizontalSpacer = 0;
            VerticalSpacer = 20;
            Position = new PdfPoint(LeftMargin, 0);
            Bounds = GetInitialBounds();
            TextAlignment = PdfTextAlignment.Center;
            ShouldAddGridLines = true;

            Height = Bounds.Height;
        }

        #region IPdfRenderable Members
        /// <summary>
        /// need to figure out what the height would be for the table, it should be the height of the 
        /// row with the most lines, because each extra line makes the table taller.
        /// </summary>
        /// <returns></returns>
        private PdfBounds GetInitialBounds()
        {
            int largestCountOfLines = 1;
            for (int i = 0; i < GetNumColumns(); i++)
            {
                string text = _data[i] + "";
                int count = new System.Text.RegularExpressions.Regex("\n").Matches(text).Count  + 1;
                if (count > largestCountOfLines)
                {
                    largestCountOfLines = count;
                }
            }
            return new PdfBounds(Position.X, Position.Y, HorizontalSpacer, largestCountOfLines * VerticalSpacer);
        }

        public override void Render(PdfPageRenderer r)
        {
            DrawTableRow(r);
        }

        public string GetColumnText(int columnIndex)
        {
            return Convert.ToString(_data[columnIndex]);
        }

        public void SetRowColumnProperty(int columnIndex, string propName, object propValue)
        {
            Columns[columnIndex].ExtendedProperties[propName] = propValue;
        }

        // Take the starting position Y - the Height of the Header Row(Vertical Spacer) - the height of the row to get to the bottom
        private double CalculateStartingYPosition()
        {
            return (Position.Y );//- Bounds.Height);
        }

        private void DrawTableRow(PdfPageRenderer w)
        {
            // the spacer depends on how many columns there are
            HorizontalSpacer = CalculateHorizontalSpacer();
            double currentX = CalculateStartingX();

            // initialize the value for the Y for the bottom line to the Y position for the row
            double textLineY = CalculateStartingYPosition();
            for (int i = 0; i < GetNumColumns(); i++)
            {
                LayoutItemColumn currentColumn = Columns[i];
                double columnWidth = GetColumnWidth(currentColumn);
                string textToShow = GetColumnText(i);

                // see if we should draw the gridlines based on the column properties
                if (ShouldDrawGridLines(currentColumn))
                {
                    PdfBounds gridLineBounds = GetGridLineBounds(currentX, textLineY, textToShow, columnWidth);

                    // get the pdfPath object for the Vertical gridlines for the row and render it
                    GetVerticalLine(gridLineBounds).Render(w);

                    // if its the last column, add the right side vertical grid line of the table
                    if (IsLastColumn(i))
                    {
                        PdfBounds colRectangleBounds = GetGridLineBounds(currentX + columnWidth, textLineY, textToShow, columnWidth);
                        GetVerticalLine(colRectangleBounds).Render(w);                        
                    }

                    if (ShouldDrawGridLines(currentColumn) )
                    {
                        PdfBounds rowGridBounds = GetBottomLineGridLineBounds(currentX, textLineY, columnWidth);
                        GetHorizontalLine(rowGridBounds).Render(w);
                    }
                }
                PdfTextAlignment textAlignment = GetTextAlignment(currentColumn);

                // check to see if we should add the row/columns text 
                if (ShouldDrawText(currentColumn))
                {
                    double textXPosition = GetXPositionWithPadding(currentX, textAlignment);
                    PdfBounds colTextBoxBounds = GetRowBounds(textXPosition, textLineY, textToShow, columnWidth);
                    GetTextBox(colTextBoxBounds, textToShow, textAlignment,currentColumn).Render(w);
                }
                

                // check to see if we need to make the horizontal line for the row, starts from the far left of the document and moves to the right side
                // there is a table wide ShouldAddGridLines property and a AddGridLines property for the column
                if (this.IsLastRow && _parentTable.ShouldAddBorder)
                {
                    PdfBounds rowGridBounds = GetBottomLineGridLineBounds(currentX, textLineY, columnWidth);
                    GetHorizontalLine(rowGridBounds).Render(w);
                }

                currentX += columnWidth;
            }
        }

       

        private bool IsLastColumn(int i)
        {
            return i == GetNumColumns() - 1;
        }

        /// <summary>
        /// creates a vertical line by adding the height to the Y coordinate
        /// </summary>
        /// <param name="colRectangleBounds"></param>
        /// <returns></returns>
        private PdfPath GetVerticalLine(PdfBounds colRectangleBounds)
        {
            PdfPath line = new PdfPath(_lineFillColor, 1);
            line.MoveTo(colRectangleBounds.Left, colRectangleBounds.Bottom );
            line.LineTo(colRectangleBounds.Left, colRectangleBounds.Bottom + colRectangleBounds.Height);
            return line;
        }

        /// <summary>
        /// creates a horizontal line by changing the X coordinate from the left side of the bounds to the right side
        /// </summary>
        /// <param name="colRectangleBounds"></param>
        /// <returns></returns>
        private PdfPath GetHorizontalLine(PdfBounds colRectangleBounds)
        {
            PdfPath line = new PdfPath(_lineFillColor, 1);
            line.MoveTo(colRectangleBounds.Left, colRectangleBounds.Bottom);
            line.LineTo(colRectangleBounds.Right, colRectangleBounds.Bottom);
            return line;
        }

        private bool ShouldDrawText(LayoutItemColumn dc)
        {
            if (dc.ExtendedProperties.ContainsKey(DrawText))
            {
                return (bool)dc.ExtendedProperties[DrawText];
            }
            return true;
        }

        private bool ShouldDrawGridLines(LayoutItemColumn dc)
        {
            if (dc.ExtendedProperties.ContainsKey(DrawGridLine))
            {
                return (bool)dc.ExtendedProperties[DrawGridLine];
            }
            return true;
        }

        private double GetXPositionWithPadding(double left, PdfTextAlignment alignment)
        {
            switch(alignment )
            {
                case PdfTextAlignment.Left:
                    {
                        return left + TextPaddingLeft;
                    }
                default:
                    {
                        return left ;
                    }
            }
          }

        private PdfBounds GetBottomLineGridLineBounds(double left, double bottom, double width)
        {

            return new PdfBounds(left, bottom, width, 1);
        }

        private PdfBounds GetRowGridLineBounds(double left, double bottom, double width)
        {
           return new PdfBounds(left, bottom, width, 1);
        }

        private PdfBounds GetGridLineBounds(double left, double bottom, string text, double width)
        {
            return new PdfBounds(left, bottom, width, Bounds.Height);
        }

        private PdfBounds GetRowBounds(double left, double bottom, string text, double width)
        {
                return new PdfBounds(left, bottom, width, Bounds.Height);
        }


        /// <summary>
        /// if there was textStyle added to the control create a PdfStyledTextBox and render the styled text
        /// if there was NOT any textStyle added, use a regular textbox
        /// </summary>
        /// <param name="textboxBounds"></param>
        /// <param name="text"></param>
        /// <param name="textAlignment"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        protected virtual IPdfRenderable GetTextBox(PdfBounds textboxBounds, string text, PdfTextAlignment textAlignment, LayoutItemColumn dc)
        {
            string fontName = GetRowDataFontName(dc);
            if (_textStyle != null)
            {
                PdfStyledTextBox text1 = new PdfStyledTextBox(GetResourceName(fontName, _pdf.Resources), textboxBounds);
                text1.Fill(_textStyle, _pdf.Resources.Fonts);
                return text1;
            }
            else
            {
                PdfTextBox text1 = new PdfTextBox(textboxBounds,GetResourceName(fontName, _pdf.Resources),FontSize);
                text1.FillColor = _textFillColor;
                text1.Alignment = textAlignment;
                text1.Text = text;
                text1.OutlineColor = _textFillColor;
                return text1;
            }
          
        }
        #endregion
    }
}
