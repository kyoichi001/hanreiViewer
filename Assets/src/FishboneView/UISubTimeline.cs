using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubTimeline : MonoBehaviour
{
    [System.Serializable]
    public class TimelineNodeData
    {
        public int ID;
        public DurationTime time;
        public string text;
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

    void Awake()
    {
        rectTransform = transform as RectTransform;
        LOD = GetComponent<Akak.LOD>();
    }
    public void Init(System.DateTime begin, System.DateTime end)
    {
        begin_time = begin;
        end_time = end;
    }
    public bool Contains(DurationTime t, int offsetYear = 10)
    {
        var a = new DurationTime(begin_time, end_time, TimeType.begin_end);
        return a.Contains(t, offsetYear);
    }
    public void AddTime(UITimeData data)
    {
        var arr = data.is_top ? topTimes : bottomTimes;
        int layer = 0;
        while (!isCapable(data.time, layer, data.is_top))
        {
            layer++;
        }
        arr.Add(new TimelineNodeData
        {
            ID = data.ID,
            time = data.time,
            text = data.text,
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
        //単位を年にするために1000で割る（365日が年の日数だがプログラムが動いてるのでヨシ！）
        rectTransform.sizeDelta = new Vector2((float)(maxTime - minTime).TotalDays / 1000 * yearUnitLength, 50);
        var time_height = 25;
        var time_layer_offset = isTop ? time_height : -time_height;
        var padding = isTop ? 30 : -30;
        var (b, e) = data.data.time.GetMinMax(10);
        var beginRatio = (float)(b - minTime).TotalDays / (float)(maxTime - minTime).TotalDays;
        var endRatio = (float)(e - minTime).TotalDays / (float)(maxTime - minTime).TotalDays;
        var rc = data.gameObject.transform as RectTransform;
        switch (data.data.time.timeType)
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
                rc.localPosition = new Vector3(
                    rectTransform.rect.xMin + rectTransform.rect.width * (endRatio + beginRatio) / 2,
                    data.data.layer * time_layer_offset + padding
                    );
                var widthRatio = endRatio - beginRatio;
                rc.sizeDelta = new Vector2(widthRatio * rectTransform.rect.width, time_height);
                break;
        }
    }
    public void GenerateUI()
    {
        ClearTimeBar();
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
    public void SetUnitLength(float yearUnitLength)
    {
        this.yearUnitLength = yearUnitLength;
        var (min_value, max_value) = (begin_time, end_time);
        foreach (Transform child in times_top)
        {
            SetPosition(min_value, max_value, child.GetComponent<UITime>(), true);
        }
        foreach (Transform child in times_bottom)
        {
            SetPosition(min_value, max_value, child.GetComponent<UITime>(), false);
        }
        ClearTimeBar();
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

    bool isCapable(DurationTime time, int layer, bool is_top, int offsetYear = 10, int paddingYear = 1)
    {
        var data = is_top ? topTimes : bottomTimes;
        foreach (var i in data)
        {
            if (i.is_top != is_top) continue;
            if (i.layer != layer) continue;
            if (i.time.Overlaps(time.Extend(paddingYear), offsetYear)) return false;
        }
        return true;
    }
    public float GetXRatioFromCenter(float globalX)
    {
        var localPoint = transform.InverseTransformPoint(new Vector3(globalX, 0, 0));
        var ratio = (localPoint.x - rectTransform.rect.xMin) / rectTransform.rect.width;
        return 2.0f * (ratio - 0.5f);//左端 -1 右端 1になるように
    }
    public Vector3 GetWorldPosFromCenterRatio(float ratio)
    {
        var pos = new Vector3(rectTransform.rect.width * ratio / 2f, 0);
        return transform.TransformPoint(pos);
    }
}
