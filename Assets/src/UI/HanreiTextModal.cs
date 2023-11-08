using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanreiTextModal : Modal
{
    [SerializeField] TMPro.TextMeshProUGUI text;


    public void Init(string text_)
    {
        text.text = text_;
    }
}
