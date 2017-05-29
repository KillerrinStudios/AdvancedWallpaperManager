using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;

namespace KillerrinStudiosToolkit.Store
{
    public class InAppPurchaseLegacyManager : InAppPurchaseManagerBase
    {
        private static InAppPurchaseLegacyManager m_instance;
        public static InAppPurchaseLegacyManager Instance
        {
            get
            {
                if (m_instance == null) m_instance = new InAppPurchaseLegacyManager();
                return m_instance;
            }
        }

        #region License Information
        public static LicenseInformation LicenseInfoContextual
        {
            get
            {
                if (DebugTools.DebugMode) return LicenseInfoSimulator;
                return LicenseInfoReal;
            }
        }
        public static LicenseInformation LicenseInfoSimulator { get { return CurrentAppSimulator.LicenseInformation; } }
        public static LicenseInformation LicenseInfoReal { get { return CurrentApp.LicenseInformation; } }
        #endregion

        #region GUID
        public static Guid GuidContextual
        {
            get
            {
                if (DebugTools.DebugMode) return GuidSimulator;
                return GuidReal;
            }
        }
        public static Guid GuidSimulator { get { return CurrentAppSimulator.AppId; } }
        public static Guid GuidReal { get { return CurrentApp.AppId; } }
        #endregion

        #region Uri
        public static Uri UriContextual
        {
            get
            {
                if (DebugTools.DebugMode) return UriSimulator;
                return UriReal;
            }
        }
        public static Uri UriSimulator { get { return CurrentAppSimulator.LinkUri; } }
        public static Uri UriReal { get { return CurrentApp.LinkUri; } }
        #endregion

        public InAppPurchaseLegacyManager()
        {
            //LicenseInfoContextual.LicenseChanged += LicenseInfoContextual_LicenseChanged;
        }
        public override void Dispose()
        {
            //LicenseInfoContextual.LicenseChanged -= LicenseInfoContextual_LicenseChanged;
        }

        public event EventHandler LicenseInfoContextualChanged;
        private void LicenseInfoContextual_LicenseChanged()
        {
            LicenseInfoContextualChanged?.Invoke(this, null);
        }

        public async Task<ListingInformation> GetListingInformationAsync()
        {
            try
            {
                if (DebugTools.DebugMode)
                    return await CurrentAppSimulator.LoadListingInformationAsync();

                return await CurrentApp.LoadListingInformationAsync();
            }
            catch (Exception ex)
            {
                DebugTools.PrintOutException(ex, "License Load failed");
                return null;
            }
        }

        public ProductLicense GetLicense(string id)
        {
            var license = LicenseInfoContextual.ProductLicenses[id];
            return license;
        }

        public async override Task RefreshAppProducts()
        {
            AppProducts.Clear();

            var listingInfo = await GetListingInformationAsync();
            foreach (var product in listingInfo.ProductListings)
            {
                var license = GetLicense(product.Value.ProductId);

                AppProduct appProduct = new AppProduct(license.ProductId, license.ProductId, false);
                appProduct.Name = product.Value.Name;
                appProduct.Description = product.Value.Description;
                appProduct.CurrentFormattedPrice = product.Value.FormattedPrice;
                appProduct.BaseFormattedPrice = product.Value.FormattedBasePrice;
                appProduct.ExpiryDate = license.ExpirationDate.DateTime;
                appProduct.Purchased = license.IsActive;
                if (license.IsConsumable)
                    appProduct.ProductType = ProductKind.Consumable;
                else appProduct.ProductType = ProductKind.Durable;

                AppProducts.Add(appProduct);
            }

            // Invoke the event
            InvokeAppProductsChanged();
        }
        public async override Task<AppProduct> UpdateProduct(AppProduct appProduct)
        {
            await Task.Run(() =>
            {
                // Attempt to get the license
                var license = GetLicense(appProduct.ProductID);
                if (license == null) return;

                // Update the product
                appProduct.ExpiryDate = license.ExpirationDate.DateTime;
                appProduct.Purchased = license.IsActive;
                if (license.IsConsumable)
                    appProduct.ProductType = ProductKind.Consumable;
                else appProduct.ProductType = ProductKind.Durable;

                // Invoke the event
                InvokeAppProductsChanged();
            });

            return appProduct;
        }

        public async override Task<AppProduct> PurchaseProduct(AppProduct appProduct)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                var results = await Purchase(appProduct.ProductID);
                appProduct = results.Item1;
            });

            return appProduct;
        }

        public async Task<(AppProduct, PurchaseResults)> Purchase(string id)
        {
            var license = GetLicense(id);

            // If the product does not exist, exit with null
            if (license == null)
            {
                Debug.WriteLine("This product does not exist");
                throw new ArgumentException($"A product by ProductID of {id} does not exist", nameof(AppProduct.ProductID));
                //return (null, null);
            }

            // If the license is valid, but we don't have the product, create the product
            var product = GetAppProductByProductID(id);
            if (product == null)
            {
                product = new AppProduct(id, id, false);
                AppProducts.Add(product);
            }

            // If the license is already active, simply return the product
            if (license.IsActive)
            {
                Debug.WriteLine("User already owns this product");
                return (product, null);
            }

            try
            {
                PurchaseResults purchaseResults;

                if (DebugTools.DebugMode)
                {
                    Debug.WriteLine("Debug mode active, Simulating product purchase");
                    purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync(id);
                    Debug.WriteLine("Finished Simulating");
                }
                else
                {
                    Debug.WriteLine("Requesting Product Purchase");
                    purchaseResults = await CurrentApp.RequestProductPurchaseAsync(id);
                    Debug.WriteLine("User finished interacting with purchase screen");
                }

                switch (purchaseResults.Status)
                {
                    case ProductPurchaseStatus.AlreadyPurchased:
                        Debug.WriteLine("User already owns this product");
                        MarkProductPurchased(product);
                        break;
                    case ProductPurchaseStatus.Succeeded:
                        Debug.WriteLine("User now owns this product");
                        MarkProductPurchased(product);
                        break;
                    case ProductPurchaseStatus.NotPurchased:
                        Debug.WriteLine("User chose to not purchase the product");
                        product.Purchased = false;
                        if (DebugTools.DebugMode)
                        {
                            Debug.WriteLine("Simulating Purchase");
                            MarkProductPurchased(product);
                        }
                        break;
                    case ProductPurchaseStatus.NotFulfilled:
                        Debug.WriteLine("A previous purchase was not fulfilled");
                        MarkProductPurchased(product);
                        break;
                    default:
                        Debug.WriteLine("An unknown response occurred. Please try again later");
                        break;
                }

                return (product, purchaseResults);
            }
            catch (Exception ex)
            {
                DebugTools.PrintOutException(ex, $"Product Purchase Error ({id})");
            }

            return (product, null);
        }
    }
}
