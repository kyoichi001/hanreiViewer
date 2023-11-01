using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [Header("From")]
    public RectTransform ui1;
    public float ui1Margin = 0;
    [Header("To")]
    public RectTransform ui2;
    public float ui2Margin = 0;

    void CalcTransform()
    {
        if (ui1 == null || ui2 == null) return;
        var offset = transform.parent.InverseTransformPoint(ui2.position) - transform.parent.InverseTransformPoint(ui1.position);
        var angle = Mathf.Atan2(offset.y, offset.x);
        var posFrom = ui1.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * ui1Margin;
        var posTo = ui2.position - new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * ui2Margin;
        var middlePos = (posFrom + posTo) / 2;
        transform.position = middlePos;
        var rt = transform as RectTransform;
        rt.sizeDelta = new Vector2(offset.magnitude - ui1Margin - ui2Margin, 16);
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }

    public Vector3 GetFromPos()
    {
        if (ui1 == null || ui2 == null) return new Vector3(float.NaN, float.NaN, float.NaN);
        var offset = ui2.position - ui1.position;
        var angle = Mathf.Atan2(offset.y, offset.x);
        return ui1.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * ui1Margin;
    }
    public Vector3 GetToPos()
    {
        if (ui1 == null || ui2 == null) return new Vector3(float.NaN, float.NaN, float.NaN);
        var offset = ui2.position - ui1.position;
        var angle = Mathf.Atan2(offset.y, offset.x);
        return ui2.position - new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * ui2Margin;
    }
    public float GetAngle()
    {
        var offset = ui2.position - ui1.position;
        return Mathf.Atan2(offset.y, offset.x);
    }

    private void Update()
    {
        CalcTransform();
    }
}
