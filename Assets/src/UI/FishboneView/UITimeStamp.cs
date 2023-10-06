using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeStampData
{
    public string person;
    public string time;
    public int time_value;
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

    [SerializeField] float boneAnchor;//timeからどれだけ離れているか
    [SerializeField] float eventsGap;
    [SerializeField] float eventsWidth;

    public int timeValue { get; private set; }

    List<UIEvent> events = new List<UIEvent>();

    private void Awake()
    {
        personText = personNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        timeText = timeNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    public void SetEventsWidth(float width)
    {
        foreach (var i in events)
        {
            i.SetBoneWidth(width);
        }
    }
    public void SetHeight(float height)
    {
        personNode.transform.localPosition = timeNode.transform.localPosition + new Vector3(0,height);
    }

    public void SetAngle(float angle)
    {
        var vec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        var height = personNode.transform.localPosition.y - timeNode.transform.localPosition.y;
        personNode.transform.localPosition = timeNode.transform.localPosition+ vec * height / Mathf.Sin(angle);
    }

    /// <summary>
    /// timeNodeのグローバル座標を指定するとともに、それを基準に他のObjectの位置も相対的に移動させる
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        var offset = position-timeNode.transform.position;
        transform.position += offset;
    }

    public Rect CalcRect()
    {
        var timeRect = timeNode.transform as RectTransform;
        var personRect = personNode.transform as RectTransform;
        var res = new Rect(timeRect.rect);
        foreach (var act in events)
        {
            var rc = act.CalcRect();
            res.yMin = Mathf.Min(res.yMin, rc.yMin);
            res.yMax = Mathf.Max(res.yMax, rc.yMax);
            res.xMin = Mathf.Min(res.xMin, rc.xMin);
            res.xMax = Mathf.Max(res.xMax, rc.xMax);
        }
        res.yMin = Mathf.Min(res.yMin, personRect.rect.yMin);
        res.yMax = Mathf.Max(res.yMax, personRect.rect.yMax);
        res.xMin = Mathf.Min(res.xMin, personRect.rect.xMin);
        res.xMax = Mathf.Max(res.xMax, personRect.rect.xMax);
        return res;
    }
    public void SetData(TimeStampData data)
    {
        personText.text = data.person;
        timeValue = data.time_value;
        timeText.text = data.time;
        foreach (var act in data.acts)
        {
            var eventSrc = Instantiate(eventPrefab, eventsContainer).GetComponent<UIEvent>();
            eventSrc.SetData(act);
            events.Add(eventSrc);
        }
        SetActsPos(boneAnchor, eventsGap);
    }
    public void AddAct(string act)
    {
        var eventSrc = Instantiate(eventPrefab, eventsContainer).GetComponent<UIEvent>();
        eventSrc.SetData(act);
        events.Add(eventSrc);
        SetActsPos(boneAnchor, eventsGap);
    }
    private void Update()
    {
        SetEventsWidth(eventsWidth);
        SetActsPos(boneAnchor, eventsGap);
    }


    public void SetActsPos(float anchor, float gap)
    {
        var offset = personNode.transform.localPosition - timeNode.transform.localPosition;
        float angle = Mathf.Atan2(offset.y, offset.x);
        var vec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        var rc = timeNode.transform as RectTransform;
        var pos = timeNode.transform.localPosition + vec * (anchor+ rc.rect.height/2) / Mathf.Sin(angle);
        foreach (var i in events)
        {
            Debug.Log($"height : {timeNode.transform.localPosition}, {rc.rect.height}, {i.CalcRect().height}, {angle} , {vec}");
            pos += (i.CalcRect().height / 2) / Mathf.Sin(angle) * vec;
            i.transform.localPosition = pos;
            pos += (i.CalcRect().height / 2 + gap) / Mathf.Sin(angle) * vec;
        }
    }
}
