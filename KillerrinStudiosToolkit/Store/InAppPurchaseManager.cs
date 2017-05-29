using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace KillerrinStudiosToolkit.Store
{
    public class InAppPurchaseManager : InAppPurchaseManagerBase
    {
        private static InAppPurchaseManager m_instance;
        public static InAppPurchaseManager Instance
        {
            get
            {
                if (m_instance == null) m_instance = new InAppPurchaseManager();
                return m_instance;
            }
        }

        public StoreContext Context { get; protected set; }
        public User ContextUser { get { return Context.User; } }

        public InAppPurchaseManager()
        {
            Context = StoreContext.GetDefault();
        }
        public InAppPurchaseManager(User user)
        {
            Context = StoreContext.GetForUser(user);
        }

        private List<ProductKind> GetProductKinds(bool includeApp = false)
        {
            List<ProductKind> kinds = new List<ProductKind>();
            if (includeApp)
            {
                kinds.Add(ProductKind.Application);
                kinds.Add(ProductKind.Game);
            }

            kinds.Add(ProductKind.Durable);
            kinds.Add(ProductKind.Consumable);
            kinds.Add(ProductKind.UnmanagedConsumable);
            return kinds;
        }

        public async override Task RefreshAppProducts()
        {
            var kinds = GetProductKinds();

            var appLicense = await Context.GetAppLicenseAsync();
            var result = await Context.GetAssociatedStoreProductsAsync(kinds.Select(x => x.ToString()));

            AppProducts.Clear();
            foreach (var item in result.Products)
            {
                Debug.WriteLine($"Key: {item.Key} | StoreID: {item.Value.StoreId} | ProductID: {item.Value.InAppOfferToken}");

                // Cache the value
                var storeProduct = item.Value;

                // Create the App Product
                AppProduct appProduct = new AppProduct(storeProduct.InAppOfferToken, storeProduct.StoreId);
                appProduct.Name = storeProduct.Title;
                appProduct.Description = storeProduct.Description;
                appProduct.CurrentFormattedPrice = storeProduct.Price.FormattedPrice;
                appProduct.BaseFormattedPrice = storeProduct.Price.FormattedBasePrice;

                // Parse the Product Type
                if (Enum.TryParse<ProductKind>(storeProduct.ProductKind, out ProductKind parsedProductKind))
                {
                    appProduct.ProductType = parsedProductKind;
                }
                else appProduct.ProductType = ProductKind.Unknown;

                // Add License Info
                appProduct.Purchased = storeProduct.IsInUserCollection;
                foreach (var license in appLicense.AddOnLicenses)
                {
                    Debug.WriteLine($"Licence Key: {license.Key} == Product Key: {item.Key}");
                    if (license.Key.Contains(item.Key))
                    {
                        appProduct.Purchased = license.Value.IsActive;
                        appProduct.ExpiryDate = license.Value.ExpirationDate.DateTime;
                        Debug.WriteLine($"Licence Found - Purchased: {appProduct.Purchased} | Expiry Date: {appProduct.ExpiryDate}");
                        break;
                    }
                }

                // Finally, add to the main list
                Debug.WriteLine(appProduct);
                AppProducts.Add(appProduct);
            }

            InvokeAppProductsChanged();
        }
        public async override Task<AppProduct> UpdateProduct(AppProduct appProduct)
        {
            var kinds = GetProductKinds();

            // Get the Store product
            var result = await Context.GetAssociatedStoreProductsAsync(kinds.Select(x => x.ToString()));
            var storeProduct = result.Products.Where(x => x.Value.StoreId == appProduct.StoreID).FirstOrDefault().Value;
            if (storeProduct == null) return null;

            // Get the License Information
            var appLicense = await Context.GetAppLicenseAsync();
            var storeProductLicense = appLicense.AddOnLicenses.Where(x => x.Value.InAppOfferToken == storeProduct.InAppOfferToken).FirstOrDefault().Value;
            if (storeProductLicense == null) return null;

            // Update the product
            appProduct.Name = storeProduct.Title;
            appProduct.Description = storeProduct.Description;
            appProduct.CurrentFormattedPrice = storeProduct.Price.FormattedPrice;
            appProduct.BaseFormattedPrice = storeProduct.Price.FormattedBasePrice;
            appProduct.ExpiryDate = storeProductLicense.ExpirationDate.DateTime;
            appProduct.Purchased = storeProductLicense.IsActive;

            // Invoke Changed and Return
            Debug.WriteLine(appProduct);
            InvokeAppProductsChanged();
            return appProduct;
        }

        public async override Task<AppProduct> PurchaseProduct(AppProduct appProduct)
        {
            if (appProduct == null)
            {
                Debug.WriteLine($"{nameof(appProduct)} is Null");
                throw new ArgumentNullException(nameof(appProduct));
                //return null;
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                try
                {
                    StorePurchaseProperties properties = new StorePurchaseProperties(appProduct.Name);
                    var purchaseResult = await Context.RequestPurchaseAsync(appProduct.StoreID);

                    switch (purchaseResult.Status)
                    {
                        case StorePurchaseStatus.Succeeded:
                            Debug.WriteLine("User now owns this product");
                            MarkProductPurchased(appProduct);
                            break;
                        case StorePurchaseStatus.AlreadyPurchased:
                            Debug.WriteLine("User already owns this product");
                            MarkProductPurchased(appProduct);
                            break;
                        case StorePurchaseStatus.NotPurchased:
                            Debug.WriteLine("User chose to not purchase the product");
                            break;
                        case StorePurchaseStatus.NetworkError:
                            Debug.WriteLine("A network error occurred. Please try again later");
                            break;
                        case StorePurchaseStatus.ServerError:
                            Debug.WriteLine("A server error occurred. Please try again later");
                            break;
                        default:
                            Debug.WriteLine("An unknown response occurred. Please try again later");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    DebugTools.PrintOutException(ex, $"Product Purchase Error ({appProduct.ProductID})");
                }
            });

            return appProduct;
        }
    }
}
