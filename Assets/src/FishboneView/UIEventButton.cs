using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventButton : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI idText;
    [SerializeField] TMPro.TextMeshProUGUI personText;
    [SerializeField] TMPro.TextMeshProUGUI timeText;


    public void Init(string ID, string person, string time)
    {
        idText.text = ID;
        personText.text = person;
        timeText.text = time;
    }
}
