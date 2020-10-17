using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Tools
{
    public static class ShipDockEditorUtils
    {
#if UNITY_EDITOR
        public static void FindAssetInEditorProject<T>(ref List<T> result, string filters, params string[] assetPaths) where  T : Object
        {
            //查找指定路径下指定类型的所有资源，返回的是资源GUID
            //"t:GameObject", new string[] { "Assets/Resources/UI" }
            string[] guids = AssetDatabase.FindAssets(filters, assetPaths);
            //从GUID获得资源所在路径
            List<string> paths = new List<string>();
            int max = guids.Length;
            for (int i = 0; i < max; i++)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
            }
            //从路径获得该资源
            max = paths.Count;
            result = new List<T>();
            for (int i = 0; i < max; i++)
            {
                var item = AssetDatabase.LoadAssetAtPath(paths[i], typeof(T));
                result.Add((T)item);
            }
        }

        public static Transform CreateGameObjectWithComponent<T>(string name, Transform parent = default) where T : Component
        {
            Transform result = default;
            if (GameObject.Find(name) == default)
            {
                GameObject target = new GameObject
                {
                    name = name
                };
                target.AddComponent<T>();
                result = target.transform;
                if (parent != default)
                {
                    result.SetParent(parent);
                }
            }
            else
            {
                "Editor ShipDock/Create Application: {0} has existed.".Log(name);
            }
            return result;
        }
#endif
    }
}