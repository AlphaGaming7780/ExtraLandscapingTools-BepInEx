using System;

namespace ExtraLandscapingTools.UI;

[Serializable]
public class AssetCat {
    public string catName;
    public string icon = ELT.GetIcon(null);
    internal bool selected = false;

    public AssetCat(string catName, string icon) {
        this.catName = catName;
        this.icon = icon;
    }

    public AssetCat()
    {
    }

}