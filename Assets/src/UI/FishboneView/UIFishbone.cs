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

    [SerializeField] float yearUnitLength = 100;//TimeLineè„ÇÃ1îNÇÃãóó£

    [SerializeField] Slider pinchSlider;

    Dictionary<(string, int), UITimeStamp> eventMap = new Dictionary<(string, int), UITimeStamp>();

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
                var dat = new TimeStampData();
                dat.person = hanreiEvent.person;
                dat.time = hanreiEvent.time;
                dat.time_value = hanreiEvent.value;
                dat.acts = new List<string> { hanreiEvent.acts };
                timeStampObj.SetData(dat);
                eventMap[(hanreiEvent.person, hanreiEvent.value)] = timeStampObj;
            }
        }
        SetEventsPosition();
    }

    private void Awake()
    {
        FishboneViewManager.Instance.OnDataLoaded.AddListener((data) =>
        {
            SetData(data.events);
        });
        pinchSlider.value = yearUnitLength;
        pinchSlider.onValueChanged.AddListener((value) =>
        {
            PinchTimeline(value);
        });
    }
    void SetEventsPosition()
    {
        var center = timeLine.transform.position;
        int yearMax = 0, yearMin = 3000;
        var rc = timeLine.transform as RectTransform;
        foreach (var i in eventMap)
        {
            yearMax = Mathf.Max(yearMax, i.Key.Item2);
            yearMin = Mathf.Min(yearMin, i.Key.Item2);
        }
        float centerTime = (yearMax + yearMin) / 2;
        foreach (var i in eventMap)
        {
            i.Value.SetPosition(center + new Vector3(yearUnitLength * (i.Key.Item2 - centerTime), rc.rect.height / 2 +10));
        }
    }
    public void PinchTimeline(float unitLength)
    {
        yearUnitLength = unitLength;
        SetEventsPosition();
    }
}
