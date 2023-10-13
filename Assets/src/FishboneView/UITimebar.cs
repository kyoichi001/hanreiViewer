using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITimebar : MonoBehaviour
{
    [SerializeField] GameObject timePopup;
    [SerializeField] TMPro.TextMeshProUGUI timeText;

    void Awake()
    {
        var eventTrigger = GetComponent<EventTrigger>();

        var enter_entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enter_entry.callback.AddListener((data) =>
        {
            timePopup.SetActive(true);
        });
        eventTrigger.triggers.Add(enter_entry);
        var exit_entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit_entry.callback.AddListener((data) =>
        {
            timePopup.SetActive(false);
        });
        eventTrigger.triggers.Add(exit_entry);
        timePopup.SetActive(false);

    }

    public void Init(System.DateTime date)
    {
        timeText.text = date.ToString("yyyy/MM/dd");
    }
}
