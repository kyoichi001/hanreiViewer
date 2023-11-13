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
            time = new DurationTime(beginTime, endTime, timeType),
            text = text,
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
            time = new DurationTime(time),
            text = text,
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
            min_value = Utility.Min(min_value, i.time, offsetYear);
            max_value = Utility.Max(max_value, i.time, offsetYear);
        }
        return (min_value, max_value);
    }


}
