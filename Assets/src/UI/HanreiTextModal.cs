using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanreiTextModal : Modal
{
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] TMPro.TextMeshProUGUI person_text;
    [SerializeField] TMPro.TextMeshProUGUI time_text;
    [SerializeField] TMPro.TextMeshProUGUI act_text;


    public void Init(string text_, string person, string time, string act)
    {
        text.text = text_;
        person_text.text = person;
        time_text.text = time;
        act_text.text = act;
    }
}
