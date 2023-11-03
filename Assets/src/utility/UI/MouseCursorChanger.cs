using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class MouseCursorChanger : MonoBehaviour
{
    public Texture2D hoverCursor;
    void Awake()
    {
        var trigger = GetComponent<EventTrigger>();
        var enter_entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enter_entry.callback.AddListener((data) =>
        {
            Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width / 2, hoverCursor.height / 2), CursorMode.Auto);
        });
        trigger.triggers.Add(enter_entry);
        var exit_entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit_entry.callback.AddListener((data) =>
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        });
        trigger.triggers.Add(exit_entry);
    }

}
