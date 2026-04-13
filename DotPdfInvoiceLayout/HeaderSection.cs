using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.PdfDoc.Generating.Rendering;
using Atalasoft.PdfDoc.Generating;
using Atalasoft.PdfDoc.Generating.Shapes;
using Atalasoft.PdfDoc.Geometry;


namespace PDFInvoice
{
    /// <summary>
    ///Add the company header. The company header consists of:
    ///1. The company logo - image 
    ///2. the company address - a PdfTextbox

    ///     Add the document header information. 
    ///1. What type of document is it(Quote, Invoice)
    /// 2. transaction number
    /// 3. transaction date
    /// </summary>
    [Serializable]
    public class HeaderSection : PdfLayoutItem, IPdfRenderable
    {
        private bool _wasLogoAdded = false;
        private double _sectionHeight = 100;
        private double _sectionWidth = 200;
        private double _spaceBetweenLogoAndAddress = 75;
        private static IPdfColor _logoColor = PdfColorFactory.FromColor(System.Drawing.Color.Transparent);

        private string _logoName = "AtalasoftLogo";
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string HeaderText { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public double PageWidth { get; set; }
        public string FontName { get; set; }
        public string FontNameBold { get; set; }
        public double CompanyFontSize { get; set; }

        /// <summary>
        /// set all our default property values in the constructor
        /// </summary>
        /// <param name="doc"></param>
        public HeaderSection(PdfGeneratedDocument doc)
        {
            _pdf = doc;
            LeftMargin = 20;
            RightMargin = 20;
            TopMargin = 50;
            BottomMargin = 50;
            HorizontalSpacer = 0;
            VerticalSpacer = 20;
            Position = new PdfPoint(LeftMargin, 0);
            Height = _sectionHeight;
            FontName = "Times New Roman";
            FontNameBold = "Times New Roman Bold";
            CompanyFontSize = 8;
            FontSize = 10;
        }

        #region IPdfRenderable Members
        public  void Render(PdfPageRenderer r)
        {
            // add the margins and vertical space to get the items position
            double currentX = CalculateStartingX();

            // use the items Y position and take the top margin and vertical spacer into account
            double currentY = Position.Y;// -TopMargin - VerticalSpacer;

            // Add the company header. The company header consists of:
            //   1. The company logo - an image 
            //   2. the company address - a PdfTextbox
            if (_wasLogoAdded)
            {
                double imageHeight = 30;
                double imageWidth = 60;
                PdfBounds logoBounds = new PdfBounds(currentX, currentY + 50, imageWidth, imageHeight);
                r.DrawingSurface.PlaceImage(_logoName, _logoColor.ColorSpaceResourceName, logoBounds);
            }

            currentX += _spaceBetweenLogoAndAddress;
            PdfBounds addressBounds = new PdfBounds(currentX, currentY, _sectionWidth, _sectionHeight);
            PdfTextBox companyAddress = GetCompanyAddressTextbox(addressBounds);
            companyAddress.Render(r);

           
            currentX = (PageWidth - RightMargin - _sectionWidth);
            PdfBounds invoiceDetailsBounds = new PdfBounds(currentX, currentY, _sectionWidth, _sectionHeight);
            PdfStyledTextBox docHeader = GetDocumentHeaderTextbox(invoiceDetailsBounds);
            docHeader.Render(r);
        }

        #endregion

        // Adds the document header information. 
        // 1. What type of document is it(Quote, Invoice)
        // 2. transaction number
        // 3. transaction date
        private PdfStyledTextBox GetDocumentHeaderTextbox(PdfBounds headerBounds)
        {
            PdfStyledTextBox docText = new PdfStyledTextBox(GetResourceName(FontNameBold, _pdf.Resources), headerBounds);
            docText.Alignment = PdfTextAlignment.Right;

            // use a StyleTextInput object so we can change the font style of some of the text
            StyleTextInput headerStyle = new StyleTextInput(GetResourceName(FontNameBold, _pdf.Resources));
            headerStyle.ChangeFontSize(FontSize);
            headerStyle.AddText(HeaderText);
            headerStyle.AddLineBreak();
            headerStyle.AddText(TransactionNumber);
            headerStyle.AddLineBreak();
            headerStyle.AddLineBreak();

            // use a different font fot the transaction date
            headerStyle.ChangeFont(GetResourceName(FontName, _pdf.Resources));
            headerStyle.AddText(TransactionDate.ToLongDateString());
            docText.Fill(headerStyle, _pdf.Resources.Fonts);
            return docText;
        }

        private PdfTextBox GetCompanyAddressTextbox(PdfBounds addressBounds)
        {
            return new PdfTextBox(addressBounds,
                                    GetResourceName(FontName, _pdf.Resources),
                                    CompanyFontSize,
                                    GetCompanyAddressText());
        }
             
        private string GetCompanyAddressText()
        {
            return CompanyName + _newLine +
                            Address1 + _newLine +
                            Address2 + _newLine +
                            City + "," + State + " " + PostalCode + _newLine +
                            Country + _newLine +
                            Phone + _newLine +
                            Fax;
        }


        public void SetLogo(string resourceName)
        {
            System.Drawing.Bitmap logo = GetEmbeddedResource(resourceName);
            if ( logo != null)
            {
                _pdf.Resources.Images.AddImage(_logoName, logo);
                _wasLogoAdded = true;
            }
        }
        private System.Drawing.Bitmap GetEmbeddedResource(string resourceName)
        {
            System.Reflection.Assembly billingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (System.IO.Stream resStream = billingAssembly.GetManifestResourceStream(resourceName))
            {
                if (resStream != null)
                {
                    // the image we have as a resource is an 8bit, need to convert to 24 bit
                    using (Atalasoft.Imaging.AtalaImage temp = new Atalasoft.Imaging.AtalaImage(resStream))
                    {
                        return temp.GetChangedPixelFormat(Atalasoft.Imaging.PixelFormat.Pixel24bppBgr).ToBitmap();
                    }
                }
            }
            return null;
        }
    }
}
