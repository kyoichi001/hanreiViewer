using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UITimeline : MonoBehaviour
{
    public class UITimeData
    {
        public int ID;
        public int begin_time;
        public int end_time;
        public string text;
        public bool is_range;
        public int layer;
    }


    [SerializeField] RectTransform times_top;
    [SerializeField] RectTransform times_bottom;
    [SerializeField] RectTransform arrow;
    [SerializeField] GameObject timePrefab;
    List<UITimeData> topTimes = new List<UITimeData>();
    List<UITimeData> bottomTimes = new List<UITimeData>();

    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = transform as RectTransform;
    }
    int time_id = 0;
    public int AddTime(int begin_time, int end_time, string text, bool isTop = true)
    {
        int layer = 0;
        while (!isCapable(begin_time, end_time, layer, isTop))
        {
            layer++;
        }
        return AddTimeToLayer(begin_time, end_time, text, layer, isTop);
    }
    public int AddTime(int time, string text, bool isTop = true)
    {
        int layer = 0;
        while (!isCapable(time, layer, isTop))
        {
            layer++;
        }
        return AddTimeToLayer(time, text, layer, isTop);
    }

    bool isCapable(int beginTime, int endTime, int layer, bool is_top)
    {
        var arr = is_top ? topTimes : bottomTimes;
        foreach (var i in arr)
        {
            if (i.layer != layer) continue;
            if (i.is_range)
            {
                if (beginTime <= i.begin_time && i.begin_time <= endTime) return false;
                if (beginTime <= i.end_time && i.end_time <= endTime) return false;
                if (i.begin_time <= beginTime && beginTime <= i.end_time) return false;
                if (i.begin_time <= endTime && endTime <= i.end_time) return false;
            }
            else
            {
                if (beginTime <= i.begin_time && i.begin_time <= endTime) return false;
            }
        }
        return true;
    }
    bool isCapable(int time, int layer, bool isTop)
    {
        var arr = isTop ? topTimes : bottomTimes;
        foreach (var i in arr)
        {
            if (i.layer != layer) continue;
            if (i.is_range)
            {
                if (i.begin_time <= time && time <= i.end_time) return false;
            }
            else
            {
                if (time == i.begin_time) return false;
            }
        }
        return true;
    }
    int AddTimeToLayer(int beginTime, int endTime, string text, int layer, bool isTop)
    {
        var arr = isTop ? topTimes : bottomTimes;
        arr.Add(new UITimeData
        {
            ID= time_id,
            begin_time = beginTime,
            end_time = endTime,
            text = text,
            is_range = true,
            layer = layer
        });
        time_id++;
        return time_id - 1;
    }
    int AddTimeToLayer(int time, string text, int layer, bool isTop)
    {
        var arr = isTop ? topTimes : bottomTimes;
        arr.Add(new UITimeData
        {
            ID= time_id,
            begin_time = time,
            text = text,
            is_range = false,
            layer = layer
        });
        time_id++;
        return time_id - 1;
    }

    void GenerateTimeUI(int beginTime, int endTime, UITimeData data, bool isTop)
    {
        //https://nekosuko.jp/1792/
        var time_height = 25;
        var parent = isTop ? times_top : times_bottom;
        var time_layer_offset = isTop ? time_height : -time_height;
        var padding = isTop ? 30 : -30;
        if (data.is_range)
        {
            var beginRatio = (data.begin_time - beginTime) / (endTime - beginTime);
            var endRatio = (data.end_time - beginTime) / (endTime - beginTime);
            var a = Instantiate(timePrefab, parent).GetComponent<UITime>();
            a.gameObject.name = data.ID.ToString();
            a.Init(data.text);
            var rc = a.transform as RectTransform;
            rc.localPosition = new Vector3(
                rectTransform.rect.xMin + rectTransform.rect.width * (endRatio + beginRatio) / 2,
                 data.layer * time_layer_offset+ padding
                );
            var widthRatio = (data.end_time - data.begin_time) / (endTime - beginTime);
            rc.sizeDelta = new Vector2(widthRatio * rectTransform.rect.width, time_height);
        }
        else
        {
            var beginRatio = (data.begin_time - beginTime) / (endTime - beginTime);
            var a = Instantiate(timePrefab, parent).GetComponent<UITime>();
            a.gameObject.name = data.ID.ToString();
            a.Init(data.text);
            var rc = a.transform as RectTransform;
            rc.localPosition = new Vector3(
                rectTransform.rect.xMin + rectTransform.rect.width * beginRatio,
                 data.layer * time_layer_offset+ padding
                );
        }
    }

    public void GenerateUI()
    {
        var min_value = 90000000;
        var max_value = 0;
        foreach (var i in topTimes)
        {
            min_value = Mathf.Min(min_value, i.begin_time);
            max_value = Mathf.Max(max_value, i.begin_time);
            if (i.is_range)
            {
                min_value = Mathf.Min(min_value, i.end_time);
                max_value = Mathf.Max(max_value, i.end_time);
            }
        }
        foreach (var i in bottomTimes)
        {
            min_value = Mathf.Min(min_value, i.begin_time);
            max_value = Mathf.Max(max_value, i.begin_time);
            if (i.is_range)
            {
                min_value = Mathf.Min(min_value, i.end_time);
                max_value = Mathf.Max(max_value, i.end_time);
            }
        }
        var yearUnit = 200;
        rectTransform.sizeDelta = new Vector2((max_value - min_value) * yearUnit, 50);
        foreach (var i in topTimes)
        {
            GenerateTimeUI(min_value, max_value, i, true);
        }
        foreach (var i in bottomTimes)
        {
            GenerateTimeUI(min_value, max_value, i, false);
        }
    }

    /// <summary>
    /// a‚Æb‚Ì•ïŠÜŠÖŒW‚Æ‚È‚éRect‚ðŽæ“¾
    /// </summary>
    public static Rect GetContainsRect(Rect a, Rect b)
    {
        var rect = new Rect(a);
        rect.yMin = Mathf.Min(rect.yMin, b.yMin);
        rect.yMax = Mathf.Max(rect.yMax, b.yMax);
        rect.xMin = Mathf.Min(rect.xMin, b.xMin);
        rect.xMax = Mathf.Max(rect.xMax, b.xMax);
        return rect;
    }

    public Rect CalcRect()
    {
        var rect = GetContainsRect(times_top.rect, times_bottom.rect);
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
