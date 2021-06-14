#if USE_BOLT_SUPPORT
namespace GleyEasyIAP
{
    using Bolt;
    using Ludiq;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [IncludeInSettings(true)]
    public class EasyIAPBoltSupport
    {
        private static GameObject initializeEventTarget;
        private static GameObject buyEventTarget;
        private static GameObject restoreEventTarget;

        public static void InitializeIAP(GameObject _eventTarget)
        {
            initializeEventTarget = _eventTarget;
            IAPManager.Instance.InitializeIAPManager(InitializeResult);
        }

        private static void InitializeResult(IAPOperationStatus status, string message, List<StoreProduct> products)
        {

            if (status == IAPOperationStatus.Fail)
            {
                Debug.Log(message);
                CustomEvent.Trigger(initializeEventTarget, "InitComplete", false);
            }
            else
            {
                CustomEvent.Trigger(initializeEventTarget, "InitComplete", true);
            }
        }

        public static void RestorePurchases(GameObject _restoreEventTarget)
        {
            restoreEventTarget = _restoreEventTarget;
            IAPManager.Instance.RestorePurchases(ProductBought, RestoreDone);
        }

        private static void RestoreDone()
        {
            CustomEvent.Trigger(restoreEventTarget, "RestoreDone");
        }

        private static void ProductBought(IAPOperationStatus status, string error, StoreProduct product)
        {
            Debug.Log("Product Restored");
        }

        public static bool CheckIfBought(ShopProductNames productToCheck)
        {
            if (IAPManager.Instance.IsInitialized())
            {
                if (IAPManager.Instance.IsActive(productToCheck))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsInitialized()
        {
            return IAPManager.Instance.IsInitialized();
        }

        public static int GetProductValue(ShopProductNames productToCheck)
        {
            if (IAPManager.Instance.IsInitialized())
            {
                return IAPManager.Instance.GetValue(productToCheck);
            }

            return 0;
        }

        public static string GetStorePrice(ShopProductNames productToCheck)
        {
            if (IAPManager.Instance.IsInitialized())
            {
                return IAPManager.Instance.GetLocalizedPriceString(productToCheck);
            }
            return "-";
        }

        public static string GetIsoCurrencyCode(ShopProductNames productToCheck)
        {
            if(IAPManager.Instance.IsInitialized())
            {
                return IAPManager.Instance.GetIsoCurrencyCode(productToCheck);
            }
            return "-";
        }

        public static void BuyProduct(GameObject _eventTarget, ShopProductNames productToBuy)
        {
            buyEventTarget = _eventTarget;
            if (IAPManager.Instance.IsInitialized())
            {
                IAPManager.Instance.BuyProduct(productToBuy, BuyComplete);
            }
            else
            {
                CustomEvent.Trigger(buyEventTarget, "BuyComplete", false);
            }
        }

       

        private static void BuyComplete(IAPOperationStatus status, string message, StoreProduct product)
        {
            if (status == IAPOperationStatus.Success)
            {
                CustomEvent.Trigger(buyEventTarget, "BuyComplete", true);
            }
            else
            {
                CustomEvent.Trigger(buyEventTarget, "BuyComplete", false);
            }
        }
    }
}
#endif
