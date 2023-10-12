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
    TimelineManager manager;

    private void Awake()
    {
        manager=GetComponent<TimelineManager>();
        pinchSlider.value = yearUnitLength;
        pinchSlider.onValueChanged.AddListener((value) =>
        {
            PinchTimeline(value);
        });
    }

    bool Splitable(System.DateTime splitTime){
        var offsetYear = 10;
        //すでにある区間を横切って切ろうとするとエラー
        foreach (var i in manager.data)
        {
            switch (i.timeType)
            {
                case TimeType.point:
                    continue;
                case TimeType.begin:
                    if ((i.begin_time ?? splitTime) <= splitTime && splitTime < i.begin_time?.AddYears(offsetYear)) return false;
                    break;
                case TimeType.end:
                    if (i.end_time?.AddYears(-offsetYear) <= splitTime && splitTime < (i.end_time ?? splitTime)) return false;
                    break;
                case TimeType.begin_end:
                    if ((i.begin_time ?? splitTime) <= splitTime && splitTime < (i.end_time ?? splitTime)) return false;
                    break;
            }
        }
return true;
    }

    public void SplitTimeline(System.DateTime splitTime)
    {
        //すでにある区間を横切って切ろうとするとエラー
        if(!Splitable(splitTime))return;
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
    public RectTransform GetTimeTransform(int ID)
    {
        foreach (var tl in subTimelines)
        {
            var res = tl.GetTimeTransform(ID);
            if (res != null) return res;
        }
        return null;
    }

    public void ClearUI()
    {

    }

    //TODO: subTimelineを並び替え、座標を計算
    void SetPosition(System.DateTime minTime, System.DateTime maxTime, UITime data, bool isTop)
    {

    }


    public void GenerateUI()
    {
        var subTL=Instantiate(subTimelinePrefab,transform).GetComponent<UISubTimeline>();
        subTL.Init();
        subTimelines.Add(subTL);
        //TODO: timenodeをsubTL内に生成したうえでdata.time_nodeに登録する
        foreach (var i in manager.data)
        {

        }
    }
    public void PinchTimeline(float unitLength)
    {
        var (min_value, max_value) = manager.CalcMinMax();
        yearUnitLength = unitLength;
    }

}
