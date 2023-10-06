using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITime : MonoBehaviour
{

    TMPro.TextMeshProUGUI text;
    private void Awake()
    {
        text=GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    public void Init(string text_)
    {
        text.text = text_;
    }
}
