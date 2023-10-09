using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Linq;

public class UITimeline : MonoBehaviour
{

    [SerializeField] float yearUnitLength = 100;
    [Header("References")]
    [SerializeField] RectTransform times_top;
    [SerializeField] RectTransform times_bottom;
    [SerializeField] RectTransform arrow;
    [SerializeField] Slider pinchSlider;
    [Header("Prefabs")]
    [SerializeField] GameObject timePrefab;

    [Header("Debug")]
    [SerializeField] List<UITimeData> topTimes = new List<UITimeData>();
    [SerializeField] List<UITimeData> bottomTimes = new List<UITimeData>();

    RectTransform rectTransform;
    int time_id = 0;
    private void Awake()
    {
        rectTransform = transform as RectTransform;
        pinchSlider.value = yearUnitLength;
        pinchSlider.onValueChanged.AddListener((value) =>
        {
            PinchTimeline(value);
        });
    }
    public int AddTime(System.DateTime? begin_time, System.DateTime? end_time, string text, bool isTop = true)
    {
        if (begin_time == null && end_time == null)
        {
            Debug.LogError("both time is null!");
            return -1;
        }
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
        var arr = is_top ? topTimes : bottomTimes;
        foreach (var i in arr)
        {
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
        var arr = isTop ? topTimes : bottomTimes;
        foreach (var i in arr)
        {
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
        var arr = isTop ? topTimes : bottomTimes;
        var timeType = TimeType.begin_end;
        if (beginTime == null) timeType = TimeType.end;
        if (endTime == null) timeType = TimeType.begin;

        arr.Add(new UITimeData
        {
            ID = time_id,
            begin_time = beginTime,
            end_time = endTime,
            text = text,
            timeType = timeType,
            layer = layer
        });
        time_id++;
        return time_id - 1;
    }
    int AddTimeToLayer(System.DateTime time, string text, int layer, bool isTop)
    {
        var arr = isTop ? topTimes : bottomTimes;
        arr.Add(new UITimeData
        {
            ID = time_id,
            begin_time = time,
            text = text,
            timeType = TimeType.point,
            layer = layer
        });
        time_id++;
        return time_id - 1;
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
        time_id = 0;
    }
    void GenerateTimeUI(UITimeData data, bool isTop)
    {
        //https://nekosuko.jp/1792/
        var parent = isTop ? times_top : times_bottom;
        var a = Instantiate(timePrefab, parent).GetComponent<UITime>();
        a.gameObject.name = data.ID.ToString();
        a.Init(data);
    }
    void SetPosition(System.DateTime minTime, System.DateTime maxTime, UITime data, bool isTop)
    {
        var yearUnit = 200;
        //単位を年にするために1000で割る
        rectTransform.sizeDelta = new Vector2((float)(maxTime - minTime).TotalDays / 1000 * yearUnit, 50);
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

    (System.DateTime, System.DateTime) CalcMinMax()
    {
        var min_value = System.DateTime.MaxValue;
        var max_value = System.DateTime.MinValue;
        foreach (var i in topTimes)
        {
            var b = i.begin_time ?? System.DateTime.MinValue;
            var e = i.end_time ?? System.DateTime.MaxValue;
            if (i.begin_time != null)
            {
                min_value = Utility.Min(min_value, b);
                max_value = Utility.Max(max_value, b);
            }
            if (i.end_time != null)
            {
                min_value = Utility.Min(min_value, e);
                max_value = Utility.Max(max_value, e);
            }
        }
        foreach (var i in bottomTimes)
        {
            var b = i.begin_time ?? System.DateTime.MinValue;
            var e = i.end_time ?? System.DateTime.MaxValue;
            if (i.begin_time != null)
            {
                min_value = Utility.Min(min_value, b);
                max_value = Utility.Max(max_value, b);
            }
            if (i.end_time != null)
            {
                min_value = Utility.Min(min_value, e);
                max_value = Utility.Max(max_value, e);
            }
        }
        return (min_value, max_value);
    }

    public void GenerateUI()
    {
        var (min_value, max_value) = CalcMinMax();
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

    public void PinchTimeline(float unitLength)
    {
        var (min_value, max_value) = CalcMinMax();
        yearUnitLength = unitLength;
        foreach (Transform child in times_top)
        {
            SetPosition(min_value, max_value, child.GetComponent<UITime>(), true);
        }
        foreach (Transform child in times_bottom)
        {
            SetPosition(min_value, max_value, child.GetComponent<UITime>(), false);
        }
    }

}
