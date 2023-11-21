using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIEvent : MonoBehaviour
{
    public UnityEvent OnButtonClicked { get; } = new();
    [SerializeField] float maxHeight = 80;
    [SerializeField] GameObject actNode;
    TMPro.TextMeshProUGUI actText;
    [SerializeField] GameObject boneLine;
    [SerializeField] GameObject modalPrefab;
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
        button.onClick.AddListener(() =>
        {
            OnButtonClicked.Invoke();
        });
    }

    public Rect CalcRect()
    {
        var rc1 = boneLine.transform as RectTransform;
        var rc2 = actNode.transform as RectTransform;
        return Utility.GetContainsRect(rc1.rect, rc2.rect);
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
