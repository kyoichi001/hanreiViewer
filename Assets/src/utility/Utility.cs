using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
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

    #region DurationTime関係
    public static System.DateTime Min(System.DateTime a, System.DateTime b) => a > b ? b : a;
    public static System.DateTime Max(System.DateTime a, System.DateTime b) => a < b ? b : a;
    public static System.DateTime Min(System.DateTime a, DurationTime b, int offsetYear = 10)
    {
        var (min, max) = b.GetMinMax(offsetYear);
        return Utility.Min(a, min);
    }
    public static System.DateTime Max(System.DateTime a, DurationTime b, int offsetYear = 10)
    {
        var (min, max) = b.GetMinMax(offsetYear);
        return Utility.Max(a, max);
    }
    public static System.DateTime Min(DurationTime b, System.DateTime a, int offsetYear = 10) => Utility.Min(a, b, offsetYear);
    public static System.DateTime Max(DurationTime b, System.DateTime a, int offsetYear = 10) => Utility.Max(a, b, offsetYear);

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
    public static bool Overlaps(System.DateTime beginValue, System.DateTime endValue, System.DateTime begin, System.DateTime end)
    {
        return begin <= endValue && beginValue <= end;
    }
    #endregion

    public static T DeepClone<T>(T src)
    {
        using (var memoryStream = new System.IO.MemoryStream())
        {
            var binaryFormatter
              = new System.Runtime.Serialization
                    .Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, src); // シリアライズ
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return (T)binaryFormatter.Deserialize(memoryStream); // デシリアライズ
        }
    }
}
