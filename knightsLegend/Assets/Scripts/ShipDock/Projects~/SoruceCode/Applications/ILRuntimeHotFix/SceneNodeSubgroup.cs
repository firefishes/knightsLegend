#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock.Applications
{
    public enum SceneNodeType
    {
        GAME_OBJECT = 1,
        ANI_CURVE,
        SPRITE,
        TEXTURE,
        CAMERA,
        ANIMATOR,
        UI_TEXT,
        UI_BUTTON,
        UI_IMAGE,
    }

    [Serializable]
    public class SceneNodeSubgroup
    {
        public string keyField;
#if ODIN_INSPECTOR
        [EnumPaging]
        [Indent(1)]
#endif
        public SceneNodeType valueType;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.GAME_OBJECT)]
        [Indent(1)]
#endif
        public GameObject value;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ANI_CURVE)]
        [Indent(1)]
#endif
        public AnimationCurve animationCurve;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ANIMATOR)]
        [Indent(1)]
#endif
        public Animator animator;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.CAMERA)]
        [Indent(1)]
#endif
        public Camera lens;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.SPRITE)]
        [Indent(1)]
#endif
        public Sprite sprite;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.TEXTURE)]
        [Indent(1)]
#endif
        public Texture texture;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_BUTTON)]
        [Indent(1)]
#endif
        public Button button;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_IMAGE)]
        [Indent(1)]
#endif
        public Image image;
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_TEXT)]
        [Indent(1)]
#endif
        public Text Label;
    }

}