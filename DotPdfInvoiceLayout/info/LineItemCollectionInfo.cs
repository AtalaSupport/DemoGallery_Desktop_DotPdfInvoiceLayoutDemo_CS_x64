using System;
using System.Collections.Generic;
using System.Collections;

namespace Atalasoft.Web.Data.Billing.InvoiceDocument
{
	public class LineItemCollectionInfo : CollectionBase
	{
		public LineItemCollectionInfo()
		{
		}

		

		public LineItemInfo this[ int index ]  
		{
			get  
			{
				return ((LineItemInfo)List[index]);
			}
			set  
			{
				List[index] = value;
			}
		}

        public int Add(LineItemInfo value)  
		{
			int AddPosition;
			AddPosition = List.Add( value );
			return AddPosition;
		}

        internal int AddExisting(LineItemInfo value)
		{
			return List.Add( value );
		}

        public int IndexOf(LineItemInfo value)  
		{
			return( List.IndexOf( value ) );
		}

        public void Insert(int index, LineItemInfo value)  
		{
			List.Insert( index, value );
		}

        public void Remove(LineItemInfo value)  
		{
			List.Remove( value );
		}

        public bool Contains(LineItemInfo value)  
		{
			// If value is not of type Int16, this will return false.
			return( List.Contains( value ) );
		}

		public double Value()
		{
			double _value = 0;

            foreach (LineItemInfo li in this)
			{
				_value =+ li.UnitPrice;
			}

			return _value;
		}

		public bool HasTaxableProducts()
		{
            foreach (LineItemInfo li in this)
			{
				if (li.IsProductTaxable)
					return true;
			}

			return false;
		}

	}
}
