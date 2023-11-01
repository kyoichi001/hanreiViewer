using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static Rect GetContainsRect(Rect a, Rect b)
    {
        var rect = new Rect(a);
        rect.yMin = Mathf.Min(rect.yMin, b.yMin);
        rect.yMax = Mathf.Max(rect.yMax, b.yMax);
        rect.xMin = Mathf.Min(rect.xMin, b.xMin);
        rect.xMax = Mathf.Max(rect.xMax, b.xMax);
        return rect;
    }
    public static Rect GetContainsRect(params Rect[] args)
    {
        var rect = new Rect(args[0]);
        foreach (var r in args)
        {
            rect.yMin = Mathf.Min(rect.yMin, r.yMin);
            rect.yMax = Mathf.Max(rect.yMax, r.yMax);
            rect.xMin = Mathf.Min(rect.xMin, r.xMin);
            rect.xMax = Mathf.Max(rect.xMax, r.xMax);
        }
        return rect;
    }
    public static System.DateTime Min(System.DateTime a, System.DateTime b)
    {
        return a > b ? b : a;
    }
    public static System.DateTime Max(System.DateTime a, System.DateTime b)
    {
        return a < b ? b : a;
    }
    public static System.DateTime Convert(int date)
    {
        var year = date / 10000;
        var month = date / 100 % 100;
        var day = date % 100;
        if (month < 1) month = 1;
        if (day < 1) day = 1;
        //Debug.Log($"{date}:{year}/{month}/{day}");
        return new System.DateTime(year, month, day);
    }
}
