using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.Web.Data.Billing.InvoiceDocument;
using System.IO;

namespace PDFInvoice
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoWelcome();

            InvoiceInfo inv = new InvoiceInfo();
            inv.BillToAddress =  new AddressInfo("Vincent", "Palermo", "Generic Auto Care", "120 Meadowbrook Road", "", "Springfield", "NY", "12180", "US", "508-233-9090", "508-334-9111");
            inv.BillToAddress.EmailAddress = "vpalermo@genericauto.com";
            inv.ShipToAddress = new AddressInfo("Mary", "Tucker", "Generic Auto Care", "1312 Supper Road", "", "Troy", "NY", "12180", "US", "508-233-9090", "508-334-9111");
            inv.ShipToAddress.EmailAddress = "mtucker@genericauto.com";
            inv.DateFinancialReport = DateTime.Now;
            inv.HasPurchaseOrder = true;
            inv.InvoiceType = "INVOICE";
            inv.PurchaseOrder = new PurchaseOrderInfo("#123432", "Net 30", DateTime.Now, "Email");
            inv.TransactionNumber = "2011120233-123494-394898-1";
            inv.LineItems = GetLineItems();

            InvoiceManager docmanager = new InvoiceManager(inv);
            docmanager.MakeInvoice();
            docmanager.Save(GetFilePath());

            DemoEnd();
        }

        private static LineItemCollectionInfo GetLineItems()
        {
            LineItemCollectionInfo coll = new LineItemCollectionInfo();
            coll.Add(new LineItemInfo("BCPBM","DotImage Barcoding All1D DEV/Build Renewal",3,270.00,false));
            coll.Add(new LineItemInfo("BCPXM-U", "DotImage Barcoding All1D Server Renewal (9+ Cores)", 4, 425.00, false));
            coll.Add(new LineItemInfo("BCSM","DotImage Barcoding Code39 SDK Renewal",1,360.00,false));
            coll.Add(new LineItemInfo("BCSXM-4","DotImage Barcoding Code39 Server Renewal",2,350.00,false));
            coll.Add(new LineItemInfo("BCWM","DotImage Barcoding Writing SDK Renewal",1,550.00,false));
            coll.Add(new LineItemInfo("BCWXM-U", "DotImage Barcoding Writing Server Renewal (9+ Cores)", 4, 325.00, false));
            coll.Add(new LineItemInfo("DIDM", "DotImage Document Imaging SDK Renewal", 1, 1080.00, false));
            coll.Add(new LineItemInfo("DIDBM", "DotImage Document Imaging Dev/Build Renewal", 2, 540.00, false));
            coll.Add(new LineItemInfo("DIDXM-8", "DotImage Document Imaging Server Renewal (Standard)", 2, 625.00, false));
            coll.Add(new LineItemInfo("DIDXM-U", "DotImage Document Imaging Server Renewal (9+ Cores)", 2, 925.00, false));
            return coll;
        }

        private static void DemoWelcome()
        {
            Console.WriteLine("Atalasoft DotPdf Invoice Generation Demo");
            Console.WriteLine();
            Console.WriteLine("***************************************************************************");
            Console.WriteLine("This program demonstrates using DotPdf to create PDF invoices from a template.");
            Console.WriteLine("Since all of the PDF generation is happening in the background, refer to the");
            Console.WriteLine("source code for this demo to see how the invoice template is created and used.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Download the DotImage SDK at:");
            Console.WriteLine("     https://www.atalasoft.com/BeginDownload");
            Console.WriteLine();
            Console.WriteLine("Download the DotImage API Reference and Dev Guide:");
            Console.WriteLine("     https://www.atalasoft.com/Support/APIs-Dev-Guides");
            Console.WriteLine();
            Console.WriteLine("Download the full sources for this demo at:");
            Console.WriteLine("     https://www.atalasoft.com/KB2/KB/50086/");
            Console.WriteLine("***************************************************************************");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press any key to generate a filled-out invoice...");
            Console.WriteLine("When the invoice is ready, it will open in your default PDF reader.");

            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("Processing...");
        }

        private static void DemoEnd()
        {
            System.Diagnostics.Process.Start(GetFilePath());
        }

        private static string GetFilePath()
        {
            return Path.Combine(Path.GetTempPath(), "dotpdf-invoice-layout.pdf");
        }
    }
}
