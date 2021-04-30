using UnityEngine;
[ExecuteInEditMode]
public class SerializedLightmapSetting : MonoBehaviour
{
    [HideInInspector]
    public Texture2D[] lightmapFar, lightmapNear;
    [HideInInspector]
    public LightmapsMode mode;
#if UNITY_EDITOR
    public void OnEnable()
    {
        Debug.Log("[SerializedLightmapSetting] hook");
        UnityEditor.Lightmapping.completed += LoadLightmaps;
    }
    public void OnDisable()
    {
        Debug.Log("[SerializedLightmapSetting] unhook");
        UnityEditor.Lightmapping.completed -= LoadLightmaps;
    }
#endif
    public void Start()
    {
        if (Application.isPlaying)
        {
            LightmapSettings.lightmapsMode = mode;
            int l1 = (lightmapFar == null) ? 0 : lightmapFar.Length;
            int l2 = (lightmapNear == null) ? 0 : lightmapNear.Length;
            int l = (l1 < l2) ? l2 : l1;
            LightmapData[] lightmaps = null;
            if (l > 0)
            {
                lightmaps = new LightmapData[l];
                for (int i = 0; i < l; i++)
                {
                    lightmaps[i] = new LightmapData();
                    if (i < l1)
                        lightmaps[i].lightmapColor = lightmapFar[i];
                    if (i < l2)
                        lightmaps[i].lightmapColor = lightmapNear[i];
                }
            }
            LightmapSettings.lightmaps = lightmaps;
            Destroy(this);
        }
    }
#if UNITY_EDITOR
    public void LoadLightmaps()
    {
        mode = LightmapSettings.lightmapsMode;
        lightmapFar = null;
        lightmapNear = null;
        if (LightmapSettings.lightmaps != null && LightmapSettings.lightmaps.Length > 0)
        {
            int l = LightmapSettings.lightmaps.Length;
            lightmapFar = new Texture2D[l];
            lightmapNear = new Texture2D[l];
            for (int i = 0; i < l; i++)
            {
                lightmapFar[i] = LightmapSettings.lightmaps[i].lightmapColor;
                lightmapNear[i] = LightmapSettings.lightmaps[i].lightmapColor;
            }
        }
        MeshLightmapSetting[] savers = GameObject.FindObjectsOfType<MeshLightmapSetting>();
        foreach (MeshLightmapSetting s in savers)
        {
            s.SaveSettings();
        }
    }
#endif
}