#if GleyIAPiOS || GleyIAPGooglePlay || GleyIAPAmazon
#define GleyIAPEnabled
#endif

using UnityEngine.Events;
using System.Collections.Generic;
using System;

#if GleyIAPEnabled
using UnityEngine;
using UnityEngine.Purchasing;
using GleyEasyIAP;
using UnityEngine.Purchasing.Security;
using System.Linq;
using System.Collections;

public class IAPManager :MonoBehaviour, IStoreListener
{
    internal bool debug;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    private List<StoreProduct> shopProducts;
    private ConfigurationBuilder builder;
    private UnityAction<IAPOperationStatus, string, List<StoreProduct>> OnInitComplete;
    private UnityAction<IAPOperationStatus, string, StoreProduct> OnCompleteMethod;
    private UnityAction RestoreDone;


    /// <summary>
    /// Static instance of the class for easy access
    /// </summary>
    private static IAPManager instance;
    public static IAPManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GleyIAPManager");
                instance = go.AddComponent<IAPManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }


    /// <summary>
    /// Initialize store products, Call this method once at the beginning of your game
    /// </summary>
    /// <param name="InitComplete">callback method, returns a list of all store products, use this method for initializations</param>
    public void InitializeIAPManager(UnityAction<IAPOperationStatus, string, List<StoreProduct>> InitComplete)
    {
        if (IsInitialized())
            return;

        IAPSettings settings = Resources.Load<IAPSettings>("IAPData");
        if (settings == null)
        {
            Debug.LogError("No products available -> Go to Window->Gley->Easy IAP and define your products");
            return;
        }
        shopProducts = settings.shopProducts;
        debug = settings.debug;
        if (debug)
        {
            Debug.Log(this + "Initialization Started");
            ScreenWriter.Write(this + "Initialization Started");
        }

        if (m_StoreController == null)
        {
            OnInitComplete = InitComplete;
            InitializePurchasing();
        }
    }


    /// <summary>
    /// Checks if shop is initialized 
    /// </summary>
    /// <returns>true if shop was already initialized</returns>
    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    /// <summary>
    /// call this method to buy a product
    /// </summary>
    /// <param name="productName">An enum member generated from Settings Window</param>
    /// <param name="OnCompleteMethod">callback method that returns the bought product details for initializations</param>
    public void BuyProduct(ShopProductNames productName, UnityAction<IAPOperationStatus, string, StoreProduct> OnCompleteMethod)
    {
        if (debug)
        {
            Debug.Log(this + "Buy Process Started for " + productName);
            ScreenWriter.Write(this + "Buy Process Started for " + productName);
        }

        this.OnCompleteMethod = OnCompleteMethod;

        for (int i = 0; i < shopProducts.Count; i++)
        {
            if (shopProducts[i].productName == productName.ToString())
            {
                BuyProductID(shopProducts[i].GetStoreID());
            }
        }
    }


    /// <summary>
    /// Restore previously bought products (Only required on iOS)
    /// </summary>
    /// <param name="OnCompleteMethod">called when the restore process is done</param>
    public void RestorePurchases(UnityAction<IAPOperationStatus, string, StoreProduct> OnCompleteMethod)
    {
        if (!IsInitialized())
        {
            if (debug)
            {
                Debug.Log(this + "RestorePurchases FAIL. Not initialized.");
                ScreenWriter.Write(this + "RestorePurchases FAIL. Not initialized.");
            }
            if (RestoreDone != null)
            {
                RestoreDone();
            }
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (debug)
            {
                Debug.Log(this + "RestorePurchases started ...");
                ScreenWriter.Write(this + "RestorePurchases started ...");
            }

            this.OnCompleteMethod = OnCompleteMethod;
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) =>
            {
                if (debug)
                {
                    Debug.Log(this + "RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                    ScreenWriter.Write(this + "RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                }
                if(RestoreDone!=null)
                {
                    RestoreDone();
                }
            });
        }
        else
        {
            if (debug)
            {
                Debug.Log(this + "RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
                ScreenWriter.Write(this + "RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
            if (RestoreDone != null)
            {
                RestoreDone();
            }
            if(OnCompleteMethod!=null)
            {
                OnCompleteMethod(IAPOperationStatus.Fail, "Not supported on this platform.Current = " + Application.platform, null);
            }
        }
    }


    /// <summary>
    /// Restore previously bought products (Only required on iOS)
    /// </summary>
    /// <param name="OnCompleteMethod">called when a product needs to be restored/param>
    /// <param name="RestoreDone">called when restore process is done/param>
    public void RestorePurchases(UnityAction<IAPOperationStatus, string, StoreProduct> OnCompleteMethod, UnityAction RestoreDone)
    {
        this.RestoreDone = RestoreDone;
        RestorePurchases(OnCompleteMethod);
    }


    /// <summary>
    /// Returns the type of the given product
    /// </summary>
    /// <param name="product"></param>
    /// <returns>Consumable/NonConsumable/Subscription</returns>
    public ProductType GetProductType(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).productType;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return 0;
        }
    }


    /// <summary>
    /// Get product reward
    /// </summary>
    /// <param name="product">store product</param>
    /// <returns>the amount of in game currency received</returns>
    public int GetValue(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).value;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return 0;
        }
    }


    /// <summary>
    /// Get the price and currency code of the product as a string
    /// </summary>
    /// <param name="product">store product</param>
    /// <returns>the localized price and currency of the product</returns>
    public string GetLocalizedPriceString(ShopProductNames product)
    {
        if (IsInitialized())
        {
            if (shopProducts != null)
            {
                return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).localizedPriceString;
            }
            else
            {
                Debug.LogError("No products available -> Go to Window->Gley->Easy IAP and define your products");
            }
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
        }
        return "-";
    }


    /// <summary>
    /// Get decimal product price denominated in the local currency
    /// </summary>
    /// <param name="product">store product</param>
    /// <returns></returns>
    public int GetPrice(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).price;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return 0;
        }
    }


    /// <summary>
    /// Get product currency in ISO 4217 format; e.g. GBP or USD.
    /// </summary>
    /// <param name="product">store product</param>
    /// <returns></returns>
    public string GetIsoCurrencyCode(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).isoCurrencyCode;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return "-";
        }
    }


    /// <summary>
    /// Get description from the store
    /// </summary>
    /// <param name="product">store product</param>
    /// <returns></returns>
    public string GetLocalizedDescription(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).localizedDescription;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return "-";
        }
    }


    /// <summary>
    /// Get title from the store
    /// </summary>
    /// <param name="product">store product</param>
    /// <returns></returns>
    public string GetLocalizedTitle(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).localizedTitle;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return "-";
        }
    }


    /// <summary>
    /// Gets the status of the product
    /// </summary>
    /// <param name="product"></param>
    /// <returns>true if the product was already bought</returns>
    public bool IsActive(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).active;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return false;
        }
    }


    /// <summary>
    /// Get additional info for subscription
    /// </summary>
    /// <param name="product">the subscription product</param>
    /// <returns>all infos available for the subscription</returns>
    public SubscriptionInfo GetSubscriptionInfo(ShopProductNames product)
    {
        if (IsInitialized())
        {
            return shopProducts.First(cond => String.Equals(cond.productName, product.ToString())).subscriptionInfo;
        }
        else
        {
            Debug.LogError("Not Initialized -> Call IAPManager.Instance.InitializeIAPManager() before anything else");
            return null;
        }
    }


    /// <summary>
    /// Converts a given name into enum member 
    /// </summary>
    /// <param name="name">string to convert</param>
    /// <returns>an enum member</returns>
    public ShopProductNames ConvertNameToShopProduct(string name)
    {
        return (ShopProductNames)Enum.Parse(typeof(ShopProductNames), name);
    }


    /// <summary>
    /// initializes the buy product process
    /// </summary>
    /// <param name="productId"></param>
    private void BuyProductID(string productId)
    {
        if (debug)
        {
            Debug.Log(this + "Buy product with id: " + productId);
            ScreenWriter.Write(this + "Buy product with id: " + productId);
        }

        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    ScreenWriter.Write(this + "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }

                if (OnCompleteMethod != null)
                {
                    OnCompleteMethod(IAPOperationStatus.Fail, "Not purchasing product, either is not found or is not available for purchase", null);
                }
            }
        }
        else
        {
            if (debug)
            {
                Debug.Log(this + "BuyProductID FAIL. Store not initialized.");
                ScreenWriter.Write(this + "BuyProductID FAIL. Store not initialized.");
            }

            if (OnCompleteMethod != null)
            {
                OnCompleteMethod(IAPOperationStatus.Fail, "Store not initialized.", null);
            }
        }
    }


    /// <summary>
    /// Initializes Unity IAP
    /// </summary>
    private void InitializePurchasing()
    {
        if (IsInitialized())
        {
            OnInitComplete(IAPOperationStatus.Success, "Already initialized", null);
            return;
        }

        builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < shopProducts.Count; i++)
        {
            builder.AddProduct(shopProducts[i].GetStoreID(), shopProducts[i].GetProductType());
        }

        if (debug)
        {
            Debug.Log("InitializePurchasing");
            ScreenWriter.Write("InitializePurchasing");
        }

        UnityPurchasing.Initialize(this, builder);
    }


    /// <summary>
    /// IStoreListener event handler called after initialization is done
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        if (debug)
        {
            Debug.Log("OnInitialized");
            ScreenWriter.Write("OnInitialized");
        }
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

        StartCoroutine(InitializeProducts());
    }

    IEnumerator InitializeProducts()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < shopProducts.Count; i++)
        {
            Product product = m_StoreController.products.WithID(shopProducts[i].GetStoreID());

            if (debug)
            {
                Debug.Log(this + product.metadata.localizedTitle + " is available " + product.availableToPurchase);
                ScreenWriter.Write(this + product.metadata.localizedTitle + " is available " + product.availableToPurchase);
            }

            if (shopProducts[i].productType == ProductType.Subscription)
            {
                if (product != null && product.hasReceipt)
                {
                    IAPSecurityException exception;
                    if (ReceiptIsValid(shopProducts[i].productName, product.receipt, out exception))
                    {
                        shopProducts[i].active = true;
                        string introJson = null;
                        SubscriptionManager p = new SubscriptionManager(product, introJson);
                        shopProducts[i].subscriptionInfo = p.getSubscriptionInfo();
                    }
                }
            }

            if (shopProducts[i].productType == ProductType.NonConsumable)
            {
                if (product != null && product.hasReceipt)
                {
                    IAPSecurityException exception;
                    if (ReceiptIsValid(shopProducts[i].productName, product.receipt, out exception))
                    {
                        shopProducts[i].active = true;
                    }
                }
            }

            if (product != null && product.availableToPurchase)
            {
                shopProducts[i].localizedPriceString = product.metadata.localizedPriceString;
                shopProducts[i].price = System.Decimal.ToInt32(product.metadata.localizedPrice);
                shopProducts[i].isoCurrencyCode = product.metadata.isoCurrencyCode;
                shopProducts[i].localizedDescription = product.metadata.localizedDescription;
                shopProducts[i].localizedTitle = product.metadata.localizedTitle;
            }
        }
        OnInitComplete(IAPOperationStatus.Success, "Success", shopProducts);
    }


    /// <summary>
    /// IStoreListener event handler called when initialization fails 
    /// </summary>
    /// <param name="error"></param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        if (debug)
        {
            Debug.Log("OnInitializeFailed");
            ScreenWriter.Write("OnInitializeFailed");
        }
        OnInitComplete(IAPOperationStatus.Fail, error.ToString(), null);
    }


    /// <summary>
    /// IStoreListener event  handler called when a purchase fails
    /// </summary>
    /// <param name="product"></param>
    /// <param name="reason"></param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        if (debug)
        {
            Debug.Log(this + "Buy Product failed for " + product.metadata.localizedTitle + " Failed. Reason: " + reason);
            ScreenWriter.Write(this + "Buy Product failed for " + product.metadata.localizedTitle + " Failed. Reason: " + reason);
        }

        if (OnCompleteMethod != null)
        {
            OnCompleteMethod(IAPOperationStatus.Fail, product.metadata.localizedTitle + " Failed. Reason: " + reason, null);
        }
    }


    /// <summary>
    /// IStoreListener event handler called when a purchase is done
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (debug)
        {
            Debug.Log(this + "Product bought " + e.purchasedProduct.definition.id);
            ScreenWriter.Write(this + "Product bought " + e.purchasedProduct.definition.id);
        }

        for (int i = 0; i < shopProducts.Count; i++)
        {
            if (String.Equals(e.purchasedProduct.definition.id, shopProducts[i].GetStoreID(), StringComparison.Ordinal))
            {
                IAPSecurityException exception;
                bool validPurchase = ReceiptIsValid(shopProducts[i].productName, e.purchasedProduct.receipt, out exception);
                if (validPurchase)
                {
                    if (shopProducts[i].productType == ProductType.Subscription || shopProducts[i].productType == ProductType.NonConsumable)
                    {
                        shopProducts[i].active = true;
                    }
                    if (OnCompleteMethod != null)
                    {
                        OnCompleteMethod(IAPOperationStatus.Success, "Purchase Successful", shopProducts[i]);
                    }
                }
                else
                {
                    if (OnCompleteMethod != null)
                    {
                        OnCompleteMethod(IAPOperationStatus.Fail, "Invalid Receipt " + exception.Message + exception.Data, null);
                    }
                }
                break;
            }
        }
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Receipt validation method
    /// </summary>
    /// <param name="productName"></param>
    /// <param name="receipt"></param>
    /// <param name="exception"></param>
    /// <returns>true if receipt is valid</returns>
    bool ReceiptIsValid(string productName, string receipt, out IAPSecurityException exception)
    {
        exception = null;
        bool validPurchase = true;
#if GleyUseValidation
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
    
        CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

        try
        {
            validator.Validate(receipt);
            if (debug)
            {
                Debug.Log(this + " Receipt is valid for " + productName);
                ScreenWriter.Write(this + " Receipt is valid for " + productName);
            }
        }
        catch (IAPSecurityException ex)
        {
            exception = ex;
            if (debug)
            {
                Debug.Log(this + " Receipt is NOT valid for " + productName);
                ScreenWriter.Write(this + " Receipt is NOT valid for " + productName);

            }
            validPurchase = false;
        }
#endif
#endif
        return validPurchase;
    }
}
#else

