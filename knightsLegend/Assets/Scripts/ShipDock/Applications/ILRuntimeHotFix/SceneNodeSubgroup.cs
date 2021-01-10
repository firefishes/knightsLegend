using Sirenix.OdinInspector;
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
        [EnumPaging]
        public SceneNodeType valueType;
        [ShowIf("valueType", SceneNodeType.GAME_OBJECT)]
        public GameObject value;
        [ShowIf("valueType", SceneNodeType.ANI_CURVE)]
        public AnimationCurve animationCurve;
    }

}