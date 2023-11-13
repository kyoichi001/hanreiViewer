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
    public static System.DateTime Min(System.DateTime a, DurationTime b, int offsetYear = 10)
    {
        switch (b.timeType)
        {
            case TimeType.point:
                return Utility.Min(a, b.begin ?? System.DateTime.MaxValue);
            case TimeType.begin_end:
                return Utility.Min(a, b.begin ?? System.DateTime.MaxValue);
            case TimeType.begin:
                return Utility.Min(a, b.begin ?? System.DateTime.MaxValue);
            case TimeType.end:
                return Utility.Min(a, (b.end ?? System.DateTime.MaxValue).AddYears(-offsetYear));
        }
        return a;
    }
    public static System.DateTime Max(System.DateTime a, DurationTime b, int offsetYear = 10)
    {
        switch (b.timeType)
        {
            case TimeType.point:
                return Utility.Max(a, b.begin ?? System.DateTime.MinValue);
            case TimeType.begin_end:
                return Utility.Max(a, b.end ?? System.DateTime.MinValue);
            case TimeType.begin:
                return Utility.Max(a, (b.begin ?? System.DateTime.MinValue).AddYears(offsetYear));
            case TimeType.end:
                return Utility.Max(a, b.end ?? System.DateTime.MinValue);
        }
        return a;
    }
    public static System.DateTime Min(DurationTime b, System.DateTime a, int offsetYear = 10)
    {
        return Utility.Min(a, b, offsetYear);
    }
    public static System.DateTime Max(DurationTime b, System.DateTime a, int offsetYear = 10)
    {
        return Utility.Max(a, b, offsetYear);
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
    public static bool Contains(System.DateTime t, System.DateTime begin, System.DateTime end)
    {
        return begin <= t && t < end;
    }
    public static bool Contains(System.DateTime beginValue, System.DateTime endValue, System.DateTime begin, System.DateTime end)
    {
        return begin <= beginValue && endValue <= end;
    }
    public static bool Contains(System.DateTime beginValue, System.DateTime endValue, DurationTime t, int offsetYear)
    {
        switch (t.timeType)
        {
            case TimeType.point:
                return Utility.Contains(t.begin ?? System.DateTime.MinValue, beginValue, endValue);
            case TimeType.begin_end:
                return Utility.Contains(t.begin ?? System.DateTime.MinValue, t.end ?? System.DateTime.MaxValue, beginValue, endValue);
            case TimeType.begin:
                return Utility.Contains(t.begin ?? System.DateTime.MinValue, t.begin?.AddYears(offsetYear) ?? System.DateTime.MaxValue, beginValue, endValue);
            case TimeType.end:
                return Utility.Contains(t.end?.AddYears(-offsetYear) ?? System.DateTime.MinValue, t.end ?? System.DateTime.MaxValue, beginValue, endValue);
        }
        return false;
    }
}
