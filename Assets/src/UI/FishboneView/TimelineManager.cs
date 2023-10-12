using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : SingletonMonoBehaviour<TimelineManager>
{
    int time_id = 0;
    public List<UITimeData> data { get; private set; } = new List<UITimeData>();
    [SerializeField] List<UITimeData> d = new List<UITimeData>();
    void Awake()
    {
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data_) =>
        {

        });
    }

    public int AddTime(System.DateTime? begin_time, System.DateTime? end_time, string text, bool isTop = true)
    {
        if (begin_time == null && end_time == null)
        {
            Debug.LogError("both time is null!");
            return -1;
        }
        var b = begin_time == null ? "null" : begin_time.ToString();
        var e = end_time == null ? "null" : begin_time.ToString();
        Debug.Log($"Timeline manager : AddTime {b} {e}");
        int layer = 0;
        while (!isCapable(begin_time, end_time, layer, isTop))
        {
            layer++;
        }
        return AddTimeToLayer(begin_time, end_time, text, layer, isTop);
    }
    public int AddTime(System.DateTime time, string text, bool isTop = true)
    {
        int layer = 0;
        while (!isCapable(time, layer, isTop))
        {
            layer++;
        }
        return AddTimeToLayer(time, text, layer, isTop);
    }

    bool isCapable(System.DateTime? beginTime, System.DateTime? endTime, int layer, bool is_top)
    {
        var b = beginTime ?? endTime?.AddYears(-10) ?? System.DateTime.MinValue;
        var e = endTime ?? beginTime?.AddYears(10) ?? System.DateTime.MaxValue;
        foreach (var i in data)
        {
            if (i.is_top != is_top) continue;
            if (i.layer != layer) continue;
            switch (i.timeType)
            {
                case TimeType.point:
                    if (b <= i.begin_time && i.begin_time <= endTime) return false;
                    break;
                case TimeType.begin:
                case TimeType.begin_end:
                case TimeType.end:
                    if (b <= i.begin_time && i.begin_time <= e) return false;
                    if (b <= i.end_time && i.end_time <= e) return false;
                    if (i.begin_time <= b && b <= i.end_time) return false;
                    if (i.begin_time <= e && e <= i.end_time) return false;
                    break;
            }
        }
        return true;
    }
    bool isCapable(System.DateTime time, int layer, bool isTop)
    {
        foreach (var i in data)
        {
            if (i.is_top != isTop) continue;
            if (i.layer != layer) continue;
            switch (i.timeType)
            {
                case TimeType.point:
                    if (time == i.begin_time) return false;
                    break;
                case TimeType.begin:
                case TimeType.begin_end:
                case TimeType.end:
                    if (i.begin_time <= time && time <= i.end_time) return false;
                    break;
            }
        }
        return true;
    }
    int AddTimeToLayer(System.DateTime? beginTime, System.DateTime? endTime, string text, int layer, bool isTop)
    {
        var timeType = TimeType.begin_end;
        if (beginTime == null) timeType = TimeType.end;
        if (endTime == null) timeType = TimeType.begin;

        data.Add(new UITimeData
        {
            ID = time_id,
            begin_time = beginTime,
            end_time = endTime,
            text = text,
            timeType = timeType,
            layer = layer,
            is_top = isTop,
        });
        d.Add(new UITimeData
        {
            ID = time_id,
            begin_time = beginTime,
            end_time = endTime,
            text = text,
            timeType = timeType,
            layer = layer,
            is_top = isTop,
        });
        time_id++;
        return time_id - 1;
    }
    int AddTimeToLayer(System.DateTime time, string text, int layer, bool isTop)
    {
        data.Add(new UITimeData
        {
            ID = time_id,
            begin_time = time,
            text = text,
            timeType = TimeType.point,
            layer = layer,
            is_top = isTop,
        });
        d.Add(new UITimeData
        {
            ID = time_id,
            begin_time = time,
            text = text,
            timeType = TimeType.point,
            layer = layer,
            is_top = isTop,
        });
        time_id++;
        return time_id - 1;
    }
    public void Clear()
    {
        data.Clear();
        time_id = 0;
    }

    public (System.DateTime, System.DateTime) CalcMinMax(int offsetYear = 0)
    {
        var min_value = System.DateTime.MaxValue;
        var max_value = System.DateTime.MinValue;
        foreach (var i in data)
        {
            var b = i.begin_time ?? System.DateTime.MinValue;
            var e = i.end_time ?? System.DateTime.MaxValue;
            switch (i.timeType)
            {
                case TimeType.point:
                    min_value = Utility.Min(min_value, b);
                    max_value = Utility.Max(max_value, b);
                    break;
                case TimeType.begin_end:
                    min_value = Utility.Min(min_value, b);
                    max_value = Utility.Max(max_value, e);
                    break;
                case TimeType.begin:
                    min_value = Utility.Min(min_value, b);
                    max_value = Utility.Max(max_value, b.AddYears(offsetYear));
                    break;
                case TimeType.end:
                    min_value = Utility.Min(min_value, e.AddYears(-offsetYear));
                    max_value = Utility.Max(max_value, e);
                    break;
            }
        }
        return (min_value, max_value);
    }


}
