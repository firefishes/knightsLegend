using System.Collections.Generic;
using System.Text;
using UnityEngine;

static public class ShipDockExtension
{

    private static StringBuilder mBuilder;
    private static Ray rayForMainCamera;
    private static Transform cameraTF;

    public static string Append(this string target, params string[] args)
    {
        if (mBuilder == null)
        {
            mBuilder = new StringBuilder();
        }
        mBuilder.Length = 0;
        mBuilder.Append(target);

        int max = args.Length;
        string temp;
        for (int i = 0; i < max; i++)
        {
            temp = args[i];
            mBuilder.Append(temp);
        }
        return mBuilder.ToString();
    }

    public static string Joins(this string[] target, string symbol = ",")
    {
        string connector;
        string result = string.Empty;
        int max = target.Length;
        string temp;
        for (int i = 0; i < max; i++)
        {
            connector = (i == max - 1) ? string.Empty : symbol;
            temp = target[i];
            result = result.Append(temp, connector);
        }
        return result;
    }

    public static string Joins(this List<string> target, string symbol = ",")
    {
        string connector;
        string result = string.Empty;
        int max = target.Count;
        string temp;
        for (int i = 0; i < max; i++)
        {
            connector = (i == max - 1) ? string.Empty : symbol;
            temp = target[i];
            result = result.Append(temp, connector);
        }
        return result;
    }

    public static List<T> Contact<T>(this List<T> target, List<T> list)
    {
        int max = (list != default) ? list.Count : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target;
    }
    
    public static List<T> Contact<T>(this List<T> target, T[] list)
    {
        int max = (list != default) ? list.Length : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target;
    }

    public static T[] ContactToArr<T>(this List<T> target, List<T> list)
    {
        int max = (list != default) ? list.Count : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target.ToArray();
    }

    public static T[] ContactToArr<T>(this List<T> target, T[] list)
    {
        int max = (list != default) ? list.Length : 0;
        T temp;
        for (int i = 0; i < max; i++)
        {
            temp = list[i];
            target.Add(temp);
        }
        return target.ToArray();
    }

    public static void ContactToArr<T>(this T[] target, T[] list, out T[] result)
    {
        result = default;
        if (list == default || list.Length < 0)
        {
            return;
        }
        int oldLen = target.Length;
        int contactLen = list.Length;
        int max = oldLen + contactLen;

        result = new T[max];
        for (int i = 0; i < max; i++)
        {
            if(i < oldLen)
            {
                result[i] = target[i];
            }
            else if(i >= oldLen && i < max)
            {
                result[i] = list[i - oldLen];
            }
        }
    }

    public static Material GetMaterial(this Renderer target, bool isGetShareMat = true, bool isCheckMultMat = false, int index = -1)
    {
        if (isCheckMultMat && index >= 0)
        {
            Material[] list;
            if (isGetShareMat)
            {
                list = target.sharedMaterials;
                return list[index];
            }
            else
            {
                list = target.materials;
                return list[index];
            }
        }
        else
        {
            return isGetShareMat? target.sharedMaterial: target.material;
        }
    }

    public static void ResetMain(this Camera target)
    {
        cameraTF = default;
    }

    public static bool CameraRaycast(this Camera target, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask)
    {
        if (cameraTF == null)
        {
            cameraTF = Camera.main.transform;
        }
        rayForMainCamera = new Ray(cameraTF.position, direction);
        bool result = Physics.Raycast(rayForMainCamera, out hitInfo, distance, layerMask);
#if UNITY_EDITOR
        Debug.DrawRay(cameraTF.position, direction, Color.yellow);
#endif
        return result;
    }

    public static Color SetAlpha(this Color target, float a = 1f)
    {
        return (target.a == a) ? target : new Color(target.r, target.g, target.b, a);
    }
}