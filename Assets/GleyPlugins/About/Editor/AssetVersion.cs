namespace GleyPlugins
{
    [System.Serializable]
    public class AssetVersion
    {
        public GleyAssets assetName;
        public string longVersion;
        public int shortVersion;

        public AssetVersion(GleyAssets assetName)
        {
            this.assetName = assetName;
        }
    }
}

