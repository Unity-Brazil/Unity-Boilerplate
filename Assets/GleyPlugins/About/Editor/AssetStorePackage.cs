namespace GleyPlugins
{
    using UnityEditor;
    using UnityEngine;

    public class AssetStorePackage
    {
        public GleyAssets asset;
        public AssetState assetState;
        public Texture2D texture;
        public string name;
        public string description;
        public string url;
        private readonly string textureName;

        public AssetStorePackage(GleyAssets asset, string name, string textureName, string description, string url)
        {
            this.asset = asset;
            this.name = name;
            this.description = description;
            this.url = url;
            this.textureName = textureName;
        }

        public void SetAssetState(AssetState newState)
        {
            assetState = newState;
        }

        public void LoadTexture()
        {
            texture = EditorGUIUtility.Load("Assets/GleyPlugins/About/Editor/Icons/" + textureName) as Texture2D;
        }
    }
}
