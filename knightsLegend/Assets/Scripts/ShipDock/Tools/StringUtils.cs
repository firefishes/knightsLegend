using System;

namespace ShipDock.Tools
{
    static public class StringUtils
    {
        public const string DOT = ".";
        public const string PATH_SYMBOL = "/";
        public const char PATH_SYMBOL_CHAR = '/';

        public static string GetQualifiedClassName(object target)
        {
            if(target == null)
            {
                return string.Empty;
            }
            Type type = target.GetType();
            return type.FullName;
        }
    }

}
