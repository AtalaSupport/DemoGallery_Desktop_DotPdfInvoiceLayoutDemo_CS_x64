using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using Atalasoft.PdfDoc.Generating.Shapes;
using System.Configuration;
using Atalasoft.PdfDoc.Generating;
using Atalasoft.Web.Data.Billing.InvoiceDocument;

namespace PDFInvoice
{
    public class InvoiceDataUtils
    {
        private string _newLine = "\n";
        private InvoiceInfo _transaction;

        public InvoiceDataUtils(InvoiceInfo trans)
        {
            _transaction = trans;
        }

        public PdfTable GetAddressTable(PdfGeneratedDocument doc,string addrName,AddressInfo addr,string width)
        {
            PdfTable address = new PdfTable(doc,10);
            address.ShouldAddVerticalGridLines = true;
            address.AddColumn(addrName,width,PdfTextAlignment.Left);
            address.AddRow(new string[]{ addr.FirstName + " " + addr.LastName + _newLine + addr.Company + _newLine + addr.Address1 + _newLine + addr.Address2 + _newLine
                       + addr.City + "," + addr.State + " " + addr.Zip + _newLine + addr.CountryText + _newLine + addr.EmailAddress});
            
            return address;
        }       

        public PdfTable GetBillToAddress(PdfGeneratedDocument doc)
        {
            return GetAddressTable(doc,"Billing Address", _transaction.BillToAddress, "260");
        }

        public PdfTable GetShipToAddress(PdfGeneratedDocument doc)
        {
            return GetAddressTable(doc,"Shipping Address", _transaction.ShipToAddress, "260");
        }
          
        public PdfTable GetLineItemsTable(PdfGeneratedDocument doc)
        {
            LineItemCollectionInfo lineItemCollection = _transaction.LineItems;
            PdfLineItemTable lineItems = new PdfLineItemTable(doc);
            lineItems.VerticalSpacer = 15;
            lineItems.TextPaddingLeft = 10;
            lineItems.AddColumn("Sku", "60", PdfTextAlignment.Left);
            lineItems.AddColumn("Description", "280", PdfTextAlignment.Left);
            lineItems.AddColumn("Quantity", "60", PdfTextAlignment.Right);
            lineItems.AddColumn("Unit Price", "80", PdfTextAlignment.Right);
            lineItems.AddColumn("Total", "80", PdfTextAlignment.Right);

             int quantityDecimalCount = 0;
             for (int i = 0; i < lineItemCollection.Count; i++)
             {
                 LineItemInfo li = lineItemCollection[i];
                 lineItems.AddRow(new string[] { li.ProductSku, li.ProductName, li.Quantity.ToString("N" + quantityDecimalCount.ToString()), li.UnitPrice.ToString("c"), (li.Quantity * li.UnitPrice).ToString("c") });
             }
             lineItems.AddSubtotalRow(4);
            return lineItems;
        }
    
        public string GetConfigurationValue(string key)
        {
            string temp = ConfigurationManager.AppSettings[key];

            // When pulling from the AppConfig, .NET adds escapes the newline characters
            string tempStripped = temp.Replace("\\n", "\n");
            return tempStripped;
        }

        public PdfTable GetWireTransferTable(PdfGeneratedDocument doc)
        {
            PdfTable wire = new PdfTable(doc);
            string wireTransferColumnHeader = "Atalasoft, Inc. WIRE INSTRUCTIONS";
            wire.AddColumn(wireTransferColumnHeader, "360", PdfTextAlignment.Left);
            
            // no gridlines for the Wire transfer instruction text
            wire.SetColumnProperty(0, PdfTableRow.DrawGridLine, false);
            wire.ShouldAddVerticalGridLines = false;
            wire.ShouldAddBorder = false;

            PdfTableRow row = new PdfTableRow(doc, wire.Columns, new string[] { "Text that \nwill be\n replaced by\n\nmore text\nthe styled \ninput\n text\n\nmore text this is needed \n\nbecause we base the heigh" }, wire);
            row.SetRowText(GetWireTransferText(doc));
            wire.AddRow(row);

            // we want the whole wire transfer section together(not split between two pages), so we set the MustFitOnPage property
            wire.MustFitOnPage = true;
            return wire;
        }
    
        private void AddLineBreaks(string text,StyleTextInput st)
        {
            string[] text1 = text.Split('\n');
            for (int i = 0; i < text1.Length; i++)
            {
                st.AddText(text1[i]);
                st.AddLineBreak();
            }
        }

        public StyleTextInput GetWireTransferText(PdfGeneratedDocument doc)
        {
            string wireTransferCompanyAddress = "Atalasoft, Inc\n116 Pleasant St, Suite 321\nEasthampton, MA 01027\n866-568-0129";
            string wireTransferSectionHeader = "Wire Transfer Information:";
            string wireTransferAddress = "Silicon Valley Bank / San Jose North\n3003 Tasman Drive\nSanta Clara, CA 95054";
            string wireTransferPhone = "\nWire Department FAX: (408) 496-2401\nWire Department Phone: ( 800) 215-6060";
            string wireTransferAccountNumber = "Account Number: 555555555\nABA Number: 555555555\nSwift Code: 55555555";
            string fontName = doc.Resources.Fonts.AddFromFontName("Times New Roman");
            StyleTextInput st = new StyleTextInput(fontName);
            AddLineBreaks(wireTransferCompanyAddress, st);
            st.AddLineBreak();
            string fontBoldName = doc.Resources.Fonts.AddFromFontName("Times New Roman Bold");
            st.ChangeFont(fontBoldName);
            AddLineBreaks(wireTransferSectionHeader, st);
            st.AddLineBreak();
            st.ChangeFont(fontName);
            AddLineBreaks(wireTransferAddress, st);
            AddLineBreaks(wireTransferPhone, st);
            AddLineBreaks(wireTransferAccountNumber, st);
            return st;            
        }
        
        public PdfTable GetPurchaseOrderTable(PdfGeneratedDocument doc)
        {
            PurchaseOrderInfo purchaseOrder = _transaction.PurchaseOrder;
            PdfTable po = new PdfTable(doc);
            po.AddColumn("PO Number");
            po.AddColumn("Terms");
            po.AddColumn("Ship Date");
            po.AddColumn("Via");
            po.AddRow(new string[] { purchaseOrder.PONumber, purchaseOrder.TermText, purchaseOrder.DateCreated.ToShortDateString() ,"Email"});
            return po;
        }

        public  AddressInfo GetCompanyAddress()
        {
            AddressInfo addr = new AddressInfo();
            addr.Company = "Atalasoft, Inc";
            addr.Address1 = "116 Pleasant Street";
            addr.Address2 = "Suite 321";
            addr.City = "Easthampton";
            addr.Country = "US";
            addr.State = "MA";
            addr.Zip = "01027";

            addr.Phone = "866-568-0129";
            addr.Fax = "413-527-1143";
            return addr;
        }
    }
}
