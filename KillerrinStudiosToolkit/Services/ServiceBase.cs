using KillerrinStudiosToolkit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KillerrinStudiosToolkit.Services
{
    public abstract class ServiceBase : ModelBase
    {
        protected bool m_serviceEnabled = true;
        public bool ServiceEnabled
        {
            get { return m_serviceEnabled; }
            protected set
            {
                if (value == true) EnableService();
                else DisableService();

                RaisePropertyChanged(nameof(ServiceEnabled));
            }
        }

        public virtual void EnableService()
        {
            m_serviceEnabled = true;
        }
        public virtual void DisableService()
        {
            m_serviceEnabled = false;
        }
    }
}