public class IAPManager
{
    internal bool debug;
    private static IAPManager instance;
    public static IAPManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new IAPManager();
            }
            return instance;
        }
    }

    internal bool IsInitialized()
    {
        return false;
    }

    public void InitializeIAPManager(UnityAction<IAPOperationStatus, string, List<StoreProduct>> InitComplete)
    {

    }

    public void BuyProduct(ShopProductNames productName, UnityAction<IAPOperationStatus, string, StoreProduct> OnCompleteMethod)
    {

    }

    public void RestorePurchases(UnityAction<IAPOperationStatus, string, StoreProduct> OnCompleteMethod)
    {

    }

    public void RestorePurchases(UnityAction<IAPOperationStatus, string, StoreProduct> OnCompleteMethod, UnityAction RestoreDone)
    {
    }
    public int GetValue(ShopProductNames product)
    {
        return 0;
    }

    public string GetLocalizedPriceString(ShopProductNames product)
    {
        return "";
    }

    public ShopProductNames ConvertNameToShopProduct(string name)
    {
        return 0;
    }

    public bool IsActive(ShopProductNames product)
    {
        return false;
    }

    internal string GetLocalizedDescription(ShopProductNames productToCheck)
    {
        return "";
    }

    internal string GetLocalizedTitle(ShopProductNames productToCheck)
    {
        return "";
    }

    internal string GetIsoCurrencyCode(ShopProductNames productToCheck)
    {
        return "";
    }
}
#endif
