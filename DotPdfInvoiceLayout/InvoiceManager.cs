using System;
using System.Collections.Generic;

using System.Text;
using System.Data;

using System.Configuration;
using Atalasoft.PdfDoc.Generating.Shapes;
using Atalasoft.Web.Data.Billing.InvoiceDocument;

namespace PDFInvoice
{
    public class InvoiceManager 
    {
        private InvoiceInfo _transaction;
        private double _pageHeight = 800;
        private double _pageWidth = 600;
        private PdfDocumentLayout _pdfInvoice;
        InvoiceDataUtils _dataUtils = null;
       
        public InvoiceManager(InvoiceInfo inv)
        {
            _transaction = inv;
            _dataUtils = new InvoiceDataUtils(_transaction);
        }

        public void MakeInvoice()
        {
            _pdfInvoice = new PdfDocumentLayout();
            _pdfInvoice.CreatePageAndSetAsCurrent();
            _pdfInvoice.AddItemToCurrentPage(GetCompanyHeader(_pageWidth));
            _pdfInvoice.AddTablesAcrossDocument(GetBillToShipToAddresses());
            _pdfInvoice.AddVerticalSpacer();
            if (HasPurchaseOrder())
            {
                _pdfInvoice.AddTableToDocument(GetPurchaseOrderDetails());
                _pdfInvoice.AddVerticalSpacer();
            }
            _pdfInvoice.AddTableToDocument(GetLineItems());
            _pdfInvoice.AddVerticalSpacer();
            _pdfInvoice.AddTableToDocument(GetWireTransferInstructions());   
        }

        private HeaderSection GetCompanyHeader(double pageWidth)
        {
            HeaderSection hs = new HeaderSection(_pdfInvoice.CurrentDocument);
            AddCompanyInfo(hs);
            AddDocumentInfo(hs);
            hs.PageWidth = pageWidth;
            return hs;
        }

        private void AddDocumentInfo(HeaderSection hs)
        {
            hs.HeaderText = "Invoice";
            hs.TransactionNumber = _transaction.TransactionNumber;
            hs.TransactionDate = _transaction.DateFinancialReport;
        }

        public List<PdfTable> GetBillToShipToAddresses()
        {
            PdfTable billToTable = _dataUtils.GetBillToAddress(_pdfInvoice.CurrentDocument);
            PdfTable shipToTable = _dataUtils.GetShipToAddress(_pdfInvoice.CurrentDocument);           
            List<PdfTable> tables = new List<PdfTable>();
            tables.Add(billToTable);
            tables.Add(shipToTable);
            return tables;
        }

        public PdfTable GetWireTransferInstructions()
        {
            PdfTable wireTransfer = _dataUtils.GetWireTransferTable(_pdfInvoice.CurrentDocument);
            return wireTransfer;
        }

        /// <summary>
        /// checks to see if the transaction has a purchase order object associated with it
        /// </summary>
        /// <returns></returns>
        public bool HasPurchaseOrder()
        {
            return _transaction.HasPurchaseOrder;            
        }

        public PdfTable GetPurchaseOrderDetails()
        {
            return  _dataUtils.GetPurchaseOrderTable(_pdfInvoice.CurrentDocument);            
        }

        public PdfTable GetLineItems()
        {          
            PdfTable lineItems = _dataUtils.GetLineItemsTable(_pdfInvoice.CurrentDocument);
            lineItems.ShouldAddVerticalGridLines = false;
            lineItems.PageHeight = _pageHeight;
            lineItems.RowDataFontSize = 10;
            return lineItems;
        }
     
        #region Data Helper Methods
        private void AddCompanyInfo(HeaderSection hs)
        {
            string logoName = "PDFInvoice.images.logo_atalasoft.gif";
            hs.SetLogo(logoName);
            AddressInfo companyAddress = _dataUtils.GetCompanyAddress();
            hs.CompanyName = companyAddress.Company;
            hs.Address1 = companyAddress.Address1;
            hs.Address2 = companyAddress.Address2;
            hs.City = companyAddress.City;
            hs.State = companyAddress.State;
            hs.PostalCode = companyAddress.Zip;
            hs.Country = companyAddress.Country;
            hs.Phone = companyAddress.Phone;
            hs.Fax = companyAddress.Fax;
        }

#endregion

        public byte[] GetPdfBytes()
        {
            return _pdfInvoice.GetPdfBytes();
        }

        public void Save(string path)
        {
            _pdfInvoice.Save(path);
        }

       
    }
}
