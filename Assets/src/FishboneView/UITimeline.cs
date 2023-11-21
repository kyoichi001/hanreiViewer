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
    [SerializeField] GameObject subTimelinePrefab;

    List<UISubTimeline> subTimelines = new List<UISubTimeline>();
    List<System.DateTime> splitTimes = new List<System.DateTime>();

    bool Splitable(System.DateTime splitTime, int offsetYear = 10)
    {
        var (min, max) = TimelineManager.Instance.CalcMinMax();
        if (splitTime <= min || max <= splitTime) return false;
        //すでにある区間を横切って切ろうとするとエラー
        foreach (var i in TimelineManager.Instance.GetData())
        {
            switch (i.time.timeType)
            {
                case TimeType.point:
                    continue;
                case TimeType.begin:
                    if ((i.time.begin ?? splitTime) <= splitTime && splitTime < i.time.begin?.AddYears(offsetYear)) return false;
                    break;
                case TimeType.end:
                    if (i.time.end?.AddYears(-offsetYear) <= splitTime && splitTime < (i.time.end ?? splitTime)) return false;
                    break;
                case TimeType.begin_end:
                    if ((i.time.begin ?? splitTime) <= splitTime && splitTime < (i.time.end ?? splitTime)) return false;
                    break;
            }
        }
        return true;
    }

    public void SplitTimeline(System.DateTime splitTime)
    {
        //すでにある区間を横切って切ろうとするとエラー
        var offsetYear = 10;
        if (!Splitable(splitTime, offsetYear)) return;
        foreach (var t in splitTimes)
        {
            if (splitTime == t) return;//境界だったら切らない
        }
        splitTimes.Add(splitTime);
    }
    public RectTransform GetTimeTransform(int ID)
    {
        foreach (var tl in subTimelines)
        {
            var res = tl.GetTimeTransform(ID);
            if (res != null) return res;
        }
        Debug.LogWarning($"UITimeline : object not found ID:{ID}");
        return null;
    }

    public void ClearUI()
    {
        subTimelines.Clear();
        splitTimes.Clear();
        foreach (Transform child in transform)
        {
            var s = child.gameObject.name;
            Debug.Log($"Destroy {s}");
            Destroy(child.gameObject);
        }
    }

    //TODO: subTimelineを並び替え、座標を計算
    void SetPosition(System.DateTime minTime, System.DateTime maxTime, UITime data, bool isTop)
    {

    }

    public void GenerateUI()
    {
        var offsetYear = 10;
        var (min_value, max_value) = TimelineManager.Instance.CalcMinMax(offsetYear);
        if (splitTimes.Count == 0)
        {
            var a = Instantiate(subTimelinePrefab, transform).GetComponent<UISubTimeline>();
            a.Init(min_value, max_value);
            subTimelines.Add(a);
        }
        else
        {
            var a = Instantiate(subTimelinePrefab, transform).GetComponent<UISubTimeline>();
            a.Init(min_value, splitTimes[0]);
            subTimelines.Add(a);
            for (int i = 1; i < splitTimes.Count - 1; i++)
            {
                var subTL = Instantiate(subTimelinePrefab, transform).GetComponent<UISubTimeline>();
                subTL.Init(splitTimes[i], splitTimes[i - 1]);
                subTimelines.Add(subTL);
            }
            if (splitTimes.Count > 1)
            {
                var b = Instantiate(subTimelinePrefab, transform).GetComponent<UISubTimeline>();
                b.Init(splitTimes[^1], max_value);
                subTimelines.Add(b);
            }
        }
        foreach (var i in TimelineManager.Instance.GetData())
        {
            foreach (var subTL in subTimelines)
            {
                if (subTL.Contains(i.time, offsetYear))
                {
                    subTL.AddTime(i);
                }
                else
                {
                    Debug.LogWarning($"not contain {i.ID} {i.time} in {subTL.begin_time} {subTL.end_time}");
                }
            }
        }
        foreach (var subTL in subTimelines)
        {
            subTL.GenerateUI();
            subTL.SetUnitLength(yearUnitLength);
        }
    }
    public void PinchTimeline(float unitLength)
    {
        var (min_value, max_value) = TimelineManager.Instance.CalcMinMax();
        yearUnitLength = unitLength;
        foreach (var subTL in subTimelines)
        {
            subTL.SetUnitLength(yearUnitLength);
        }

    }

    public float GetTimebarLength()
    {
        foreach (var subTL in subTimelines)
        {
            return subTL.GetTimebarLength();
        }
        return 0;
    }
    public (int, int) GetPixelTime()
    {
        foreach (var subTL in subTimelines)
        {
            var (a, b, _) = subTL.GetPixelTime();
            return (a, b);
        }
        return (0, 0);

    }
}
