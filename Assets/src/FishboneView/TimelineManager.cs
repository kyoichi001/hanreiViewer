using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : SingletonMonoBehaviour<TimelineManager>
{
    int time_id = 0;

    [Header("Debug")]
    [SerializeField] List<UITimeData> data = new List<UITimeData>();
    public List<UITimeData> GetData() => data;
    void Awake()
    {
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data_) =>
        {

        });
    }
    public int AddTime(System.DateTime? beginTime, System.DateTime? endTime, string text, bool isTop)
    {
        if (beginTime == null && endTime == null)
        {
            Debug.LogError("both time is null!");
            return -1;
        }
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
            is_top = isTop,
        });
        time_id++;
        return time_id - 1;
    }
    public int AddTime(System.DateTime time, string text, bool isTop)
    {
        data.Add(new UITimeData
        {
            ID = time_id,
            begin_time = time,
            text = text,
            timeType = TimeType.point,
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
