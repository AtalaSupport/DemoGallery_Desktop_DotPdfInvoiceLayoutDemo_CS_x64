using System;
using System.Collections.Generic;

namespace Atalasoft.Web.Data.Billing.InvoiceDocument
{
    public class AddressInfo
    {
        #region Variables

        private Guid _Guid = Guid.Empty;
        private string _firstName = String.Empty;
        private string _lastName = String.Empty;
        private string _company = String.Empty;
        private string _address1 = String.Empty;
        private string _address2 = String.Empty;
        private string _city = String.Empty;
        private string _StateText = String.Empty;
        private string _zip = String.Empty;
        private string _CountryText = String.Empty;
        private string _phone = String.Empty;
        private string _fax = String.Empty;
        private bool _InWallet = false;
        private DateTime _DateCreated = DateTime.UtcNow;

        private Guid _StateGuid = Guid.Empty;
        private string _State = null;

        private Guid _CountryGuid = Guid.Empty;
        private string _Country = null;

        private Guid _CustomerGuid = Guid.Empty;

        private string _email;

        
        #endregion

        #region Properties

        public string EmailAddress
        {
            get { return _email; }
            set { _email = value; }
        }
        public Guid Guid
        {
            get { return _Guid; }
        }

        /// <remarks>
        /// All DateTime objects in dotCRMCart utilize UTC timestamps. The UTC timestamp
        /// should be assigned when setting the value for a DateTime object, and the timestamp
        /// should be converted / displayed as a localized timestamp when retreiving a
        /// timestamp.
        /// </remarks>
        /// <summary>Gets the DateTime timestamp when the Address object was created.</summary>
        public DateTime DateCreated
        {
            get { return _DateCreated; }
        }

       

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
            }
        }


        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
            }
        }


        public string Company
        {
            get { return _company; }
            set
            {
                _company = value;
            }
        }


        public string Address1
        {
            get { return _address1; }
            set
            {
                _address1 = value;
            }
        }


        public string Address2
        {
            get { return _address2; }
            set
            {
                _address2 = value;
            }
        }


        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
            }
        }


       
        public string State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;

            }
        }


        public string Zip
        {
            get { return _zip; }
            set
            {
                _zip = value;
            }
        }


        public string CountryText
        {
            get { return _CountryText; }
            set
            {
                _CountryText = value;

                if (value != String.Empty)
                {
                    _CountryGuid = Guid.Empty;
                    _Country = null;
                }
            }
        }

        public string Country
        {
            get
            {
                return _Country;
            }
            set
            {
                _Country = value;
            }
        }


        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
            }
        }


        public string Fax
        {
            get { return _fax; }
            set
            {
                _fax = value;
               
            }
        }


        public bool InWallet
        {
            get { return _InWallet; }
            set
            {
                _InWallet = value;
            }
        }


        #endregion

        #region Constructors

        public AddressInfo()
        {
        }

        public AddressInfo(
            string FirstName,
            string LastName,
            string Company,
            string Address1,
            string Address2,
            string City,
            string State,
            string ZipCode,
            string Country,
            string Phone,
            string Fax
            )
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Company = Company;
            this.Address1 = Address1;
            this.Address2 = Address2;
            this.City = City;
            this.State = State;
            this.Zip = ZipCode;
            this.Country = Country;
            this.Phone = Phone;
            this.Fax = Fax;
        }

        public AddressInfo(AddressInfo address)
        {
            this.FirstName = address.FirstName;
            this.LastName = address.LastName;
            this.Company = address.Company;
            this.Address1 = address.Address1;
            this.Address2 = address.Address2;
            this.City = address.City;
            this.State = address.State;
           
            this.Country = address.Country;
            this.CountryText = address.CountryText;
            this.Zip = address.Zip;
            this.Phone = address.Phone;
            this.Fax = address.Fax;
           

        }

        #endregion
        public override string ToString()
        {
            System.Text.StringBuilder sbAddress = new System.Text.StringBuilder();

            if ((_firstName.Length > 0) || (_lastName.Length > 0))
                sbAddress.Append(_firstName + " " + _lastName + "\r\n");

            if (_company.Length > 0)
                sbAddress.Append(_company + "\r\n");

            if (_address1.Length > 0)
                sbAddress.Append(_address1 + "\r\n");

            if (_address2.Length > 0)
                sbAddress.Append(_address2 + "\r\n");

          


            if (this.Country != null)
                sbAddress.Append(_Country);

            return sbAddress.ToString();

        }

    }

}
