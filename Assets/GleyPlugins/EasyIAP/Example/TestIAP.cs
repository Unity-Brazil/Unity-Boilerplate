using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TestIAP : MonoBehaviour
{
    public class MyStoreProducts
    {
        public ShopProductNames name;
        public bool bought;

        public MyStoreProducts(ShopProductNames name, bool bought)
        {
            this.name = name;
            this.bought = bought;
        }
    }

    private List<MyStoreProducts> consumableProducts = new List<MyStoreProducts>();
    private List<MyStoreProducts> nonCOnsumableProducts = new List<MyStoreProducts>();
    private List<MyStoreProducts> subscriptions = new List<MyStoreProducts>();
    private Vector2 scrollViewVector = Vector2.zero;
    private float buttonWidth;
    private float buttonHeight;
    private float padding;
    private int nr;
    private int indexNumberConsumable;
    private int indexNumberNonConsumable;
    private int indexNumberSubscription;
    private int coins = 0;
    private bool purchaseInProgress;
    private bool initializationInProgress;
    private bool showConsumable;
    private bool showNonConsumable;
    private bool showSubscription;
    private bool showButtons = true;


    void Start()
    {
        buttonWidth = Screen.width;
        padding = Screen.width / 6;
    }


    void OnGUI()
    {
        buttonHeight = Screen.height / (12+nonCOnsumableProducts.Count+subscriptions.Count);
        GUI.skin.button.fontSize = 20;
        GUI.skin.label.fontSize = 25;
        GUI.skin.label.alignment = TextAnchor.LowerCenter;
        GUI.skin.textField.fontSize = 20;
        GUI.skin.textField.alignment = TextAnchor.MiddleCenter;

        nr = 0;
        if (showButtons)
        {
            if (!IAPManager.Instance.IsInitialized())
            {
                if (initializationInProgress == false)
                {
                    if (Button("Initialize"))
                    {
                        
                        initializationInProgress = true;
                        //Initialize IAP
                        IAPManager.Instance.InitializeIAPManager(InitializeResult);
                    }
                }
            }
            else
            {
                if (purchaseInProgress == false)
                {
                    #region Consumable
                    Label("Consumable");
                    if (consumableProducts.Count > 0)
                    {
                        //used to select a single consumable product from the list
                        if (Button("\\/   " + "Tap to select a consumable product" + "   \\/"))
                        {
                            if (!showConsumable)
                            {
                                showConsumable = true;
                            }
                            else
                            {
                                showConsumable = false;
                            }
                        }
                        if (showConsumable)
                        {
                            DropDown(7, ref indexNumberConsumable, ref showConsumable, consumableProducts);
                        }

                        if (Button("BUY " + consumableProducts[indexNumberConsumable].name + " " + IAPManager.Instance.GetValue(consumableProducts[indexNumberConsumable].name) + " Coins " + IAPManager.Instance.GetLocalizedPriceString(consumableProducts[indexNumberConsumable].name)))
                        {
                            purchaseInProgress = true;

                            //this function can be called like this
                            //IAPManager.Instance.BuyProduct(ShopProductNames.Consumable1, ProductBought);
                            IAPManager.Instance.BuyProduct(consumableProducts[indexNumberConsumable].name, ProductBought);
                        }
                    }
                    else
                    {
                        Label("No Consumable Products Defined");
                    }
                    #endregion


                    #region NonConsumable
                    Label("Non Consumable");
                    if (nonCOnsumableProducts.Count > 0)
                    {
                        //used to select a single NonConsumable product from the list
                        if (Button("\\/   " + "Tap to select a non consumable product" + "   \\/"))
                        {
                            if (!showNonConsumable)
                            {
                                showNonConsumable = true;
                            }
                            else
                            {
                                showNonConsumable = false;
                            }
                        }
                        if (showNonConsumable)
                        {
                            DropDown(6, ref indexNumberNonConsumable, ref showNonConsumable, nonCOnsumableProducts);
                        }

                        if (Button("BUY " + nonCOnsumableProducts[indexNumberNonConsumable].name + " " + IAPManager.Instance.GetLocalizedPriceString(nonCOnsumableProducts[indexNumberNonConsumable].name)))
                        {
                            purchaseInProgress = true;

                            //this function can be called like this
                            //IAPManager.Instance.BuyProduct(ShopProductNames.NonConsumable1, ProductBought);
                            IAPManager.Instance.BuyProduct(nonCOnsumableProducts[indexNumberNonConsumable].name, ProductBought);
                        }
                    }
                    else
                    {
                        Label("No Non Consumable Products Defined");
                    }
                    #endregion


                    #region Subscription
                    Label("Subscription");
                    if (subscriptions.Count > 0)
                    {
                        //used to select a single Subscription from the list
                        if (Button("\\/   " + "Tap to select a subscription" + "   \\/"))
                        {
                            if (!showSubscription)
                            {
                                showSubscription = true;
                            }
                            else
                            {
                                showSubscription = false;
                            }
                        }
                        if (showSubscription)
                        {
                            DropDown(2, ref indexNumberSubscription, ref showSubscription, subscriptions);
                        }


                        if (Button("BUY " + subscriptions[indexNumberSubscription].name + " " + IAPManager.Instance.GetLocalizedPriceString(subscriptions[indexNumberSubscription].name)))
                        {
                            purchaseInProgress = true;

                            //this function can be called like this
                            //IAPManager.Instance.BuyProduct(ShopProductNames.Subscription1, ProductBought);
                            IAPManager.Instance.BuyProduct(subscriptions[indexNumberSubscription].name, ProductBought);
                        }
                    }
                    else
                    {
                        Label("No Subscription Defined");
                    }
                    #endregion

                    if (Button("Restore Purchases"))
                    {
                        IAPManager.Instance.RestorePurchases(ProductBought,RestoreDone);
                    }


                    //test variables to see if the products work
                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                    Label("Coins: " + coins);
                    for (int i = 0; i < nonCOnsumableProducts.Count; i++)
                    {
                        Label(nonCOnsumableProducts[i].name + " was bought " + nonCOnsumableProducts[i].bought);
                    }
                    for (int i = 0; i < subscriptions.Count; i++)
                    {
                        Label(subscriptions[i].name + " was bought " + subscriptions[i].bought);
                    }
                }
            }
        }

        //hide buttons to see the debug messages
        if (GUI.Button(new Rect(0, Screen.height - buttonHeight, Screen.width / 3, buttonHeight), "Hide/Show Buttons"))
        {
            showButtons = !showButtons;
        }
    }

    private void RestoreDone()
    {
        if (IAPManager.Instance.debug)
        {
            Debug.Log("Restore done");
            GleyEasyIAP.ScreenWriter.Write("Restore done");
        }
    }

    /// <summary>
    /// automatically called after one product is bought
    /// </summary>
    /// <param name="status">The purchase status: Success/Failed</param>
    /// <param name="message">Error message if status is failed</param>
    /// <param name="product">the product that was bought, use the values from shop product to update your game data</param>
    private void ProductBought(IAPOperationStatus status, string message, StoreProduct product)
    {
        purchaseInProgress = false;
        if (status == IAPOperationStatus.Success)
        {
            if (IAPManager.Instance.debug)
            {
                Debug.Log("Buy product completed: " + product.localizedTitle + " receive value: " + product.value);
                GleyEasyIAP.ScreenWriter.Write("Buy product completed: " + product.localizedTitle + " receive value: " + product.value);
            }

            //each consumable gives coins in this example
            if (product.productType == ProductType.Consumable)
            {
                coins += product.value;
            }

            //non consumable Unlock Level 1 -> unlocks level 1 so we set the corresponding bool to true
            if (product.productName == "UnlockLevel1")
            {
                //unlockLevel1 = true;
            }

            //non consumable Unlock Level 2 -> unlocks level 2 so we set the corresponding bool to true
            if (product.productName == "UnlockLevel2")
            {
                //unlockLevel2 = true;
            }

            //subscription has been bought so we set our subscription variable to true
            if (product.productName == "Subscription")
            {
                //subscription = true;
            }

            if(product.productType == ProductType.NonConsumable)
            {
                if (product.active)
                {
                    nonCOnsumableProducts.First(cond => cond.name.ToString() == product.productName).bought = true;
                }
            }
            if (product.productType == ProductType.Subscription)
            {
                if (product.active)
                {
                    subscriptions.First(cond => cond.name.ToString() == product.productName).bought = true;
                }
            }
        }
        else
        {
            //en error occurred in the buy process, log the message for more details
            if (IAPManager.Instance.debug)
            {
                Debug.Log("Buy product failed: " + message);
                GleyEasyIAP.ScreenWriter.Write("Buy product failed: " + message);
            }
        }
    }

    /// <summary>
    /// automatically called after initialization is success
    /// </summary>
    /// <param name="status">The initialization result: Success/Failed</param>
    /// <param name="message">Error message if status is failed</param>
    /// <param name="shopProducts">list of all shop products, can be used to unlock update some game values</param>
    private void InitializeResult(IAPOperationStatus status, string message, List<StoreProduct> shopProducts)
    {
        initializationInProgress = false;
        consumableProducts = new List<MyStoreProducts>();
        nonCOnsumableProducts = new List<MyStoreProducts>();
        subscriptions = new List<MyStoreProducts>();

        if (status == IAPOperationStatus.Success)
        {
            //IAP was successfully initialized
            //loop through all products and check which one are bought to update our variables
            for (int i = 0; i < shopProducts.Count; i++)
            {
                if (shopProducts[i].productName == "UnlockLevel1")
                {
                    //if a product is active, means that user had already bought that product so enable access
                    if (shopProducts[i].active)
                    {
                        //unlockLevel1 = true;
                    }
                }
                if (shopProducts[i].productName == "UnlockLevel2")
                {
                    if (shopProducts[i].active)
                    {
                        //unlockLevel2 = true;
                    }
                }
                if (shopProducts[i].productName == "Subscription")
                {
                    //if a subscription is active means that the subscription is still valid so enable access
                    if (shopProducts[i].active)
                    {
                        //subscription = true;
                    }
                }

                //construct a different list of each category of products, for an easy display purpose, not required
                switch (shopProducts[i].productType)
                {
                    case ProductType.Consumable:
                        consumableProducts.Add(new MyStoreProducts(IAPManager.Instance.ConvertNameToShopProduct(shopProducts[i].productName), shopProducts[i].active));
                        break;
                    case ProductType.NonConsumable:
                        nonCOnsumableProducts.Add(new MyStoreProducts(IAPManager.Instance.ConvertNameToShopProduct(shopProducts[i].productName), shopProducts[i].active));
                        break;
                    case ProductType.Subscription:
                        subscriptions.Add(new MyStoreProducts(IAPManager.Instance.ConvertNameToShopProduct(shopProducts[i].productName), shopProducts[i].active));
                        break;
                }
            }
        }
        else
        {
            //Error initializing IAP
        }

        if (IAPManager.Instance.debug)
        {
            Debug.Log("Init status: " + status + " message " + message);
            GleyEasyIAP.ScreenWriter.Write("Init status: " + status + " message " + message);
        }
    }


    //custom GUI button
    bool Button(string label)
    {
        nr++;
        return GUI.Button(new Rect(0, (nr - 1) * buttonHeight, buttonWidth, buttonHeight), label);
    }


    //custom GUI label
    void Label(string label)
    {
        GUI.Label(new Rect(0, nr * buttonHeight, buttonWidth, buttonHeight), label);
        nr++;
    }


    //custom drop down
    void DropDown(int maxButtons, ref int indexNumber, ref bool showDropDown, List<MyStoreProducts> coll)
    {
        Rect dropDownRect = new Rect(0, 0 * buttonHeight, buttonWidth, maxButtons * buttonHeight);
        scrollViewVector = GUI.BeginScrollView(new Rect(dropDownRect.x, (dropDownRect.y + nr * buttonHeight), dropDownRect.width, dropDownRect.height), scrollViewVector, new Rect(0, 0, dropDownRect.width - 100, Mathf.Max(dropDownRect.height, (coll.Count * buttonHeight))));
        float min = Mathf.Min(dropDownRect.height, (coll.Count * buttonHeight));
        for (int index = 0; index < coll.Count; index++)
        {

            if (GUI.Button(new Rect(padding, (index * buttonHeight), buttonWidth - 2 * padding, buttonHeight), coll[index].name + " " + IAPManager.Instance.GetLocalizedPriceString(coll[index].name)))
            {
                showDropDown = false;
                indexNumber = index;
            }
            if (index < min / buttonHeight)
            {
                nr++;
            }
        }
        GUI.EndScrollView();
    }
}
