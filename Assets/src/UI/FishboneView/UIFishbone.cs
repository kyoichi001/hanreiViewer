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

    Dictionary<(string, System.DateTime,System.DateTime), UITimeStamp> eventMap = new Dictionary<(string, System.DateTime,System.DateTime), UITimeStamp>();
    Dictionary<(string, System.DateTime,System.DateTime), int> timeMap = new Dictionary<(string, System.DateTime,System.DateTime), int>();


    public void AddData(DataType data_)
    {
        if(data_.person=="" || data_.acts=="")return;
        if(data_.time.begin.value==0&&data_.time.end.value==0&&data_.time.point.value==0)return;

        var time_text="";
        var  begin_value=new System.DateTime();
        var end_value=new System.DateTime();
        var is_range=false;
        if(data_.time.point!=null&&data_.time.point.value!=0){
            time_text+=data_.time.point.text;
            var year=data_.time.point.value/10000;
            var month=data_.time.point.value/100%100;
            var day=data_.time.point.value%100;
            if(month<1)month=1;
            if(day<1)day=1;
            Debug.Log($"{data_.time.point.value}:{year}{month}{day}");
            begin_value=new System.DateTime(year,month,day);
        }
        if(data_.time.begin!=null&&data_.time.begin.value!=0){
            time_text+=data_.time.begin.text;
            var year=data_.time.begin.value/10000;
            var month=data_.time.begin.value/100%100;
            var day=data_.time.begin.value%100;
            if(month<1)month=1;
            if(day<1)day=1;
            Debug.Log($"{data_.time.begin.value}:{year}{month}{day}");
            begin_value=new System.DateTime(year,month,day);
            is_range=true;
        }
        if(data_.time.end!=null&&data_.time.end.value!=0){
            time_text+=data_.time.end.text;
            var year=data_.time.end.value/10000;
            var month=data_.time.end.value/100%100;
            var day=data_.time.end.value%100;
            if(month<1)month=1;
            if(day<1)day=1;
            Debug.Log($"{data_.time.end.value}:{year}{month}{day}");
            end_value=new System.DateTime(year,month,day);
            is_range=true;
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
            var time_id = 0;
            if(is_range){
               time_id= uiTimeLine.AddTime(begin_value,end_value, time_text);
            }else{
               time_id= uiTimeLine.AddTime(begin_value, time_text);
            }
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
