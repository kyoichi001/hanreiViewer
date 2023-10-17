using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIEvent : MonoBehaviour
{
    [SerializeField] float maxHeight = 80;
    [SerializeField] GameObject actNode;
    TMPro.TextMeshProUGUI actText;
    [SerializeField] GameObject boneLine;
    Canvas canvas;

    private void Awake()
    {
        actText = actNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        canvas = GetComponent<Canvas>();
        var button = actNode.GetComponent<Button>();
        var trigger = actNode.GetComponent<EventTrigger>();
        var enter_entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enter_entry.callback.AddListener((data) =>
        {
            //Debug.Log($"hovering event! : {actText.text}");
            canvas.sortingOrder = 10;
            var height = actText.preferredHeight;
            var rt = actNode.transform as RectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        });
        trigger.triggers.Add(enter_entry);
        var exit_entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit_entry.callback.AddListener((data) =>
        {
            canvas.sortingOrder = 9;
            var height = actText.preferredHeight;
            height = Mathf.Min(height, maxHeight);
            var rt = actNode.transform as RectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        });
        trigger.triggers.Add(exit_entry);
    }

    public Rect CalcRect()
    {
        var res = new Rect();
        var rc1 = boneLine.transform as RectTransform;
        var rc2 = actNode.transform as RectTransform;

        res.yMin = Mathf.Min(rc1.rect.yMin, rc2.rect.yMin);
        res.yMax = Mathf.Max(rc1.rect.yMax, rc2.rect.yMax);
        res.xMin = Mathf.Min(rc1.rect.xMin, rc2.rect.xMin);
        res.xMax = Mathf.Max(rc1.rect.xMax, rc2.rect.xMax);
        return res;
    }
    public void SetActWidth(float width)
    {
        var rt = actNode.transform as RectTransform;
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }
    public float GetActWidth()
    {
        var rt = actNode.transform as RectTransform;
        return rt.sizeDelta.x;
    }
    public void SetBoneWidth(float width)
    {
        var rt = boneLine.transform as RectTransform;
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }
    public float GetBoneWidth()
    {
        var rt = boneLine.transform as RectTransform;
        return rt.sizeDelta.x;
    }

    public void SetData(string act)
    {
        actText.text = act;
        var height = actText.preferredHeight;
        height = Mathf.Min(height, maxHeight);
        var rt = actNode.transform as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }
}
