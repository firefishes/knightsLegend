using System.Collections;
using System.Collections.Generic;
using ShipDock.Tools;
using UnityEditor;

namespace ShipDock.Editors
{
    public class ShipDockEditorData : Singletons<ShipDockEditorData>
    {
        public string outputRoot;
        public string platformPath;
        public BuildTarget buildPlatform;
        public UnityEngine.Object[] selections;
        public KeyValueList<string, List<ABAssetCreater>> ABCreaterMapper;
    }

}
