using System.Collections.Generic;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools;

public class JSONSurfacesMaterail {
    public Dictionary<string, float> Float = [];
    public Dictionary<string, Vector4> Vector = [];
    // public PrefabIdentifierInfo[] prefabIdentifierInfos = [];
    public List<PrefabIdentifierInfo> prefabIdentifierInfos = [];
}
