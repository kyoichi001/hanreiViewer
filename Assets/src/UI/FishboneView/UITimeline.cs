using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UITimeline : MonoBehaviour
{
    [System.Serializable]
    public class UITimeData
    {
        public int ID;
        public System.DateTime begin_time;
        public System.DateTime end_time;
        public string text;
        public bool is_range;
        public int layer;
    }

    [Header("References")]
    [SerializeField] RectTransform times_top;
    [SerializeField] RectTransform times_bottom;
    [SerializeField] RectTransform arrow;
    [Header("Prefabs")]
    [SerializeField] GameObject timePrefab;
    [SerializeField] float yearUnitLength = 100;

    [Header("Debug")]
    [SerializeField]List<UITimeData> topTimes = new List<UITimeData>();
    [SerializeField]List<UITimeData> bottomTimes = new List<UITimeData>();
    [SerializeField] Slider pinchSlider;

    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = transform as RectTransform;
        pinchSlider.value = yearUnitLength;
        pinchSlider.onValueChanged.AddListener((value) =>
        {
            PinchTimeline(value);
        });
    }
    int time_id = 0;
    public int AddTime(System.DateTime begin_time, System.DateTime end_time, string text, bool isTop = true)
    {
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

    bool isCapable(System.DateTime beginTime, System.DateTime endTime, int layer, bool is_top)
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
    bool isCapable(System.DateTime time, int layer, bool isTop)
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
    int AddTimeToLayer(System.DateTime beginTime, System.DateTime endTime, string text, int layer, bool isTop)
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
    int AddTimeToLayer(System.DateTime time, string text, int layer, bool isTop)
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

    void GenerateTimeUI(UITimeData data, bool isTop)
    {
        //https://nekosuko.jp/1792/
        var parent = isTop ? times_top : times_bottom;
        var a = Instantiate(timePrefab, parent).GetComponent<UITime>();
        a.gameObject.name = data.ID.ToString();
        a.Init(data.begin_time,data.end_time,data.is_range,data.layer,data.text);
    }
    void SetPosition(System.DateTime minTime, System.DateTime maxTime, UITime data, bool isTop){
        var yearUnit = 200;
        //単位を年にするために1000で割る
        rectTransform.sizeDelta = new Vector2((float)(maxTime - minTime).TotalDays/1000 * yearUnit, 50);
        var time_height = 25;
        var time_layer_offset = isTop ? time_height : -time_height;
        var padding = isTop ? 30 : -30;
        var beginRatio = (float)(data.begin - minTime).TotalDays / (float)(maxTime - minTime).TotalDays;
        var endRatio = (float)(data.end - minTime).TotalDays / (float)(maxTime - minTime).TotalDays;
        if (data.is_range)
        {
            beginRatio=Mathf.Clamp(beginRatio,0f,1f);
            endRatio=Mathf.Clamp(endRatio,0f,1f);
            Debug.Log($"time {minTime},{maxTime}, time {data.begin},{data.end}, ratio {beginRatio},{endRatio}");
            var rc = data.gameObject.transform as RectTransform;
            rc.localPosition = new Vector3(
                rectTransform.rect.xMin + rectTransform.rect.width * (endRatio + beginRatio) / 2,
                 data.layer * time_layer_offset+ padding
                );
            var widthRatio =endRatio-beginRatio;
            Debug.Log($"{rectTransform.rect.width}:{widthRatio},{widthRatio*rectTransform.rect.width}",gameObject);
            rc.sizeDelta = new Vector2(widthRatio * rectTransform.rect.width, time_height);
        }
        else
        {
            var rc = data.gameObject.transform as RectTransform;
            rc.localPosition = new Vector3(
                rectTransform.rect.xMin + rectTransform.rect.width * beginRatio,
                 data.layer * time_layer_offset+ padding
                );
        }
    }

(System.DateTime,System.DateTime) CalcMinMax(){
        var min_value = System.DateTime.MaxValue;
        var max_value = System.DateTime.MinValue;
        
        foreach (var i in topTimes)
        {
            //TODO: �Е�������Ԃ��Ȃ�������point�̂Ƃ�MinMax���v�Z���Ȃ��悤��
            if(i.begin_time!=null){
                min_value = min_value> i.begin_time? i.begin_time:min_value;
                max_value = max_value< i.begin_time? i.begin_time:max_value;
            }
            if (i.end_time!=null)
            {
                min_value = min_value> i.end_time? i.end_time:min_value;
                max_value = max_value< i.end_time? i.end_time:max_value;
            }
        }
        foreach (var i in bottomTimes)
        {
            if(i.begin_time!=null){
                min_value = min_value> i.begin_time? i.begin_time:min_value;
                max_value = max_value< i.begin_time? i.begin_time:max_value;
            }
            if (i.end_time!=null)
            {
                min_value = min_value> i.end_time? i.end_time:min_value;
                max_value = max_value< i.end_time? i.end_time:max_value;
            }
        }
    return (min_value,max_value);
}

    public void GenerateUI()
    {
        var (min_value,max_value) = CalcMinMax();
        foreach (var i in topTimes)
        {
            GenerateTimeUI( i, true);
        }
        foreach (var i in bottomTimes)
        {
            GenerateTimeUI( i, false);
        }
        foreach(Transform child in times_top){
        SetPosition(min_value,max_value,child.GetComponent<UITime>(),true);
        }
        foreach(Transform child in times_bottom){
        SetPosition(min_value,max_value,child.GetComponent<UITime>(),false);
        }
    }

    /// <summary>
    /// a??b?????W????Rect???��
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

    public void PinchTimeline(float unitLength)
    {
        var (min_value,max_value) = CalcMinMax();
        yearUnitLength = unitLength;
        foreach(Transform child in times_top){
        SetPosition(min_value,max_value,child.GetComponent<UITime>(),true);
        }
        foreach(Transform child in times_bottom){
        SetPosition(min_value,max_value,child.GetComponent<UITime>(),false);
        }
    }

}
