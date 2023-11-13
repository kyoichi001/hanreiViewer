using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeType
{
    point,
    begin_end,
    begin,
    end
}

[System.Serializable]
public class DurationTime
{
    public System.DateTime? begin;
    public System.DateTime? end;
    public TimeType timeType;
    public DurationTime() { }
    public DurationTime(System.DateTime value)
    {
        this.begin = value;
        this.timeType = TimeType.point;
    }
    public DurationTime(System.DateTime? begin, System.DateTime? end, TimeType timeType)
    {
        this.begin = begin;
        this.end = end;
        this.timeType = timeType;
    }

    public override string ToString()
    {
        var b = begin?.ToString() ?? "null";
        var e = end?.ToString() ?? "null";
        return b + ":" + e;
    }

    public bool Contains(System.DateTime t, int offsetYear = 10)
    {
        switch (timeType)
        {
            case TimeType.point:
                return (begin ?? System.DateTime.MinValue) == t;
            case TimeType.begin_end:
                return Utility.Contains(begin ?? System.DateTime.MinValue, end ?? System.DateTime.MaxValue, t);
            case TimeType.begin:
                return Utility.Contains(begin ?? System.DateTime.MinValue, begin?.AddYears(offsetYear) ?? System.DateTime.MaxValue, t);
            case TimeType.end:
                return Utility.Contains(end?.AddYears(-offsetYear) ?? System.DateTime.MinValue, end ?? System.DateTime.MaxValue, t);
        }
        return begin <= t && t < end;
    }
    public bool Contains(System.DateTime beginValue, System.DateTime endValue, int offsetYear = 10)
    {
        switch (timeType)
        {
            case TimeType.point:
                return Utility.Contains(begin ?? System.DateTime.MinValue, beginValue, endValue);
            case TimeType.begin_end:
                return Utility.Contains(begin ?? System.DateTime.MinValue, end ?? System.DateTime.MaxValue, beginValue, endValue);
            case TimeType.begin:
                return Utility.Contains(begin ?? System.DateTime.MinValue, begin?.AddYears(offsetYear) ?? System.DateTime.MaxValue, beginValue, endValue);
            case TimeType.end:
                return Utility.Contains(end?.AddYears(-offsetYear) ?? System.DateTime.MinValue, end ?? System.DateTime.MaxValue, beginValue, endValue);
        }
        return begin <= beginValue && endValue <= end;
    }
    /* public bool Contains(DurationTime t, int offsetYear = 10)
     {
         var (b, e) = t.GetMinMax(offsetYear);
         switch (timeType)
         {
             case TimeType.point://点は区間を含まない（逆は成り立つ）
                 return false;
             case TimeType.begin_end:
                 return Contains(b, e, offsetYear);
             case TimeType.begin:
                 var (b2, e2) = GetMinMax(offsetYear);
                 return Utility.Contains(begin ?? System.DateTime.MinValue, begin?.AddYears(offsetYear) ?? System.DateTime.MaxValue, beginValue, endValue);
             case TimeType.end:
                 return Utility.Contains(end?.AddYears(-offsetYear) ?? System.DateTime.MinValue, end ?? System.DateTime.MaxValue, beginValue, endValue);
         }
         return false;
     }*/
    public (System.DateTime, System.DateTime) GetMinMax(int offsetYear = 10)
    {
        switch (timeType)
        {
            case TimeType.point:
                return (begin ?? System.DateTime.MinValue, begin ?? System.DateTime.MinValue);
            case TimeType.begin_end:
                return (begin ?? System.DateTime.MinValue, end ?? System.DateTime.MinValue);
            case TimeType.begin:
                return (begin ?? System.DateTime.MinValue, begin?.AddYears(offsetYear) ?? System.DateTime.MinValue);
            case TimeType.end:
                return (end?.AddYears(-offsetYear) ?? System.DateTime.MinValue, end ?? System.DateTime.MinValue);
        }
        return (System.DateTime.MinValue, System.DateTime.MaxValue);
    }
}
