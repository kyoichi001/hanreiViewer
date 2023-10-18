using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITimelineGuide : MonoBehaviour
{

    [SerializeField] RectTransform bar1;
    [SerializeField] RectTransform bar2;
    [SerializeField] TMPro.TextMeshProUGUI timeText;
    [SerializeField] UITimeline timeline;

    // Update is called once per frame
    void Update()
    {
        timeText.text = "";
        var globalUnitLength = timeline.GetTimebarLength();
        bar2.localPosition = bar1.localPosition + new Vector3(globalUnitLength / transform.lossyScale.x, 0);
        var (year, month) = timeline.GetPixelTime();
        if (year > 0)
            timeText.text += $"{year} years";
        if (month > 0)
            timeText.text = $"{month} months";
    }
}
