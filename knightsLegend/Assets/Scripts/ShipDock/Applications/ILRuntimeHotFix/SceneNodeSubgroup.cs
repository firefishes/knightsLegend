#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public enum SceneNodeType
    {
        GAME_OBJECT = 1,
        ANI_CURVE,
    }

    [Serializable]
    public class SceneNodeSubgroup
    {
        public string keyField;
#if ODIN_INSPECTOR
        [EnumPaging]
#endif
        public SceneNodeType valueType;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.GAME_OBJECT)]
#endif
        public GameObject value;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ANI_CURVE)]
#endif
        public AnimationCurve animationCurve;
    }

}