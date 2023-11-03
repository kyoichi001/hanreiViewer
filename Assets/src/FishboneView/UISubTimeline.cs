using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubTimeline : MonoBehaviour
{
    [System.Serializable]
    public class TimelineNodeData
    {
        public int ID;
        public System.DateTime? begin_time;
        public System.DateTime? end_time;
        public string text;
        public TimeType timeType;
        public bool is_top;
        public int layer;
    }

    [SerializeField] float yearUnitLength = 100;
    [Header("References")]
    [SerializeField] RectTransform times_top;
    [SerializeField] RectTransform times_bottom;
    [SerializeField] RectTransform arrow;
    [Header("Prefabs")]
    [SerializeField] GameObject timePrefab;
    [SerializeField] GameObject timeBarPrefab;
    [Header("Debug")]
    [SerializeField, ReadOnly] List<TimelineNodeData> topTimes = new List<TimelineNodeData>();
    [SerializeField, ReadOnly] List<TimelineNodeData> bottomTimes = new List<TimelineNodeData>();
    [SerializeField, ReadOnly] float cameraScale;
    [SerializeField, ReadOnly] int currentScaleLevel = 0;

    public System.DateTime begin_time { get; private set; }
    public System.DateTime end_time { get; private set; }

    RectTransform rectTransform;
    Akak.LOD LOD;
    Camera mainCamera;

    void Awake()
    {
        rectTransform = transform as RectTransform;
        mainCamera = FindObjectOfType<Camera>();
        LOD = GetComponent<Akak.LOD>();
    }
    public void Init(System.DateTime begin, System.DateTime end)
    {
        begin_time = begin;
        end_time = end;
    }
    public bool Contains(System.DateTime? begin, System.DateTime? end, TimeType timeType, int offsetYear = 10)
    {
        switch (timeType)
        {
            case TimeType.point:
                return Utility.Contains(begin ?? System.DateTime.MinValue, begin_time, end_time);
            case TimeType.begin_end:
                return Utility.Contains(begin ?? System.DateTime.MinValue, end ?? System.DateTime.MaxValue, begin_time, end_time);
            case TimeType.begin:
                return Utility.Contains(begin ?? System.DateTime.MinValue, begin?.AddYears(offsetYear) ?? System.DateTime.MaxValue, begin_time, end_time);
            case TimeType.end:
                return Utility.Contains(end?.AddYears(-offsetYear) ?? System.DateTime.MinValue, end ?? System.DateTime.MaxValue, begin_time, end_time);
        }
        return false;
    }
    public void AddTime(UITimeData data)
    {
        var arr = data.is_top ? topTimes : bottomTimes;
        int layer = 0;
        if (data.timeType == TimeType.point)
        {
            while (!isCapable(data.begin_time ?? System.DateTime.MinValue, layer, data.is_top))
            {
                layer++;
            }
        }
        else
        {
            while (!isCapable(data.begin_time, data.end_time, layer, data.is_top))
            {
                layer++;
            }
        }
        arr.Add(new TimelineNodeData
        {
            ID = data.ID,
            begin_time = data.begin_time,
            end_time = data.end_time,
            text = data.text,
            timeType = data.timeType,
            is_top = data.is_top,
            layer = layer
        });
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
    void GenerateTimeUI(TimelineNodeData data, bool isTop)
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
    public (int, int, int) GetPixelTime()
    {
        var level = LOD.GetLOD();
        switch (level)
        {
            case 0: return (0, 3, 3);
            case 1: return (0, 6, 2);
            case 2: return (1, 0, 1);
            case 3: return (5, 0, 0);
        }
        return (0, 3, 3);
    }

    public Rect CalcRect()
    {
        return Utility.GetContainsRect(times_top.rect, times_bottom.rect);
    }

    public RectTransform GetTimeTransform(int ID)
    {
        foreach (Transform i in times_top)
            if (i.gameObject.name == ID.ToString()) return i as RectTransform;
        foreach (Transform i in times_bottom)
            if (i.gameObject.name == ID.ToString()) return i as RectTransform;
        return null;
    }

    bool isCapable(System.DateTime? beginTime, System.DateTime? endTime, int layer, bool is_top, int offsetYear = 10)
    {
        var b = beginTime ?? endTime?.AddYears(-offsetYear) ?? System.DateTime.MinValue;
        var e = endTime ?? beginTime?.AddYears(offsetYear) ?? System.DateTime.MaxValue;
        var data = is_top ? topTimes : bottomTimes;
        foreach (var i in data)
        {
            var b2 = i.begin_time ?? i.end_time?.AddYears(-offsetYear) ?? System.DateTime.MinValue;
            var e2 = i.end_time ?? i.begin_time?.AddYears(offsetYear) ?? System.DateTime.MaxValue;
            if (i.is_top != is_top) continue;
            if (i.layer != layer) continue;
            switch (i.timeType)
            {
                case TimeType.point:
                    if (b <= b2 && b2 <= e) return false;
                    break;
                case TimeType.begin:
                case TimeType.begin_end:
                case TimeType.end:
                    if (b <= b2 && b2 <= e) return false;
                    if (b <= e2 && e2 <= e) return false;
                    if (b2 <= b && b <= e2) return false;
                    if (b2 <= e && e <= e2) return false;
                    break;
            }
        }
        return true;
    }
    bool isCapable(System.DateTime time, int layer, bool isTop, int offsetYear = 10, int paddingYear = 1)
    {
        var b = time.AddYears(-paddingYear);
        var e = time.AddYears(paddingYear);
        var data = isTop ? topTimes : bottomTimes;
        foreach (var i in data)
        {
            var b2 = i.begin_time ?? i.end_time?.AddYears(-offsetYear) ?? System.DateTime.MinValue;
            var e2 = i.end_time ?? i.begin_time?.AddYears(offsetYear) ?? System.DateTime.MaxValue;
            if (i.is_top != isTop) continue;
            if (i.layer != layer) continue;
            switch (i.timeType)
            {
                case TimeType.point:
                    if (b <= b2 && b2 <= e) return false;
                    break;
                case TimeType.begin:
                case TimeType.begin_end:
                case TimeType.end:
                    if (b <= b2 && b2 <= e) return false;
                    if (b <= e2 && e2 <= e) return false;
                    if (b2 <= b && b <= e2) return false;
                    if (b2 <= e && e <= e2) return false;
                    break;
            }
        }
        return true;
    }
}
