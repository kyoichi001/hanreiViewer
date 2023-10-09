using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class TimeStampData
{
    public string person;
    public RectTransform time_node;
    public List<string> acts;
}

public class UITimeStamp : MonoBehaviour
{
    [SerializeField] float boneAnchor;//timeからどれだけ離れているか
    [SerializeField] float eventsGap;
    [SerializeField] float eventsWidth;

    [Header("prefabs")]
    [SerializeField] GameObject eventPrefab;

    [Header("References")]
    [SerializeField] GameObject personNode;
    TMPro.TextMeshProUGUI personText;
    [SerializeField] Transform eventsContainer;
    [SerializeField] GameObject boneLine;

    [Header("Debug")]
    [SerializeField] TimeStampData data;

    List<UIEvent> events = new List<UIEvent>();

    private void Awake()
    {
        personText = personNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
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
        personNode.transform.localPosition = data.time_node.localPosition + new Vector3(0, height);
    }

    public void SetAngle(float angle)
    {
        var vec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        var height = personNode.transform.localPosition.y - data.time_node.localPosition.y;
        personNode.transform.localPosition = data.time_node.localPosition + vec * height / Mathf.Sin(angle);
    }

    /// <summary>
    /// timeNodeのグローバル座標を指定するとともに、それを基準に他のObjectの位置も相対的に移動させる
    /// </summary>
    public void SetPosition()
    {
        Debug.Log($"set position {data.time_node.position}");
        transform.position = data.time_node.position;
    }

    public Rect CalcRect()
    {
        var timeRect = data.time_node;
        var personRect = personNode.transform as RectTransform;
        var res = new Rect(timeRect.rect);
        foreach (var act in events)
        {
            var rc = act.CalcRect();
            res = Utility.GetContainsRect(res, rc);
        }
        res = Utility.GetContainsRect(res, personRect.rect);
        return res;
    }
    public void SetData(TimeStampData data)
    {
        this.data = data;
        personText.text = data.person;
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
        var offset = personNode.transform.localPosition;
        float angle = Mathf.Atan2(offset.y, offset.x);
        var vec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        var rc = data.time_node;
        var pos = vec * (anchor + rc.rect.height / 2) / Mathf.Sin(angle);
        foreach (var i in events)
        {
            //Debug.Log($"height : {timeNode.transform.localPosition}, {rc.rect.height}, {i.CalcRect().height}, {angle} , {vec}");
            pos += (i.CalcRect().height / 2) / Mathf.Sin(angle) * vec;
            i.transform.localPosition = pos;
            pos += (i.CalcRect().height / 2 + gap) / Mathf.Sin(angle) * vec;
        }
    }
}
