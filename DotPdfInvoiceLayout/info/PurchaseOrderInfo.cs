using System;
using System.Collections.Generic;



namespace Atalasoft.Web.Data.Billing.InvoiceDocument
{
    public class PurchaseOrderInfo
    {
        public string PONumber{get;set;}
        public string TermText{get;set;}
        public DateTime DateCreated{get;set;}
        public string DeliveredVia{get;set;}

        public PurchaseOrderInfo(string poNum,string term,DateTime createdDate,string via)
        {
            PONumber = poNum;
            TermText = term;
            DateCreated = createdDate;
            DeliveredVia = via;
        }
    }
}
