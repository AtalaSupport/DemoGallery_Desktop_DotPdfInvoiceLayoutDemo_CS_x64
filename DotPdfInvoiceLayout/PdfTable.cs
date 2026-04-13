using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.PdfDoc.Generating;
using Atalasoft.PdfDoc.Generating.Rendering;
using Atalasoft.PdfDoc.Geometry;
using Atalasoft.PdfDoc.Generating.Shapes;
using System.Data;
using Atalasoft.PdfDoc.Generating.ResourceHandling.Fonts;
using Atalasoft.PdfDoc.Generating.ResourceHandling;
using System.Collections;

namespace PDFInvoice
{
    public class PdfTable : PdfLayoutItem
    {
        public PdfTextAlignment HeaderTextAlignment { get; set; }
        public PdfTextAlignment RowTextAlignment { get; set; }
        public double TextPaddingLeft { get; set; }
        public double TextPaddingRight { get; set; }
        public double HeaderFontSize { get; set; }
        public double RowDataFontSize { get; set; }

        protected List<PdfTableRow> _rows = new List<PdfTableRow>();
        protected PdfTableHeader _header;
    

        private PdfTableColumnCollection _columns;
        // if the whole table should fit on the page without a page break
        public bool ShouldTableFitOnPage { get; set; }
        public bool ShouldAddVerticalGridLines { get; set; }
        public bool ShouldAddBorder { get; set; }

        public PdfTableColumnCollection Columns
        { 
            get { return _columns; }  
        }

        public IEnumerable<PdfTableRow> Rows
        {
            get { return _rows; }
        }

        public PdfTableHeader Header
        {
            get { return _header; }
        }

        public PdfTable()
        {
            Init(new PdfGeneratedDocument(), null, PdfTextAlignment.Center, TextPaddingLeft);
        }

        /// <summary>
        /// Create a PdfTable object with rows and columns matching the Datatable passed into the
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dt">Columns and rows will match the columns and rows of the datatable</param>
        /// <param name="textAlignment"></param>
        /// <param name="rowTextPadding"></param>
        public PdfTable(PdfGeneratedDocument doc, DataTable dt, PdfTextAlignment textAlignment, double rowTextPadding)
        {
            Init(doc, dt, textAlignment,rowTextPadding);
        }
        public PdfTable(PdfGeneratedDocument doc, DataTable dt)
        {
            Init(doc, dt, PdfTextAlignment.Center,0);
        }

        public PdfTable(PdfGeneratedDocument doc)
        {
            Init(doc, null, PdfTextAlignment.Center, 0);
        }
        public PdfTable(PdfGeneratedDocument doc, double rowTextPadding)
        {
            Init(doc, null, PdfTextAlignment.Center, rowTextPadding);
        }
       
        public virtual void Init(PdfGeneratedDocument doc, DataTable dt, PdfTextAlignment rowTextAlignment, double rowTextPadding)
        {
            _pdf = doc;

            HeaderFontName = "Times New Roman";
            RowDataFontName = "Times New Roman";
            HeaderFontSize = 10;
            RowDataFontSize = 10;
            LeftMargin = 20;
            RightMargin = 20;
            TopMargin = 50;
            BottomMargin = 50;

            RowTextAlignment = rowTextAlignment;
            HorizontalSpacer = 0;
            VerticalSpacer = 20;
            TextPaddingLeft = rowTextPadding;

            ShouldAddVerticalGridLines = true;
            ShouldAddBorder = true;
            if (dt != null)
            {
                //_tableData = dt;
                AddTableRows(dt);
            }
            else
            {
                
                // initialize columns and rows
                //_tableData = new DataTable();
                _columns = new PdfTableColumnCollection();
                _header = new PdfTableHeader(_pdf, _columns);
                
            }
        }

        public void AddTableRows(DataTable dt)
        {
            _rows.Clear();

            // create the rows from the datasource
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PdfTableRow row = new PdfTableRow(_pdf, dt.Columns, dt.Rows[i],this);
                row.TextAlignment = this.RowTextAlignment;
                row.TextPaddingLeft = this.TextPaddingLeft;
                row.ShouldAddGridLines = this.ShouldAddVerticalGridLines;
               // row.SetRowText()
                _rows.Add(row);
            }

            // need to set the last row IsLastRow property so we know when the table ends
            _rows[_rows.Count - 1].IsLastRow = true;

            // create the table header
            _header = new PdfTableHeader(_pdf, dt.Columns);
        }      

        public void ResetTableRows(DataTable dt)
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                PdfTableRow row = _rows[i];
                row.TextAlignment = this.RowTextAlignment;
                row.TextPaddingLeft = this.TextPaddingLeft;
                row.ShouldAddGridLines = this.ShouldAddVerticalGridLines;                
                row.IsLastRow = false;
            }

            // make sure the last row is set
            if (_rows.Count > 0)
                _rows[_rows.Count - 1].IsLastRow = true;
        }

        public double CalculateHeight()
        {
            double result = 0;
            // add each row to the document
            foreach (PdfTableRow row in this.Rows)
            {
                result += row.Height;
            }

            // don't forget to add in the header height
            result += this.Header.Height;

            return result;
        }

        public void AddColumn(string colName)
        {
            _columns.Add(new LayoutItemColumn(colName));
        }

        public void AddColumn(string name, string width, PdfTextAlignment textAlign)
        {
            _columns.Add(new LayoutItemColumn(name, width, textAlign));
        }

        private void ResetLastRow()
        {
            // add each row to the document
            foreach (PdfTableRow row in this.Rows)
            {
                row.IsLastRow = false;
            }
            // make sure the last row is set
            if (_rows.Count > 0)
                _rows[_rows.Count - 1].IsLastRow = true;
        }

        public void AddRow(object[] items)
        {
            PdfTableRow row = new PdfTableRow(_pdf, _columns, items, this);
            row.TextAlignment = this.RowTextAlignment;
            row.TextPaddingLeft = this.TextPaddingLeft;
            row.ShouldAddGridLines = this.ShouldAddVerticalGridLines;
            this._rows.Add(row);
            ResetLastRow();
        }

        public void AddRow(PdfTableRow row)
        {
            this._rows.Add(row);
            ResetLastRow();
        }

        public void SetColumnProperty(int columnIndex,string propName,object propValue)
        {
            Header.Columns[columnIndex].ExtendedProperties[propName] =  propValue;
        }
    }
}