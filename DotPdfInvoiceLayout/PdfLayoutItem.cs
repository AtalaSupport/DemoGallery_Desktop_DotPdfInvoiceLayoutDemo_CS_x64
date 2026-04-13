using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.PdfDoc.Generating;
using Atalasoft.PdfDoc.Geometry;
using Atalasoft.PdfDoc.Generating.ResourceHandling;
using Atalasoft.PdfDoc.Generating.Rendering;

namespace PDFInvoice
{
    public abstract class PdfLayoutItem
    {
        protected const string _widthPropertyName = "Width";
        protected const string _newLine = "\n";  

        Dictionary<string, string> _fontNameToResourceName = new Dictionary<string, string>();

        protected string _name;
        protected PdfGeneratedDocument _pdf;
        protected double _width = 600;
        public double HorizontalSpacer { get; set; }
        public double VerticalSpacer { get; set; }
        public string HeaderFontName { get; set; }
        public string RowDataFontName { get; set; }
        public double FontSize { get; set; }
        public double TopMargin { get; set; }
        public double BottomMargin { get; set; }
        public double LeftMargin { get; set; }
        public double RightMargin { get; set; }
        public double PageHeight { get; set; }
        public PdfPoint Position { get; set; }
        public PdfBounds Bounds { get; set; }

        public double Height { get; set; }

        /// <summary>
        /// will be used to determine whether the whole item must fit on a page, or it can be broken up across pages
        /// </summary>
        public bool MustFitOnPage { get; set; }

        public string GetResourceName(string fontName, GlobalResources resources)
        {
            string resName = null;
            if (_fontNameToResourceName.TryGetValue(fontName, out resName))
            {
                return resName;
            }
            resName = resources.Fonts.AddFromFontName(fontName);
            _fontNameToResourceName.Add(fontName, resName);
            return resName;
        }
      
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        protected double CalculateStartingX()
        {
            double left = Position.X + LeftMargin;
            return left;
        }
    }
}
