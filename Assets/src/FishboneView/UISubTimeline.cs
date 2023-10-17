using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubTimeline : MonoBehaviour
{
    [SerializeField] float yearUnitLength = 100;
    [Header("References")]
    [SerializeField] RectTransform times_top;
    [SerializeField] RectTransform times_bottom;
    [SerializeField] RectTransform arrow;
    [Header("Prefabs")]
    [SerializeField] GameObject timePrefab;
    [SerializeField] GameObject timeBarPrefab;
    [Header("Debug")]
    [SerializeField, ReadOnly] List<UITimeData> topTimes = new List<UITimeData>();
    [SerializeField, ReadOnly] List<UITimeData> bottomTimes = new List<UITimeData>();


    public System.DateTime begin_time { get; private set; }
    public System.DateTime end_time { get; private set; }

    RectTransform rectTransform;

    int currentScaleLevel = 0;
    public void Init(System.DateTime begin, System.DateTime end)
    {
        rectTransform = transform as RectTransform;
        begin_time = begin;
        end_time = end;
    }
    public bool Contains(System.DateTime time)
    {
        return begin_time <= time && time < end_time;
    }
    public bool Contains(System.DateTime? begin, System.DateTime? end, TimeType timeType, int offsetYear = 10)
    {
        switch (timeType)
        {
            case TimeType.point:
                return Contains(begin ?? System.DateTime.MinValue);
            case TimeType.begin_end:
                return
                begin_time <= (begin ?? System.DateTime.MinValue) &&
                (end ?? System.DateTime.MaxValue) <= end_time;
            case TimeType.begin:
                return
                begin_time <= (begin ?? System.DateTime.MinValue) &&
                (begin?.AddYears(offsetYear) ?? System.DateTime.MaxValue) <= end_time;
            case TimeType.end:
                return
                begin_time <= (end?.AddYears(-offsetYear) ?? System.DateTime.MinValue) &&
                (end ?? System.DateTime.MaxValue) <= end_time;
        }
        return false;
    }
    public void AddTime(UITimeData data)
    {
        var arr = data.is_top ? topTimes : bottomTimes;
        arr.Add(data);
    }
    public void ClearUI()
    {
        topTimes.Clear();
        bottomTimes.Clear();
        foreach (Transform child in times_top)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in times_bottom)
        {
            Destroy(child.gameObject);
        }
    }
    void GenerateTimeUI(UITimeData data, bool isTop)
    {
        //https://nekosuko.jp/1792/
        var parent = isTop ? times_top : times_bottom;
        var a = Instantiate(timePrefab, parent).GetComponent<UITime>();
        a.gameObject.name = data.ID.ToString();
        a.Init(data);
    }
    void ClearTimeBar()
    {
        foreach (Transform child in arrow)
        {
            Destroy(child.gameObject);
        }
    }
    void GenerateTimeBar(System.DateTime minTime, System.DateTime maxTime)
    {
        var (timeTickYear, timeTickMonth) = GetPixelTime();

        var t = minTime.AddYears(timeTickYear).AddMonths(timeTickMonth);
        int max_count = 0;
        while (t < maxTime && max_count < 500)
        {
            var timeRatio = (float)(t - minTime).TotalDays / (float)(maxTime - minTime).TotalDays;
            var a = Instantiate(timeBarPrefab, arrow).GetComponent<UITimebar>();
            a.Init(t);
            a.transform.localPosition = new Vector3(
                        rectTransform.rect.xMin + rectTransform.rect.width * timeRatio,
                        0
                        );
            t = t.AddYears(timeTickYear).AddMonths(timeTickMonth);
            max_count++;
        }
    }

    void SetPosition(System.DateTime minTime, System.DateTime maxTime, UITime data, bool isTop)
    {
        //単位を年にするために1000で割る
        rectTransform.sizeDelta = new Vector2((float)(maxTime - minTime).TotalDays / 1000 * yearUnitLength, 50);
        var time_height = 25;
        var time_layer_offset = isTop ? time_height : -time_height;
        var padding = isTop ? 30 : -30;
        System.DateTime b = data.data.begin_time ?? data.data.end_time?.AddYears(-10) ?? System.DateTime.MinValue;
        System.DateTime e = data.data.end_time ?? data.data.begin_time?.AddYears(10) ?? System.DateTime.MaxValue;
        var beginRatio = (float)(b - minTime).TotalDays / (float)(maxTime - minTime).TotalDays;
        var endRatio = (float)(e - minTime).TotalDays / (float)(maxTime - minTime).TotalDays;
        var rc = data.gameObject.transform as RectTransform;
        Debug.Log($"time {minTime},{maxTime}, time {data.data.begin_time},{data.data.end_time}, ratio {beginRatio},{endRatio}");
        switch (data.data.timeType)
        {
            case TimeType.point:
                rc.localPosition = new Vector3(
                    rectTransform.rect.xMin + rectTransform.rect.width * beginRatio,
                    data.data.layer * time_layer_offset + padding
                    );
                break;
            case TimeType.begin_end:
            case TimeType.begin:
            case TimeType.end:
                beginRatio = Mathf.Clamp(beginRatio, 0f, 1f);
                endRatio = Mathf.Clamp(endRatio, 0f, 1f);
                rc.localPosition = new Vector3(
                    rectTransform.rect.xMin + rectTransform.rect.width * (endRatio + beginRatio) / 2,
                    data.data.layer * time_layer_offset + padding
                    );
                var widthRatio = endRatio - beginRatio;
                Debug.Log($"{rectTransform.rect.width}:{widthRatio},{widthRatio * rectTransform.rect.width}", gameObject);
                rc.sizeDelta = new Vector2(widthRatio * rectTransform.rect.width, time_height);
                break;
        }
    }
    public void GenerateUI(float yearUnitLength)
    {
        ClearTimeBar();
        this.yearUnitLength = yearUnitLength;
        var (min_value, max_value) = (begin_time, end_time);
        foreach (var i in topTimes)
        {
            GenerateTimeUI(i, true);
        }
        foreach (var i in bottomTimes)
        {
            GenerateTimeUI(i, false);
        }
        foreach (Transform child in times_top)
        {
            SetPosition(min_value, max_value, child.GetComponent<UITime>(), true);
        }
        foreach (Transform child in times_bottom)
        {
            SetPosition(min_value, max_value, child.GetComponent<UITime>(), false);
        }
        GenerateTimeBar(min_value, max_value);
    }
    void Update()
    {
        var lv = currentScaleLevel;
        var (timeTickYear, timeTickMonth) = GetPixelTime();
        if (lv != currentScaleLevel)
        {
            ClearTimeBar();
            GenerateTimeBar(begin_time, end_time);
        }
    }

    (int, int) GetPixelTime()
    {
        var scale = transform.lossyScale.x;
        if (scale <= 0.2f + 0.1f)
        {
            currentScaleLevel = 0;
            return (5, 0);
        }
        else if (scale <= 0.5f + .25f)
        {
            currentScaleLevel = 1;
            return (1, 0);
        }
        else if (scale <= 1f + .25f)
        {
            currentScaleLevel = 2;
            return (0, 6);
        }
        else if (scale <= 2.0f + .25f)
        {
            currentScaleLevel = 3;
            return (0, 3);
        }
        else
        {
            currentScaleLevel = 4;
            return (0, 1);
        }
    }


    public Rect CalcRect()
    {
        var rect = Utility.GetContainsRect(times_top.rect, times_bottom.rect);
        return rect;
    }

    public RectTransform GetTimeTransform(int ID)
    {
        foreach (Transform i in times_top)
            if (i.gameObject.name == ID.ToString()) return i as RectTransform;
        foreach (Transform i in times_bottom)
            if (i.gameObject.name == ID.ToString()) return i as RectTransform;
        return null;
    }
}
