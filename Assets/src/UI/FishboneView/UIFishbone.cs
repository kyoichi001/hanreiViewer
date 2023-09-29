using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFishbone : MonoBehaviour
{

    [SerializeField] GameObject timeLine;
    [SerializeField] Transform timeStampsContainer;
    [SerializeField] GameObject timeStampPrefab;
    [SerializeField] List<HanreiEvent> data = new List<HanreiEvent>();

    Dictionary<(string, string), UITimeStamp> eventMap = new Dictionary<(string, string), UITimeStamp>();

    public void SetData(List<HanreiEvent> data)
    {
        this.data = data;
        foreach (var hanreiEvent in this.data)
        {
            if (eventMap.ContainsKey((hanreiEvent.person, hanreiEvent.time)))
            {
                eventMap[(hanreiEvent.person, hanreiEvent.time)].AddAct(hanreiEvent.acts);
            }
            else
            {
                var timeStampObj = Instantiate(timeStampPrefab, timeStampsContainer).GetComponent<UITimeStamp>();
                var dat = new TimeStampData();
                dat.person = hanreiEvent.person;
                dat.time = hanreiEvent.time;
                dat.acts = new List<string> {hanreiEvent.acts };
                eventMap[(hanreiEvent.person, hanreiEvent.time)] = timeStampObj;
                timeStampObj.SetData(dat);
            }
        }
    }

    private void Awake()
    {
        FishboneViewManager.Instance.OnDataLoaded.AddListener((data) =>
        {
            SetData(data.events);
        });
    }
}
