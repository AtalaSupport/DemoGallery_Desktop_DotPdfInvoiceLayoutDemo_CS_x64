using System;
using System.Collections.Generic;

using System.Text;
using Atalasoft.PdfDoc.Geometry;
using Atalasoft.PdfDoc.Generating;
using Atalasoft.PdfDoc.Generating.Rendering;
using System.Data;
using Atalasoft.PdfDoc.Generating.Shapes;

namespace PDFInvoice
{
    /// <summary>
    /// this class is responsible for positioning items on a document, it adds pages as necessary
    /// </summary>
    public class PdfDocumentLayout
    {
        private PdfGeneratedDocument _pdf;
        private double _spacer = 20;
        private double _pageHeight = 800;
        private double _pageWidth = 600;
        private double _topMargin = 40;
        private PdfPoint _currentDocPosition;
        private PdfGeneratedPage _currentPage;
        private PdfGeneratedPageFactory _pageFactory = new PdfGeneratedPageFactory();

        public PdfDocumentLayout()
        {
            Init();            
        }

        public PdfGeneratedDocument CurrentDocument
        {
            get { return _pdf; }
        }
    
        public double VerticalSpacer
        {
            get { return _spacer; }
            set { _spacer = value; }
        }

        private void Init()
        {
            _pdf = new PdfGeneratedDocument();
            _pdf.EmbedGeneratedContent = false;

            // initial starting point of doc, top - left
            SetStartingPagePosition();
        }

        /// <summary>
        ///  we will need to reset the current position whenever we create a new page
        /// </summary>
        public void SetStartingPagePosition()
        {
            double topOfPage = _pageHeight - _topMargin;
            double leftPosition = 0 ;
            _currentDocPosition = new PdfPoint(leftPosition, topOfPage);
        }

        public PdfPoint CurrentDocumentPosition
        {
            get { return _currentDocPosition; }
            set { _currentDocPosition = value; }
        }



        public void AddItemToCurrentPage(HeaderSection item)
        {
            PdfPoint currPosition = GetCurrentPosition();
            item.Position = GetPositionBasedOnHeight(currPosition,item);
            _currentPage.DrawingList.Add(item);
            AdvanceCurrentDocPosition(currPosition, item);
        }

        /// <summary>
        /// assumes we want to put the item at the current position(where we left off)
        /// </summary>
        /// <param name="item"></param>
        public void AddItemToCurrentPage(PdfLayoutDataItem item)
        {
            PdfPoint currPosition = GetCurrentPosition();
            item.Position = GetPositionBasedOnHeight(currPosition,item);
            _currentPage.DrawingList.Add(item);
            AdvanceCurrentDocPosition(currPosition, item);
        }
        
        public void AddShapeToCurrentPageLocation(PdfBaseShape shape)
        {
            PdfPoint currPosition = GetCurrentPosition();
            shape.Location = currPosition;
            _currentPage.DrawingList.Add(shape);        
        }
        
        /// <summary>
        /// moves the current document postion the amount of the items height, and resets the X position to 0
        /// </summary>
        /// <param name="currPosition"></param>
        /// <param name="item"></param>
        private void AdvanceCurrentDocPosition(PdfPoint currPosition, PdfLayoutItem item)
        {
            _currentDocPosition = new PdfPoint(0, currPosition.Y - item.Height);
        }

        /// <summary>
        /// creates a new page and adds it to the document, any new items added will be added to the new page
        /// </summary>
        /// <returns></returns>
        public PdfGeneratedPage CreatePageAndSetAsCurrent()
        {
            PdfGeneratedPage page1 = _pageFactory.GetNewPage();
            _pdf.Pages.Add(page1);
            SetStartingPagePosition();
            _currentPage = page1;
            return page1;
        }

        private PdfPoint GetCurrentPosition()
        {
            return _currentDocPosition;
        }

        public void AddVerticalSpacer()
        {
            AddVerticalSpacer(_spacer);
        }

        public void AddVerticalSpacer(double spaceToAdd)
        {
            // check to see if we need to move to a new page
            if (ItemFitsOnPage(spaceToAdd, _currentDocPosition))
            {
                MoveCurrentPosition(new PdfPoint(0, _currentDocPosition.Y - spaceToAdd));
            }
            else
            {
                CreatePageAndSetAsCurrent();
            }
        }

        public void MoveCurrentPosition(PdfPoint newPosition)
        {
            _currentDocPosition = newPosition;
        }
               
        private PdfPoint AddTableHeaderToPage(PdfTable table1, PdfPoint currentPoint, PdfGeneratedDocument doc)
        {
            // check to see if the table header will fit on the current page
            // if not , add the current page to the document and create a new page
            if (!ItemFitsOnPage(table1.Header.Height, currentPoint))
            {
                _currentPage = CreatePageAndSetAsCurrent();
                SetStartingPagePosition();
                currentPoint = _currentDocPosition;
            }
            
            // set where the header will be placed
            table1.Header.Position = GetPositionBasedOnHeight(currentPoint, table1.Header);// new PdfPoint(currentPoint);

            // need to add a clone of the header, don't want to accidentally reference the table header again
            _currentPage.DrawingList.Add(table1.Header.Clone());
            // return the point where we left off in the document
            return new PdfPoint(currentPoint.X, currentPoint.Y - table1.Header.Height);
        }

