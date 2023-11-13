using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UITimeData
{
    public int ID;
    public DurationTime time;
    public string text;
    public bool is_top;
}

public class UITime : MonoBehaviour
{
    public UISubTimeline.TimelineNodeData data;
    [Header("Debug")]
    [SerializeField, ReadOnly] string begin_time;
    [SerializeField, ReadOnly] string end_time;
    CanvasGroup canvasGroup;
    TMPro.TextMeshProUGUI textUI;
    private void Awake()
    {
        textUI = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Init(UISubTimeline.TimelineNodeData data_)
    {
        data = data_;
        textUI.text = data.text;
        begin_time = data.time.begin.ToString();
        end_time = data.time.end.ToString();
    }
    public void Activate()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }
    public void Deactivate()
    {
        canvasGroup.alpha = 0.1f;
        canvasGroup.interactable = false;
    }
}
