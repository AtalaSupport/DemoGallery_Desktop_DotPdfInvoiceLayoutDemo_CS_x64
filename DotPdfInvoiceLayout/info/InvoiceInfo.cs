using System;
using System.Collections.Generic;

namespace Atalasoft.Web.Data.Billing.InvoiceDocument
{
    public class InvoiceInfo
    {

        public AddressInfo BillToAddress { get; set; }
        public AddressInfo ShipToAddress { get; set; }
        public LineItemCollectionInfo LineItems { get; set; }
        public PurchaseOrderInfo PurchaseOrder { get; set; }
        public string InvoiceType { get; set; }

        public string TransactionNumber { get; set; }
        public DateTime DateFinancialReport { get; set; }

        public bool HasPurchaseOrder { get; set; }
        public InvoiceInfo()
        {
            
            
        }

    }
}
