using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Services.Store;

namespace KillerrinStudiosToolkit.Store
{
    public static class InAppPurchaseManagerFactory
    {
        public static InAppPurchaseManagerBase Create(bool useInstance = true)
        {
            if (HasRequiredAPIInformation())
            {
                if (useInstance)
                    return InAppPurchaseManager.Instance;
                return new InAppPurchaseManager();
            }

            if (useInstance)
                return InAppPurchaseLegacyManager.Instance;
            return new InAppPurchaseLegacyManager();
        }

        public static bool HasRequiredAPIInformation()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Services.Store.StoreContract", 1, 0) || ApiInformation.IsTypePresent(typeof(StoreContext).FullName))
            {
                return true;
            }
            return false;
        }
    }
}
