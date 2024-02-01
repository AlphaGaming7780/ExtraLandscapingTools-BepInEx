using System.Collections.Generic;
using System.Reflection;
using Colossal.IO.AssetDatabase;
using Game.Prefabs;
using Unity.Entities;

namespace ExtraLandscapingTools;

public abstract class Extension {

    public enum ExtensionType : int
    {
        Other = 0,
        Surfaces = 1,
        // Decals = 2,
        Assets = 2
    }

    public static Dictionary<string, Extension> Extensions {private set; get;} = [];

    protected abstract ExtensionType extensionType { get; }

    protected virtual void OnCreate() {
        Prefab.onAddPrefab += OnAddPrefab;
        ELT.onGetIcon += OnGetIcon;
    }

    public virtual void OnLoadLocalization(LocaleAsset localeAsset) {}
    public virtual void OnAddPrefab(PrefabBase prefabBase) {}
    public virtual string OnGetIcon(PrefabBase prefabBase) {return null;}

    public void Test() {
        Plugin.Logger.LogMessage(Assembly.GetExecutingAssembly().FullName);
    }

    public static void RegisterELTExtension( string extensionID , Extension extension ) {
        if(Extensions.ContainsKey(extensionID)) return;
        Extensions.Add(extensionID, extension);
        extension.OnCreate();
    }

}