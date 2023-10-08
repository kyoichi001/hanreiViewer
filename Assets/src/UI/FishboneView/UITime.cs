using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITime : MonoBehaviour
{
    public System.DateTime begin;
    public System.DateTime end;
    public int layer;
    public bool is_range;
    public string text;

    [SerializeField]string time_text;

    TMPro.TextMeshProUGUI textUI;
    private void Awake()
    {
        textUI=GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    public void Init(System.DateTime begin_, System.DateTime end_,bool is_range_,int layer_,string text_)
    {
        begin=begin_;
        is_range=is_range_;
        end=end_;
        text=text_;
        layer=layer_;
        textUI.text = text_;
        time_text=begin_.ToShortDateString()+end_.ToShortDateString();
    }
}
