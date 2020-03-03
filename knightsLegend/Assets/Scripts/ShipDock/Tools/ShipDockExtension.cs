using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

static public class ShipDockExtension
{

    private static StringBuilder mBuilder;

    public static string Append(this string target, params string[] args)
    {
        if (mBuilder == null)
        {
            mBuilder = new StringBuilder();
        }
        mBuilder.Length = 0;
        mBuilder.Append(target);

        int max = args.Length;
        for (int i = 0; i < max; i++)
        {
            mBuilder.Append(args[i]);
        }
        return mBuilder.ToString();
    }

    public static string Joins(this string[] target, string symbol = ",")
    {
        string connector;
        string result = string.Empty;
        int max = target.Length;
        for (int i = 0; i < max; i++)
        {
            connector = (i == max - 1) ? string.Empty : symbol;
            result = result.Append(target[i], connector);
        }
        return result;
    }

    public static string Joins(this List<string> target, string symbol = ",")
    {
        string connector;
        string result = string.Empty;
        int max = target.Count;
        for (int i = 0; i < max; i++)
        {
            connector = (i == max - 1) ? string.Empty : symbol;
            result = result.Append(target[i], connector);
        }
        return result;
    }

    public static List<T> Contact<T>(this List<T> target, List<T> list)
    {
        int max = (list != default) ? list.Count : 0;
        for (int i = 0; i < max; i++)
        {
            target.Add(list[i]);
        }
        return target;
    }
    
    public static List<T> Contact<T>(this List<T> target, T[] list)
    {
        int max = (list != default) ? list.Length : 0;
        for (int i = 0; i < max; i++)
        {
            target.Add(list[i]);
        }
        return target;
    }

    public static T[] ContactToArr<T>(this List<T> target, List<T> list)
    {
        int max = (list != default) ? list.Count : 0;
        for (int i = 0; i < max; i++)
        {
            target.Add(list[i]);
        }
        return target.ToArray();
    }

    public static T[] ContactToArr<T>(this List<T> target, T[] list)
    {
        int max = (list != default) ? list.Length : 0;
        for (int i = 0; i < max; i++)
        {
            target.Add(list[i]);
        }
        return target.ToArray();
    }
}