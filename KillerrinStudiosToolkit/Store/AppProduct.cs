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
        public string ID { get; }

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

        private bool m_isConsumable = false;
        public bool IsConsumable
        {
            get { return m_isConsumable; }
            set
            {
                m_isConsumable = value;
                RaisePropertyChanged(nameof(IsConsumable));
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

        public AppProduct(string id) : this(id, false) { }
        public AppProduct(string id, bool purchased)
        {
            ID = id;
            Purchased = purchased;
            IsConsumable = false;
            ExpiryDate = DateTime.MaxValue;
        }
    }
}
