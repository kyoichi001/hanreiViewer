using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DataType=HanreiTokenizedData.HanreiTextTokenData.HanreiEventsData;

public class UIFishbone : MonoBehaviour
{

[Header("References")]
    [SerializeField] GameObject timeLine;
    [SerializeField] Transform timeStampsContainer;
    [SerializeField] GameObject timeStampPrefab;
    [SerializeField] Slider pinchSlider;

[Header("YearUnit")]
    [SerializeField] float yearUnitLength = 100;
    
    [Header("Debug")]
    [SerializeField] List<DataType> data = new List<DataType>();



    UITimeline uiTimeLine;

    Dictionary<(string, int,int), UITimeStamp> eventMap = new Dictionary<(string, int,int), UITimeStamp>();
    Dictionary<(string, int,int), int> timeMap = new Dictionary<(string, int,int), int>();


    public void AddData(DataType data_)
    {
        if(data_.person=="" || data_.acts=="")return;

        var time_text="";
        var begin_value=0;
        var end_value=0;
        var is_range=false;
        if(data_.time.point!=null&&data_.time.point.value!=0){
            time_text+=data_.time.point.text;
            begin_value=data_.time.point.value;
        }
        if(data_.time.begin!=null&&data_.time.begin.value!=0){
            time_text+=data_.time.begin.text;
            begin_value=data_.time.begin.value;
            is_range=true;
        }
        if(data_.time.end!=null&&data_.time.end.value!=0){
            time_text+=data_.time.end.text;
            end_value=data_.time.end.value;
            is_range=true;
        }
        
        if(begin_value==0&&end_value==0){
            Debug.Log($"???????????? {data_.time.begin.text}{data_.time.begin.value},{data_.time.end.text}{data_.time.end.value},{data_.time.point.text}{data_.time.point.value}",gameObject);
        return;
        }
        data.Add(data_);
        var map_key=(data_.person, begin_value,end_value);
        if (eventMap.ContainsKey(map_key))
        {
            eventMap[(data_.person, begin_value,end_value)].AddAct(data_.acts);
        }
        else
        {
            var timeStampObj = Instantiate(timeStampPrefab, timeStampsContainer).GetComponent<UITimeStamp>();
            var dat = new TimeStampData
            {
                person = data_.person,
                time = time_text,
                begin_value = begin_value,
                end_value = end_value,
                is_range = is_range,
                acts = new List<string> { data_.acts }
            };
            timeStampObj.SetData(dat);
            eventMap[map_key] = timeStampObj;
            var time_id = uiTimeLine.AddTime(begin_value,end_value, time_text);
            timeMap[map_key] = time_id;
        }
    }

    private void Awake()
    {
        uiTimeLine = timeLine.GetComponent<UITimeline>();
        pinchSlider.value = yearUnitLength;
        pinchSlider.onValueChanged.AddListener((value) =>
        {
            PinchTimeline(value);
        });
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data) =>
        {
            foreach(var d in data.datas){
                foreach(var e in d.events){
                    AddData(e);
                }
            }
            uiTimeLine.GenerateUI();
            SetEventsPosition();
        });
    }
    void SetEventsPosition()
    {
        foreach (var i in eventMap)
        {
            i.Value.SetPosition(uiTimeLine.GetTimeTransform(timeMap[i.Key]).position);
        }
    }
    public void PinchTimeline(float unitLength)
    {
        yearUnitLength = unitLength;
        SetEventsPosition();
    }
}
