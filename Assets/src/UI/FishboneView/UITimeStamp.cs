using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStampData
{
    public string person;
    public string time;
    public List<string> acts;
}


public class UITimeStamp : MonoBehaviour
{

    [SerializeField] GameObject personNode;
    TMPro.TextMeshProUGUI personText;
    [SerializeField] GameObject timeNode;
    TMPro.TextMeshProUGUI timeText;
    [SerializeField] Transform eventsContainer;
    [SerializeField] GameObject eventPrefab;
    [SerializeField] GameObject boneLine;
    private void Awake()
    {
        personText=personNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        timeText=timeNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    public void SetEventsWidth()
    {

    }
    public void SetHeight(float height)
    {

    }
    public void SetData(TimeStampData data)
    {
        personText.text=data.person;
        timeText.text=data.time;
        foreach (var act in data.acts)
        {
            var eventSrc = Instantiate(eventPrefab, eventsContainer).GetComponent<UIEvent>();
            eventSrc.SetData(act);
        }
    }
    public void AddAct(string act)
    {
        var eventSrc = Instantiate(eventPrefab, eventsContainer).GetComponent<UIEvent>();
        eventSrc.SetData(act);
    }
    public float GetWidth()
    {
        return 0f;
    }
}
