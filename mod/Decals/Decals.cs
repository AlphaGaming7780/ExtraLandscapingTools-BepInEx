using Colossal.AssetPipeline;
using Colossal.IO.AssetDatabase;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools;

public class CustomDecals
{
    public static void CreateCustomDecals(RenderPrefab renderBasePrefab) {

        // StaticObjectPrefab staticObjectPrefab = (StaticObjectPrefab)ScriptableObject.CreateInstance("StaticObjectPrefab");
        // staticObjectPrefab.name = "TestDecals";

        // ObjectMeshInfo objectMeshInfo = new();
        // // RenderPrefab renderPrefab = (RenderPrefab)ScriptableObject.CreateInstance("RenderPrefab");
        
        // objectMeshInfo.m_Mesh = renderBasePrefab;

        // SubObjectDefaultProbability subObjectDefaultProbability = staticObjectPrefab.AddComponent<SubObjectDefaultProbability>();

        Material material = renderBasePrefab.ObtainMaterial(0);

        foreach(string s in material.GetPropertyNames(MaterialPropertyType.Float)) {Plugin.Logger.LogMessage($"Float : {s} | {material.GetFloat(s)}");}
        foreach(string s in material.GetPropertyNames(MaterialPropertyType.Vector)) {Plugin.Logger.LogMessage($"Vector : {s} | {material.GetVector(s)}");}
        foreach(string s in material.GetPropertyNames(MaterialPropertyType.Texture)) {Plugin.Logger.LogMessage($"Texture : {s}");}

        // ELT.m_PrefabSystem.AddPrefab(staticObjectPrefab);

    }
}