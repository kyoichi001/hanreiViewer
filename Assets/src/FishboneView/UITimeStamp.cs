using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable]
public class TimeStampData
{
    public string person;
    public RectTransform time_node;
    public List<string> acts;
    public bool is_top;
    public string claim_state;
    public int issue_num;
}

public class UITimeStamp : MonoBehaviour
{
    float height = 300;
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
    CanvasGroup canvasGroup;
    DrawArrow ui_arrow;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        personText = personNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        ui_arrow = GetComponentInChildren<DrawArrow>();
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
        this.height = height;
        personNode.transform.localPosition = new Vector3(0, height);
    }

    public void SetAngle(float angle)
    {
        var vec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        personNode.transform.localPosition = vec * height / Mathf.Sin(angle);
    }

    /// <summary>
    /// timeNodeのグローバル座標を指定するとともに、それを基準に他のObjectの位置も相対的に移動させる
    /// </summary>
    public void SetPosition()
    {
        Debug.Log($"set position {data.time_node.position}");
        transform.position = data.time_node.position;
        if (data.is_top)
        {
            SetHeight(300);
            SetAngle(80 * Mathf.Deg2Rad);
            SetActsPos(eventsGap);
        }
        else
        {
            SetHeight(-300);
            SetAngle(-80 * Mathf.Deg2Rad);
            SetActsPos(eventsGap);
        }
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
        ui_arrow.ui2 = data.time_node;
    }
    public void AddAct(string act)
    {
        var eventSrc = Instantiate(eventPrefab, eventsContainer).GetComponent<UIEvent>();
        eventSrc.SetData(act);
        events.Add(eventSrc);
        SetActsPos(eventsGap);
    }
    private void Update()
    {
        SetEventsWidth(eventsWidth);
        //SetActsPos(boneAnchor, eventsGap);
    }


    void SetActsPos(float gap)
    {
        var offset = personNode.transform.localPosition;
        float angle = Mathf.Atan2(offset.y, offset.x);
        Debug.Log($"timestamp : {data.is_top}, {angle}");
        var vec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        var pos = Vector3.zero;
        if (data.is_top)
        {
            foreach (var i in events)
            {
                //Debug.Log($"height : {timeNode.transform.localPosition}, {rc.rect.height}, {i.CalcRect().height}, {angle} , {vec}");
                pos += (i.CalcRect().height / 2) / Mathf.Sin(angle) * vec;
                i.transform.localPosition = pos;
                pos += (i.CalcRect().height / 2 + gap) / Mathf.Sin(angle) * vec;
            }
        }
        else
        {
            foreach (var i in events)
            {
                //Debug.Log($"height : {timeNode.transform.localPosition}, {rc.rect.height}, {i.CalcRect().height}, {angle} , {vec}");
                pos -= (i.CalcRect().height / 2) / Mathf.Sin(angle) * vec;
                i.transform.localPosition = pos;
                pos -= (i.CalcRect().height / 2 + gap) / Mathf.Sin(angle) * vec;
            }
        }
        var mid = vec * pos.y / 2 / Mathf.Sin(angle);
        var bone_mid = personNode.transform.localPosition.y / 2;
        var offset2 = mid.y - bone_mid;
        foreach (var i in events)
        {
            i.transform.localPosition -= new Vector3(0, offset2);
        }
    }

    public void Activate()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        data.time_node.GetComponent<UITime>().Activate();
        var canvases = GetComponentsInChildren<Canvas>();
        foreach (var c in canvases)
        {
            c.sortingOrder = 3;
        }
        var raycasters = GetComponentsInChildren<GraphicRaycaster>();
        foreach (var r in raycasters)
        {
            r.enabled = true;
        }
    }
    public void Deactivate()
    {
        canvasGroup.alpha = 0.1f;
        canvasGroup.interactable = false;
        data.time_node.GetComponent<UITime>().Deactivate();
        var canvases = GetComponentsInChildren<Canvas>();
        foreach (var c in canvases)
        {
            c.sortingOrder = 0;
        }
        var raycasters = GetComponentsInChildren<GraphicRaycaster>();
        foreach (var r in raycasters)
        {
            r.enabled = false;
        }
    }

    public bool matchFilter(bool genkoku, bool hikoku, bool jijitsu)
    {

        /*if (claim == "")
        {
            if (data.claim_state == "" || data.claim_state == "saibanjo") return true;
        }
        else if (claim == "genkoku")
        {
            if (data.claim_state == claim) return true;
        }
        else if (claim == "hikoku")
        {
            if (data.claim_state == claim) return true;
        }
        else if (claim == "saibanjo")
        {
            if (data.claim_state == claim) return true;
        }*/
        if (genkoku && data.claim_state == "genkoku")
        {
            return true;
        }
        if (hikoku && data.claim_state == "hikoku")
        {
            return true;
        }
        if (jijitsu && (data.claim_state == "" || data.claim_state == "saibanjo"))
        {
            return true;
        }
        return false;
    }

}
