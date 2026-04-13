using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.PdfDoc.Generating;

namespace PDFInvoice
{
    public class PdfGeneratedPageFactory
    {
        private double _pageHeight = 800;
        private double _pageWidth = 600;

        internal Atalasoft.PdfDoc.Generating.PdfGeneratedPage MakePage()
        {
            return new PdfGeneratedPage(_pageWidth, _pageHeight);
        }

       

        internal Atalasoft.PdfDoc.Generating.PdfGeneratedPage GetNewPage()
        {
            return  new PdfGeneratedPage(_pageWidth, _pageHeight);
        }
    }
}
