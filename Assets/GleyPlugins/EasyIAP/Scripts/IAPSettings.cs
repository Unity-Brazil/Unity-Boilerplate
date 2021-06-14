namespace GleyEasyIAP
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Used to save user settings made from Settings Window
    /// </summary>
    public class IAPSettings : ScriptableObject
    {
        public bool debug;
        public bool useReceiptValidation;
        public bool usePlaymaker;
        public bool useBolt;
        public bool useGameFlow;
        public bool useForGooglePlay;
        public bool useForAmazon;
        public bool useForIos;
        public List<StoreProduct> shopProducts = new List<StoreProduct>();
        
    }
}
