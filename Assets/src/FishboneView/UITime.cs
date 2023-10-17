using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeType
{
    point,
    begin_end,
    begin,
    end
}
[System.Serializable]
public class UITimeData
{
    public int ID;
    public System.DateTime? begin_time;
    public System.DateTime? end_time;
    public string text;
    public TimeType timeType;
    public bool is_top;
    public int layer;
}

public class UITime : MonoBehaviour
{
    public UITimeData data;
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
    public void Init(UITimeData data_)
    {
        data = data_;
        textUI.text = data.text;
        begin_time = data.begin_time.ToString();
        end_time = data.end_time.ToString();
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
