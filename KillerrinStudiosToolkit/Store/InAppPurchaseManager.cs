using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace KillerrinStudiosToolkit.Store
{
    public class InAppPurchaseManager
    {
        public static IReadOnlyList<AppProduct> AppProducts { get; protected set; }
        public static List<string> Whitelist { get; } = new List<string>();
        public static void UpdateProducts()
        {
            var newProducts = new List<AppProduct>();
            foreach (var license in LicenseInfoContextual.ProductLicenses.Values)
            {
                AppProduct product = new AppProduct(license.ProductId);
                product.Purchased = license.IsActive;
                product.ExpiryDate = license.ExpirationDate.DateTime;
                product.IsConsumable = license.IsConsumable;
                newProducts.Add(product);
            }

            AppProducts = newProducts;
            InvokeAppProductsChanged();
        }
        public static event EventHandler AppProductsChanged;
        private static void InvokeAppProductsChanged() { AppProductsChanged?.Invoke(AppProducts, null); }

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

        public InAppPurchaseManager()
        {
            //UpdateProducts();
            //LicenseInfoContextual.LicenseChanged += LicenseInfoContextual_LicenseChanged;
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

        public bool IsUserOnWhitelist(string id)
        {
            return Whitelist.Contains(id);
        }
        public AppProduct GetAppProduct(string id)
        {
            return AppProducts
                .Where(x => x.ID == id)
                .FirstOrDefault();
        }

        public ProductLicense GetLicense(string id)
        {
            var license = LicenseInfoContextual.ProductLicenses[id];
            return license;
        }

        public void UpdateProduct(string id)
        {
            var license = GetLicense(id);
            var product = GetAppProduct(id);

            product.ExpiryDate = license.ExpirationDate.DateTime;
            product.IsConsumable = license.IsConsumable;
            product.Purchased = license.IsActive;
            InvokeAppProductsChanged();
        }

        public async Task<(AppProduct, PurchaseResults)> Purchase(string id)
        {
            var license = GetLicense(id);

            // If the product does not exist, exit with null
            if (license == null)
            {
                Debug.WriteLine("This product does not exist");
                return (null, null);
            }

            // If the license is already active, simply return the product
            var product = GetAppProduct(id);
            if (product == null)
            {
                product = new AppProduct(id, false);
            }

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
                        product.Purchased = true;
                        break;
                    case ProductPurchaseStatus.Succeeded:
                        Debug.WriteLine("User now owns this product");
                        product.Purchased = true;
                        break;
                    case ProductPurchaseStatus.NotPurchased:
                        Debug.WriteLine("User chose to not purchase the product");
                        product.Purchased = false;
                        if (DebugTools.DebugMode)
                        {
                            Debug.WriteLine("Simulating Purchase");
                            product.Purchased = false;
                        }
                        break;
                    case ProductPurchaseStatus.NotFulfilled:
                        Debug.WriteLine("A previous purchase was not fulfilled");
                        product.Purchased = true;
                        break;
                    default:
                        Debug.WriteLine("An unknown response occurred");
                        break;
                }

                InvokeAppProductsChanged();
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
