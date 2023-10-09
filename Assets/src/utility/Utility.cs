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
}
