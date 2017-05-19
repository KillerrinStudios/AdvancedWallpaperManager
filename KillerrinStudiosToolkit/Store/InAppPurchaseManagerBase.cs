using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace KillerrinStudiosToolkit.Store
{
    public abstract class InAppPurchaseManagerBase : IDisposable
    {
        public static List<AppProduct> AppProducts { get; } = new List<AppProduct>();
        public static List<string> Whitelist { get; } = new List<string>();

        public static event TypedEventHandler<InAppPurchaseManagerBase, List<AppProduct>> AppProductsChanged;
        protected void InvokeAppProductsChanged() { AppProductsChanged?.Invoke(this, AppProducts); }

        public bool IsUserOnWhitelist(string value)
        {
            return Whitelist.Contains(value);
        }

        public virtual void Dispose() { }

        public AppProduct GetAppProductByProductID(string productID)
        {
            productID = productID.ToUpper();
            return AppProducts
                .Where(x => x.ProductID.ToUpper().Equals(productID))
                .FirstOrDefault();
        }
        public AppProduct GetAppProductByStoreID(string storeID)
        {
            storeID = storeID.ToUpper();
            return AppProducts
                .Where(x => x.StoreID.ToUpper().Equals(storeID))
                .FirstOrDefault();
        }

        public abstract Task RefreshAppProducts();
        public abstract Task<AppProduct> UpdateProduct(AppProduct appProduct);
        public abstract Task<AppProduct> PurchaseProduct(AppProduct appProduct);

        protected void MarkProductPurchased(AppProduct appProduct)
        {
            appProduct.Purchased = true;
            InvokeAppProductsChanged();
        }
    }
}
