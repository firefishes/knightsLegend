#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
        UI_LAYOUT_GROUP,
        UI_TOGGLE,
        UI_TOGGLE_GROUP,
        UI_SLIDER,
        UI_SCROLL_BAR,
        UI_DROP_DOWN,
        UI_INPUT_FIELD,
        UI_CANVAS,
        UI_EVENT_SYSTEM,
        UI_EVENT_TRIGGER,
        TRANSFORM,
        MATERIAL,
        SPRITE_RENDERER,
        MESH_FILTER,
        AUDIO_SOURCE,
        ILRUNTIME_HOTFIX,
        ILRUNTIME_HOTFIX_UI,
    }

    [Serializable]
    public class SceneNodeSubgroup
    {
        public string keyField;
#if ODIN_INSPECTOR
        [EnumPaging, Indent(1)]
#endif
        public SceneNodeType valueType;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.GAME_OBJECT), Indent(1)]
#endif
        public GameObject value;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ANI_CURVE), Indent(1)]
#endif
        public AnimationCurve animationCurve;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ANIMATOR), Indent(1)]
#endif
        public Animator animator;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.CAMERA), Indent(1)]
#endif
        public Camera lens;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.SPRITE), Indent(1)]
#endif
        public Sprite sprite;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.TEXTURE), Indent(1)]
#endif
        public Texture texture;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_BUTTON), Indent(1)]
#endif
        public Button button;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_IMAGE), Indent(1)]
#endif
        public Image image;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_TEXT), Indent(1)]
#endif
        public Text Label;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_LAYOUT_GROUP), Indent(1)]
#endif
        public LayoutGroup layoutGroup;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_TOGGLE), Indent(1)]
#endif
        public UnityEngine.UI.Toggle toggle;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_TOGGLE_GROUP), Indent(1)]
#endif
        public ToggleGroup toggleGroup;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_SLIDER), Indent(1)]
#endif
        public Slider slider;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_SCROLL_BAR), Indent(1)]
#endif
        public Scrollbar scrollBar;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_DROP_DOWN), Indent(1)]
#endif
        public Dropdown dropDown;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_INPUT_FIELD), Indent(1)]
#endif
        public InputField inputField;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_CANVAS), Indent(1)]
#endif
        public Canvas canvas;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_EVENT_SYSTEM), Indent(1)]
#endif
        public EventSystem eventSystem;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_EVENT_TRIGGER), Indent(1)]
#endif
        public EventTrigger eventTrigger;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.TRANSFORM), Indent(1)]
#endif
        public Transform trans;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.MATERIAL), Indent(1)]
#endif
        public Material materialNode;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.SPRITE_RENDERER), Indent(1)]
#endif
        public SpriteRenderer spriteRendererNode;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.MESH_FILTER), Indent(1)]
#endif
        public MeshFilter meshFilterNode;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.AUDIO_SOURCE), Indent(1)]
#endif
        public AudioSource audioSource;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ILRUNTIME_HOTFIX), Indent(1)]
#endif
        public HotFixerComponent hotFixer;

#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ILRUNTIME_HOTFIX_UI), Indent(1)]
#endif
        public HotFixerUIAgent hotFixerUI;

    }

}