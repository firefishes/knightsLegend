using ShipDock.Tools;
using System.Collections.Generic;
using UnityEditor;

namespace ShipDock.Editors
{
    public class ShipDockEditorData : Singletons<ShipDockEditorData>
    {
        public string platformPath;
        public BuildTarget buildPlatform;
        public UnityEngine.Object[] selections;
        public KeyValueList<string, List<ABAssetCreater>> ABCreaterMapper;
    }

}
