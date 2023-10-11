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
    [SerializeField] Slider pinchSlider;
    [SerializeField] GameObject subTimelinePrefab;

    List<UISubTimeline> subTimelines = new List<UISubTimeline>();

    List<UITimeData> data;

    int time_id = 0;
    private void Awake()
    {
        pinchSlider.value = yearUnitLength;
        pinchSlider.onValueChanged.AddListener((value) =>
        {
            PinchTimeline(value);
        });
    }

    public UISubTimeline GetNearest(System.DateTime splitTime)
    {
        var minTime = System.DateTime.MaxValue;
        UISubTimeline minTL = null;
        var maxTime = System.DateTime.MinValue;
        UISubTimeline maxTL = null;
        foreach (var tl in subTimelines)
        {
            if (tl.begin_time <= splitTime && splitTime < tl.end_time) return tl;
            if (tl.begin_time < minTime)
            {
                minTime = tl.begin_time;
                minTL = tl;
            }
            if (maxTime < tl.end_time)
            {
                maxTime = tl.end_time;
                maxTL = tl;
            }
        }
        if (splitTime < minTime) return minTL;
        if (maxTime <= splitTime) return maxTL;
        return null;
    }
    public void SplitTimeline(System.DateTime splitTime)
    {
        var offsetYear = 10;
        //すでにある区間を横切って切ろうとするとエラー
        foreach (var i in data)
        {
            switch (i.timeType)
            {
                case TimeType.point:
                    continue;
                case TimeType.begin:
                    if ((i.begin_time ?? splitTime) <= splitTime && splitTime < i.begin_time?.AddYears(offsetYear)) return;
                    break;
                case TimeType.end:
                    if (i.end_time?.AddYears(-offsetYear) <= splitTime && splitTime < (i.end_time ?? splitTime)) return;
                    break;
                case TimeType.begin_end:
                    if ((i.begin_time ?? splitTime) <= splitTime && splitTime < (i.end_time ?? splitTime)) return;
                    break;
            }
        }
        foreach (var tl in subTimelines)
        {
            if (splitTime == tl.begin_time || splitTime == tl.end_time) return;//境界だったら切らない
            if (tl.begin_time < splitTime && splitTime < tl.end_time)
            {
                var new_left = Instantiate(subTimelinePrefab).GetComponent<UISubTimeline>();
                var new_right = Instantiate(subTimelinePrefab).GetComponent<UISubTimeline>();
                new_left.Init(tl.begin_time, splitTime);
                new_right.Init(splitTime, tl.end_time);
                //TODO: 当該Timelineの削除＆新しいものに置き換え
                subTimelines.Add(new_left);
                subTimelines.Add(new_right);

                return;
            }
        }
    }

    public int AddData(System.DateTime? begin_time, System.DateTime? end_time, string text, bool isTop = true)
    {
        var timeType = TimeType.begin_end;
        if (begin_time == null) timeType = TimeType.end;
        if (end_time == null) timeType = TimeType.begin;
        int layer = 0;
        while (!isCapable(begin_time, end_time, layer, isTop))
        {
            layer++;
        }
        data.Add(new UITimeData
        {
            ID = time_id,
            begin_time = begin_time,
            end_time = end_time,
            text = text,
            timeType = timeType,
            is_top = isTop,
            layer = layer,
        });
        time_id++;
        return time_id - 1;
    }
    public int AddData(System.DateTime time, string text, bool isTop = true)
    {
        int layer = 0;
        while (!isCapable(time, layer, isTop))
        {
            layer++;
        }
        data.Add(new UITimeData
        {
            ID = time_id,
            begin_time = time,
            end_time = null,
            text = text,
            timeType = TimeType.point,
            is_top = isTop,
        });
        time_id++;
        return time_id - 1;
    }
    (System.DateTime, System.DateTime) CalcMinMax()
    {
        var min_value = System.DateTime.MaxValue;
        var max_value = System.DateTime.MinValue;
        foreach (var i in data)
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
    public RectTransform GetTimeTransform(int ID)
    {
        foreach (var tl in subTimelines)
        {
            var res = tl.GetTimeTransform(ID);
            if (res != null) return res;
        }
        return null;
    }
    bool isCapable(System.DateTime? beginTime, System.DateTime? endTime, int layer, bool is_top)
    {
        var b = beginTime ?? endTime?.AddYears(-10) ?? System.DateTime.MinValue;
        var e = endTime ?? beginTime?.AddYears(10) ?? System.DateTime.MaxValue;
        foreach (var i in data)
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
        foreach (var i in data)
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
    public void ClearUI()
    {
        foreach (var tl in subTimelines)
        {
            tl.ClearUI();
        }
        data.Clear();
        time_id = 0;
    }

    //TODO: subTimelineを並び替え、座標を計算
    void SetPosition(System.DateTime minTime, System.DateTime maxTime, UITime data, bool isTop)
    {

    }


    public void GenerateUI()
    {
        var (min_value, max_value) = CalcMinMax();
        foreach (var i in data)
        {

        }
    }


    public void PinchTimeline(float unitLength)
    {
        var (min_value, max_value) = CalcMinMax();
        yearUnitLength = unitLength;
    }

}