        /// <summary>
        /// this method will assume the user wants to add the item to the current position in the document
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="table1"></param>
        public void AddTableToDocument(PdfTable table1)
        {
            AddTableToDocument(GetCurrentPosition(), table1);
        }

        /// <summary>
        /// adds tables side by side on the document, it adds each table and increments the X position but not the Y
        /// </summary>
        /// <param name="tables"></param>
        public void AddTablesAcrossDocument(List<PdfTable> tables)
        {
            PdfPoint initialPosition = GetCurrentPosition();
            PdfPoint currentPoint = GetCurrentPosition();
            foreach (PdfTable table1 in tables)
            {
                // add the table header, using the initialPosition as the starting point
                // method returns the point in the document where we left off
                currentPoint = AddTableHeaderToPage(table1, currentPoint, _pdf);

                // add each row to the document
                foreach (PdfTableRow row in table1.Rows)
                {
                    // set the position of the row and add it
                    row.Position = GetPositionBasedOnHeight(currentPoint,row);
                    _currentPage.DrawingList.Add(row);                 
                }

                // set the values of the point where the NEXT table will be placed
                currentPoint = new PdfPoint(currentPoint.X + (_pageWidth / tables.Count), initialPosition.Y);
                _currentDocPosition = currentPoint;            
            }

            // set the current point for the document, so we know where we left off
            // use the height of the first table to determine how much we should increase the Y position
           MoveCurrentPosition(new PdfPoint(0,(currentPoint.Y - tables[0].CalculateHeight())));
        }

        private PdfPoint GetPositionBasedOnHeight(PdfPoint currentPoint, PdfLayoutItem item)
        {
            return new PdfPoint(currentPoint.X, currentPoint.Y - item.Height);
        }

        public void AddTableToDocument(PdfPoint initialPosition, PdfTable table1)
        {
            // check to see if the whole table must fit on the page
            if (table1.MustFitOnPage)
            {
                // if it does NOT fit on the page, add a new page and 
                // set the document position marker to the beginning of the page
                if ( !ItemFitsOnPage(table1.CalculateHeight(),initialPosition))
                {
                    _currentPage = CreatePageAndSetAsCurrent();
                    SetStartingPagePosition();
                    initialPosition = _currentDocPosition;
                }
                 
            }
            // add the table header, using the initialPosition as the starting point
            // method returns the point where we left off in the document
            PdfPoint currentPoint = AddTableHeaderToPage(table1, initialPosition, _pdf);

            // add each row to the document
            foreach (PdfTableRow row in table1.Rows)
            {
                // check to see if it will fit on the current page
                // if not, add the current page to the document and make a new one
                // also add the table header to the new page

                // don't bother checking to see if the row fits on the page if the MustFitOnPage property is True
                if (!table1.MustFitOnPage && !RowFitsOnPage(row, currentPoint))
                {
                    // create a new page in the document to put the items on 
                    _currentPage = CreatePageAndSetAsCurrent();
                    SetStartingPagePosition();
                    initialPosition = _currentDocPosition;
                    // add the table header to the new page
                    currentPoint = AddTableHeaderToPage(table1, initialPosition, _pdf);
                }

                //add the current row, returns where we left off
                currentPoint = AddRowToPageAndAdvancePosition(row, currentPoint, _currentPage);
            }

            // set the current point for the document, so we know where we left off
            _currentDocPosition = currentPoint;
        }

        private PdfPoint AddRowToPageAndAdvancePosition(PdfTableRow row, PdfPoint position, PdfGeneratedPage page)
        {
            row.Position = GetPositionBasedOnHeight(position,row);
            page.DrawingList.Add(row);
            return new PdfPoint(position.X, position.Y - row.Height);
        }

        // compare the height of the item to the current point in the document
        private bool ItemFitsOnPage(double height, PdfPoint currentPoint)
        {
            return (currentPoint.Y > height);
        }

        // will the current row based on the height fit with where the current position is?
        private bool RowFitsOnPage(PdfTableRow row, PdfPoint currentPoint)
        {
            return ItemFitsOnPage(row.Height, currentPoint);
        }

        public byte[] GetPdfBytes()
        {
            System.IO.MemoryStream invoiceStream = new System.IO.MemoryStream();
            _pdf.Save(invoiceStream);
            return invoiceStream.ToArray();
        }

        public void Save(string path)
        {
            if (_pdf != null)
            {
                _pdf.Save(path);
            }
        }

    }
}
