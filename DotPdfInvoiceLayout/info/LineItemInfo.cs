using System;
using System.Collections.Generic;

namespace Atalasoft.Web.Data.Billing.InvoiceDocument
{
    public class LineItemInfo
    {
        public string ProductSku{get;set;}
        public string ProductName {get;set;}
        public double Quantity {get;set;}
        public double UnitPrice{get;set;}
        public bool IsProductTaxable { get; set; }

        public LineItemInfo(string productSku,string productName,double qty,double price, bool isTaxable)
        {
            ProductSku = productSku;
            ProductName = productName;
            Quantity = qty;
            UnitPrice = price;
            IsProductTaxable = isTaxable;
        }        
    }
}
