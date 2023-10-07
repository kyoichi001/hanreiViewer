using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFishbone : MonoBehaviour
{

    [SerializeField] GameObject timeLine;
    [SerializeField] Transform timeStampsContainer;
    [SerializeField] GameObject timeStampPrefab;
    [SerializeField] List<HanreiEvent> data = new List<HanreiEvent>();

    UITimeline uiTimeLine;

    [SerializeField] float yearUnitLength = 100;//TimeLine���1�N�̋���

    [SerializeField] Slider pinchSlider;

    Dictionary<(string, int), UITimeStamp> eventMap = new Dictionary<(string, int), UITimeStamp>();
    Dictionary<(string, int), int> timeMap = new Dictionary<(string, int), int>();

    public void SetData(List<HanreiEvent> data)
    {
        this.data = data;
        foreach (var hanreiEvent in this.data)
        {
            if (eventMap.ContainsKey((hanreiEvent.person, hanreiEvent.value)))
            {
                eventMap[(hanreiEvent.person, hanreiEvent.value)].AddAct(hanreiEvent.acts);
            }
            else
            {
                var timeStampObj = Instantiate(timeStampPrefab, timeStampsContainer).GetComponent<UITimeStamp>();
                var dat = new TimeStampData
                {
                    person = hanreiEvent.person,
                    time = hanreiEvent.time,
                    time_value = hanreiEvent.value,
                    acts = new List<string> { hanreiEvent.acts }
                };
                timeStampObj.SetData(dat);
                eventMap[(hanreiEvent.person, hanreiEvent.value)] = timeStampObj;
                var time_id = uiTimeLine.AddTime(hanreiEvent.value, hanreiEvent.time);
                timeMap[(hanreiEvent.person, hanreiEvent.value)] = time_id;
            }
        }
        uiTimeLine.GenerateUI();
        SetEventsPosition();
    }

    private void Awake()
    {
        uiTimeLine = timeLine.GetComponent<UITimeline>();
        FishboneViewManager.Instance.OnDataLoaded.AddListener((data) =>
        {
            SetData(data.events);
        });
        pinchSlider.value = yearUnitLength;
        pinchSlider.onValueChanged.AddListener((value) =>
        {
            PinchTimeline(value);
        });
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data) =>
        {

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
