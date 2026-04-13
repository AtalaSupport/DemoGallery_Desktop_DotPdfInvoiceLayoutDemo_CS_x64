using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;
using Atalasoft.PdfDoc.Generating.Shapes;

namespace PDFInvoice
{
    public class LayoutItemColumn
    {
        // property name constants
        public const string DrawGridLine = "DrawVerticalGridLine";
        public const string DrawText = "DrawText";
        public const string TextAlignment = "TextAlignment";
        public const string Width = "Width";

        public string ColumnName { get; set; }

        private Hashtable _properties = new Hashtable();

        public Hashtable ExtendedProperties
        {
            get { return _properties; }
        }
        public LayoutItemColumn()
        { }

        public LayoutItemColumn(string name)
        {
            ColumnName = name;
        }

        public LayoutItemColumn(string name, string width,PdfTextAlignment textAlignment)
        {
            ColumnName = name;
            AddProperty(Width, width);
            AddProperty(TextAlignment, textAlignment);
        }
        
        private void AddProperty(string name ,object value)
        {
            _properties[name] = value; 
        }

        private LayoutItemColumn CreateColumn(string name, string width)
        {
            return CreateColumn(name, width, PdfTextAlignment.Center);
        }
        

        public static LayoutItemColumn CreateColumn(string name, string width, PdfTextAlignment textAlign)
        {
            LayoutItemColumn col = new LayoutItemColumn();
            col.ColumnName = name;
            col.AddProperty(Width,width);
            col.AddProperty(TextAlignment, textAlign);
            return col;
        }
    }
}
