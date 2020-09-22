#define _COLOR_LOG

using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;
#if !RELEASE
using System.Runtime.InteropServices;
using UnityEngine.Profiling;
#endif

namespace ShipDock.Tools
{
    /// <summary>调试工具类</summary>
    public class DebugUtils
    {

        /// <summary>是否为调试模式</summary>
        public static bool isDebug = false;

        private static int logLine = 0;
        private static int logLineMax = 0;
        private static Text logText;

        /// <summary>打印某个对象的内存地址</summary>
        public static void LogMemory(System.Object debugObject, string log = "", UnityEngine.Object obj = null)
        {
            if (!isDebug)
            {
                return;
            }
            
            GCHandle gcHandle = GCHandle.Alloc(debugObject, GCHandleType.Pinned);
            IntPtr addr = gcHandle.AddrOfPinnedObject();
            //logContent.Append(log);
            //logContent.Append("0x");
            //logContent.Append(addr.ToString());
        }

        /// <summary>调试射线</summary>
        public static void DebugDrawRay(Vector3 origin, Vector3 direction)
        {
#if !RELEASE
            if (!isDebug)
            {
                return;
            }

            Ray ray = new Ray(origin, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 2000))
            {
                Debug.DrawLine(origin, hit.point, Color.green);
            }
#endif
        }

        public static void LogMemInfo(ref string content)
        {
            int MUnit = 1000000;
            StringBuilder sbd = new StringBuilder();
            content = "Mono Used: ".Append((Profiler.GetMonoUsedSizeLong() / MUnit).ToString(), "M\n");
            content = content.Append("All Memory: ", (Profiler.GetTotalAllocatedMemoryLong() / MUnit).ToString(), "M\n");
            content = content.Append("Un Used Reserved: ", (Profiler.GetTotalUnusedReservedMemoryLong() / MUnit).ToString(), "M\n");
        }

        private const string LOG_COLOR_TODO = "#5731F0";
        private const string LOG_COLOR_WARNING = "#E78D08";
        private const string LOG_COLOR_DEBUG = "#00BEEEFF";
        private const string LOG_COLOR_ERROR = "#EE0000";
        private const string LOG_COLOR_DEFAULT = "#FFFFFFFF";
        private const string COLOR_FORMAT = "<color=\"{0}\">{1}</color>";
        private const string COLOR_SYBMBOL = "#";
        private const string COLOR_LOG_MATCHER_TODO = "todo:";
        private const string COLOR_LOG_MATCHER_WARNING = "warning:";
        private const string COLOR_LOG_MATCHER_ERROR = "error:";
        private const string COLOR_LOG_MATCHER_LOG = "log:";
        private const string COLOR_LOG_MATCHER_TESTER = "Tester:";

        private static string colorInLog = string.Empty;
        private static string colorValueInLog = string.Empty;
        private static string firstElment = string.Empty;
        private static UnityEngine.Object logObjTarget;

        public static void LogInColorAndSignIt(UnityEngine.Object logTarget, params string[] contents)
        {
            logObjTarget = logTarget;
            LogInColor(contents);
        }

        public static void LogInColor(params string[] contents)
        {
            colorInLog = string.Empty;
            firstElment = contents[0].ToString();

            bool isSetColor = (contents[0] != null) && (firstElment.IndexOf(COLOR_SYBMBOL) != -1);
            int start = isSetColor ? 1 : 0;
#if !COLOR_LOG
            start = 0;
#endif
            int max = contents.Length;
            for (int i = start; i < max; i++)
            {
                colorInLog += contents[i];
            }

            colorValueInLog = isSetColor ? firstElment : string.Empty;
#if !COLOR_LOG
            colorValueInLog = string.Empty;
#endif
            bool isTODO = colorInLog.IndexOf(COLOR_LOG_MATCHER_TODO, StringComparison.OrdinalIgnoreCase) != -1;
            bool isWarning = colorInLog.IndexOf(COLOR_LOG_MATCHER_WARNING, StringComparison.OrdinalIgnoreCase) != -1;
            bool isLog = colorInLog.IndexOf(COLOR_LOG_MATCHER_LOG, StringComparison.OrdinalIgnoreCase) != -1;
            bool isError = colorInLog.IndexOf(COLOR_LOG_MATCHER_ERROR, StringComparison.OrdinalIgnoreCase) != -1;
            bool isTester = colorInLog.IndexOf(COLOR_LOG_MATCHER_TESTER, StringComparison.OrdinalIgnoreCase) != -1;
            if (string.IsNullOrEmpty(colorValueInLog))
            {
                if (isTODO)
                {
                    colorValueInLog = LOG_COLOR_TODO;
                }
                else if (isWarning)
                {
                    colorValueInLog = LOG_COLOR_WARNING;
                }
                else if (isError)
                {
                    colorValueInLog = LOG_COLOR_ERROR;
                }
#if COLOR_LOG
                else if (isLog)
                {
                    colorValueInLog = LOG_COLOR_DEBUG;
                }
                else
                {
                    colorValueInLog = LOG_COLOR_DEFAULT;
                }
#endif
            }

#if COLOR_LOG
            colorInLog = string.Format(COLOR_FORMAT, colorValueInLog, colorInLog);
#else
            if (isTODO || isError || isWarning)
            {
                colorInLog = string.Format(COLOR_FORMAT, colorValueInLog, colorInLog);
            }
            if (isSetColor)
            {
                colorInLog = colorInLog.Remove(0, 7);//去掉自定义的颜色
            }
#endif
            if(logObjTarget != null)
            {
                Debug.Log(colorInLog, logObjTarget);
                logObjTarget = null;
            }
            else
            {
                Debug.Log(colorInLog);
            }
        }

        public static void LogInColorAndLocation(UnityEngine.Object target, params string[] contents)
        {
#if !RELEASE
            LogInColor(contents);
            if (target != null)
            {
                Debug.Log("点击以定位", target);
            }
#endif
        }

        public static void SetLogTextUI(ref Text target)
        {
            logText = target;
        }

        public static void SetLogMaxLine(int value)
        {
            logLineMax = value;
        }
    }
}