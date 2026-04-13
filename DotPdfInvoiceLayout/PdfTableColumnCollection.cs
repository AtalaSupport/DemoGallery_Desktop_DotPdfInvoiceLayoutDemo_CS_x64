using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Collections;
using Atalasoft.PdfDoc.Generating.Shapes;

namespace PDFInvoice
{
    public class PdfTableColumnCollection : ICollection, IEnumerable, IList<LayoutItemColumn>
    {
        private List<LayoutItemColumn> _columns;
        
        public List<LayoutItemColumn> Columns
        {
            get { return _columns; }
        }

        public PdfTableColumnCollection()
        {
            _columns = new List<LayoutItemColumn>();
        }

        public PdfTableColumnCollection(DataColumnCollection coll)
        {
            _columns = new List<LayoutItemColumn>();
            for (int i = 0; i < coll.Count; i++)
            {
                DataColumn col = coll[i];
                
                LayoutItemColumn item = new LayoutItemColumn();
                item.ColumnName = col.ColumnName;


                string width = (string)col.ExtendedProperties[LayoutItemColumn.Width];

                if (col.ExtendedProperties.ContainsKey(LayoutItemColumn.TextAlignment))
                {
                    item.ExtendedProperties.Add(LayoutItemColumn.TextAlignment,  (Atalasoft.PdfDoc.Generating.Shapes.PdfTextAlignment)col.ExtendedProperties[LayoutItemColumn.TextAlignment]);
                }
                if (!string.IsNullOrEmpty(width))
                {
                    item.ExtendedProperties.Add(LayoutItemColumn.Width, width);
                }
                _columns.Add(item);
            }
        }

        public PdfTableColumnCollection Clone()
        {
            PdfTableColumnCollection rtnColl = new PdfTableColumnCollection();
            for (int i = 0; i < _columns.Count; i++)
            {
                LayoutItemColumn temp = _columns[i];
                LayoutItemColumn col = new LayoutItemColumn(temp.ColumnName);
                IDictionaryEnumerator en = temp.ExtendedProperties.GetEnumerator();
                while (en.MoveNext())
                {
                    col.ExtendedProperties.Add(en.Key, en.Value);
                }
                
                rtnColl.Add(col);
            }
            return rtnColl;
        }

        public IEnumerator GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return _columns.Count;
            }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public LayoutItemColumn this[int index]
        {
            get
            {
                return _columns[index];
            }
            set
            {
                this[index] = value;
            }
        }

        public int IndexOf(LayoutItemColumn item)
        {
            return _columns.IndexOf(item);
        }

        public void Insert(int index, LayoutItemColumn item)
        {
            _columns.Insert(index, item);
        }

        public void Add(LayoutItemColumn item)
        {
            _columns.Add(item);
        }

        public bool Contains(LayoutItemColumn item)
        {
            return _columns.Contains(item);
        }

        public void CopyTo(LayoutItemColumn[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(LayoutItemColumn item)
        {
            return _columns.Remove(item);
        }

        IEnumerator<LayoutItemColumn> IEnumerable<LayoutItemColumn>.GetEnumerator()
        {
            return _columns.GetEnumerator();
        }


        public void RemoveAt(int index)
        {
            _columns.RemoveAt(index);
        }


        public void Clear()
        {
            _columns.Clear();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
