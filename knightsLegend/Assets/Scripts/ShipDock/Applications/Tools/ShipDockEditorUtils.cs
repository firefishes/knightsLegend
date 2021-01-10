using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Editors
{
    public static class ShipDockEditorUtils
    {
#if UNITY_EDITOR
        /// <summary>
        /// 从编辑器项目面板中查找资源文件
        /// </summary>
        /// <typeparam name="T">要查找的文件泛型</typeparam>
        /// <param name="result">结果列表</param>
        /// <param name="filters">过滤器，Sample: "t:GameObject"</param>
        /// <param name="assetPaths">文件路径，Sample：sample: new string[] { @"Assets\Scripts\ShipDock\Applications\Prefabs" }</param>
        public static void FindAssetInEditorProject<T>(ref List<T> result, string filters, params string[] assetPaths) where T : Object
        {
            Debug.Log("Find asset in editor project: ");
            foreach(var i in assetPaths)
            {
                Debug.Log(i);
            }
            Debug.Log("Find end..");

            string[] guids = AssetDatabase.FindAssets(filters, assetPaths);//查找指定路径下指定类型的所有资源，返回的是资源GUID
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