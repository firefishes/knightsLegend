using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class MeshLightmapSetting : MonoBehaviour
{
    [HideInInspector]
    public int lightmapIndex;
    [HideInInspector]
    public Vector4 lightmapScaleOffset;

    [SerializeField]
    private string m_InfoShower;

    public bool isReset;

    public void SaveSettings()
    {
        Renderer renderer = GetComponent<Renderer>();
        lightmapIndex = renderer.lightmapIndex;
        lightmapScaleOffset = renderer.lightmapScaleOffset;
    }

    public void LoadSettings()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer.lightmapScaleOffset == Vector4.zero)
        {
            renderer.lightmapIndex = lightmapIndex;
            renderer.lightmapScaleOffset = lightmapScaleOffset;
        }
        else
        {
            lightmapIndex = renderer.lightmapIndex;
            lightmapScaleOffset = renderer.lightmapScaleOffset;
        }
        m_InfoShower = "lightmapIndex:".Append(lightmapIndex.ToString(), ", lightmapScaleOffset:", lightmapScaleOffset.ToString());
    }

    void Start()
    {
        LoadSettings();
        //if (Application.isPlaying)
        //{
        //    Destroy(this);
        //}
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (isReset)
        {
            isReset = false;
            lightmapIndex = 0;
            lightmapScaleOffset = Vector4.zero;
            m_InfoShower = "光照贴图信息已经清空";
        }
        else
        {
            LoadSettings();
        }
    }
#endif
}