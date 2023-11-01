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
    [SerializeField, ReadOnly] float cameraScale;
    [SerializeField, ReadOnly] int currentScaleLevel = 0;

    public System.DateTime begin_time { get; private set; }
    public System.DateTime end_time { get; private set; }

    RectTransform rectTransform;

    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
    }
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
        var (timeTickYear, timeTickMonth, _) = GetPixelTime();

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
    public float GetTimebarLength()
    {
        var (timeTickYear, timeTickMonth, _) = GetPixelTime();
        var (min_value, max_value) = (begin_time, end_time);
        var t2 = min_value.AddYears(timeTickYear).AddMonths(timeTickMonth);
        var timeRatio2 = (float)(t2 - min_value).TotalDays / (float)(max_value - min_value).TotalDays;
        return timeRatio2 * rectTransform.rect.width * transform.lossyScale.x;
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
        //Debug.Log($"time {minTime},{maxTime}, time {data.data.begin_time},{data.data.end_time}, ratio {beginRatio},{endRatio}");
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
                //Debug.Log($"{rectTransform.rect.width}:{widthRatio},{widthRatio * rectTransform.rect.width}", gameObject);
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
        var (timeTickYear, timeTickMonth, level) = GetPixelTime();
        if (level != currentScaleLevel)
        {
            currentScaleLevel = level;
            Debug.Log("generate time bar");
            ClearTimeBar();
            GenerateTimeBar(begin_time, end_time);
        }
    }
    Camera mainCamera;
    public (int, int, int) GetPixelTime()
    {
        cameraScale = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        var scale = cameraScale;
        if (scale >= 1500)
        {
            return (5, 0, 0);
        }
        else if (scale >= 800)
        {
            return (1, 0, 1);
        }
        else if (scale >= 700)
        {
            return (0, 6, 2);
        }
        else if (scale >= 600)
        {
            return (0, 3, 3);
        }
        else
        {
            return (0, 3, 3);
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
