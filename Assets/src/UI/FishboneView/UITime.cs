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
    public int layer;
}

public class UITime : MonoBehaviour
{
    public UITimeData data;

    TMPro.TextMeshProUGUI textUI;
    private void Awake()
    {
        textUI = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    public void Init(UITimeData data_)
    {
        data = data_;
        textUI.text = data.text;
    }
}
