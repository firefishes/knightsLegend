using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public static class AppPaths
    {
        public static string resData = "res_data";
        public static string resDataRoot = resData.Append(StringUtils.PATH_SYMBOL);

        public static string StreamingResDataRoot { get; } = Application.streamingAssetsPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);
        public static string DataPathResDataRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);


    }

}