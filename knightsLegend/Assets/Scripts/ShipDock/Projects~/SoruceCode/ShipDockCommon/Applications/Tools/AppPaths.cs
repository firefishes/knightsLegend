using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public static class AppPaths
    {
        public static string resData = "res_data";
        public static string confData = "conf_data";
        public static string outputPath = "res_data_output~/";
        public static string resDataRoot = resData.Append(StringUtils.PATH_SYMBOL);//, "{0}/");
        public static string confDataRoot = confData.Append(StringUtils.PATH_SYMBOL);//, "{0}/");

        public static string ABBuildOutput { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, outputPath);
        public static string ABBuildOutputRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, outputPath, resDataRoot);
        public static string ABBuildOutputTempRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, outputPath, resDataRoot);//需要做字符串格式化
        public static string StreamingResDataRoot { get; } = Application.streamingAssetsPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);
        public static string DataPathResDataRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);
        public static string DataPathConfDataRoot { get; } = Application.dataPath.Append(StringUtils.PATH_SYMBOL, confDataRoot);
        public static string PersistentResDataRoot { get; set; } = Application.persistentDataPath.Append(StringUtils.PATH_SYMBOL, resDataRoot);


    }

}