using KillerrinStudiosToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillerrinStudiosToolkit.Store
{
    public class AppProduct : ModelBase
    {
        public string ProductID { get; }
        public string StoreID { get; }

        private ProductKind m_productType = ProductKind.Durable;
        public ProductKind ProductType
        {
            get { return m_productType; }
            set
            {
                m_productType = value;
                RaisePropertyChanged(nameof(ProductType));
            }
        }

        private bool m_purchased = false;
        public bool Purchased
        {
            get { return m_purchased; }
            set
            {
                m_purchased = value;
                RaisePropertyChanged(nameof(Purchased));
            }
        }

        private DateTime m_expiryDate = DateTime.MaxValue;
        public DateTime ExpiryDate
        {
            get { return m_expiryDate; }
            set
            {
                m_expiryDate = value;
                RaisePropertyChanged(nameof(ExpiryDate));
            }
        }

        private string m_name = "";
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string m_description = "";
        public string Description
        {
            get { return m_description; }
            set
            {
                m_description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        private string m_baseFormattedPrice = "0.00";
        public string BaseFormattedPrice
        {
            get { return m_baseFormattedPrice; }
            set
            {
                m_baseFormattedPrice = value;
                RaisePropertyChanged(nameof(BaseFormattedPrice));
            }
        }
        private string m_currentFormattedPrice = "0.00";
        public string CurrentFormattedPrice
        {
            get { return m_currentFormattedPrice; }
            set
            {
                m_currentFormattedPrice = value;
                RaisePropertyChanged(nameof(CurrentFormattedPrice));
            }
        }


        public AppProduct(string productID, string storeID) : this(productID, storeID, false) { }
        public AppProduct(string productID, string storeID, bool purchased)
        {
            ProductID = productID;
            StoreID = storeID;
            Purchased = purchased;
            ProductType = ProductKind.Durable;
            ExpiryDate = DateTime.MaxValue;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
