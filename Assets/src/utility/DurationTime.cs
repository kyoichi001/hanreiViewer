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
        this.end = null;
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
    public DurationTime Extend(int paddingYear)
    {
        switch (timeType)
        {
            case TimeType.point:
                return new DurationTime(begin?.AddYears(-paddingYear), begin?.AddYears(paddingYear), TimeType.begin_end);
            case TimeType.begin_end:
            case TimeType.end:
            case TimeType.begin:
                return new DurationTime(begin?.AddYears(-paddingYear), end?.AddYears(paddingYear), timeType);
        }
        return null;
    }
    /// <summary>
    /// 自身の区間が相手を完全に覆うかどうか
    /// </summary>
    public bool Contains(DurationTime t, int offsetYear = 10, bool infinity = false)
    {
        var (b, e) = t.GetMinMax(offsetYear);
        var (b2, e2) = GetMinMax(offsetYear);
        if (infinity)//無限の長さとして扱う
        {
            switch (timeType)
            {
                case TimeType.point:
                    return t.timeType == TimeType.point && begin == t.begin;
                case TimeType.begin_end:
                    switch (t.timeType)
                    {
                        case TimeType.point:
                            return Utility.Contains(b, b2, e2);
                        case TimeType.begin_end:
                            return Utility.Contains(b, e, b2, e2);
                        case TimeType.begin:
                        case TimeType.end:
                            return false;
                    }
                    return false;
                case TimeType.begin:
                    switch (t.timeType)
                    {
                        case TimeType.point:
                        case TimeType.begin_end:
                        case TimeType.begin:
                            return b2 <= b;
                        case TimeType.end:
                            return false;
                    }
                    return false;
                case TimeType.end:
                    switch (t.timeType)
                    {
                        case TimeType.point:
                            return e2 <= b;
                        case TimeType.begin_end:
                        case TimeType.end:
                            return e2 <= e;
                        case TimeType.begin:
                            return false;
                    }
                    return false;
            }
            return false;
        }
        switch (timeType)
        {
            case TimeType.point://点は区間を含まない（逆は成り立つ）
                return t.timeType == TimeType.point && begin == t.begin;
            case TimeType.begin_end:
            case TimeType.begin:
            case TimeType.end:
                return Utility.Contains(b, e, b2, e2);
        }
        return false;
    }
    /// <summary>
    /// 区間が重なるかどうか
    /// </summary>
    public bool Overlaps(DurationTime t, int offsetYear = 10, bool infinity = false)
    {
        var (b, e) = t.GetMinMax(offsetYear);
        var (b2, e2) = GetMinMax(offsetYear);
        if (infinity)//無限の長さとして扱う
        {
            switch (timeType)
            {
                case TimeType.point:
                    return t.timeType == TimeType.point && begin == t.begin;
                case TimeType.begin_end:
                    switch (t.timeType)
                    {
                        case TimeType.point:
                            return Utility.Contains(b, b2, e2);
                        case TimeType.begin_end:
                            return Utility.Overlaps(b, e, b2, e2);
                        case TimeType.begin:
                            return b <= e2;
                        case TimeType.end:
                            return b2 <= e;
                    }
                    return false;
                case TimeType.begin:
                    switch (t.timeType)
                    {
                        case TimeType.point:
                            return b2 <= b;
                        case TimeType.begin_end:
                            return b2 <= e;
                        case TimeType.begin:
                            return true;
                        case TimeType.end:
                            return b2 <= e;
                    }
                    return false;
                case TimeType.end:
                    switch (t.timeType)
                    {
                        case TimeType.point:
                            return b <= e2;
                        case TimeType.begin_end:
                            return b <= e2;
                        case TimeType.end:
                            return true;
                        case TimeType.begin:
                            return b <= e2;
                    }
                    return false;
            }
            return false;
        }
        switch (timeType)
        {
            case TimeType.point:
                if (t.timeType == TimeType.point) return begin == t.begin;
                return t.Contains(this, offsetYear);
            case TimeType.begin_end:
            case TimeType.begin:
            case TimeType.end:
                return Utility.Overlaps(b, e, b2, e2);
        }
        return false;
    }
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
