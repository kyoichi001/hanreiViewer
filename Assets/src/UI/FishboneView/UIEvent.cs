using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvent : MonoBehaviour
{
    [SerializeField] GameObject actNode;
    TMPro.TextMeshProUGUI actText;
    [SerializeField] GameObject boneLine;
    private void Awake()
    {
        actText = actNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public Rect CalcRect()
    {
        var res = new Rect();
        var rc1 = boneLine.transform as RectTransform;
        var rc2 = actNode.transform as RectTransform;

        res.yMin = Mathf.Min(rc1.rect.yMin, rc2.rect.yMin);
        res.yMax = Mathf.Max(rc1.rect.yMax, rc2.rect.yMax);
        res.xMin = Mathf.Min(rc1.rect.xMin, rc2.rect.xMin);
        res.xMax = Mathf.Max(rc1.rect.xMax, rc2.rect.xMax);
        return res;
    }
    public void SetActWidth(float width)
    {
        var rt = actNode.transform as RectTransform;
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }
    public float GetActWidth()
    {
        var rt = actNode.transform as RectTransform;
        return rt.sizeDelta.x;
    }
    public void SetBoneWidth(float width)
    {
        var rt = boneLine.transform as RectTransform;
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }
    public float GetBoneWidth()
    {
        var rt = boneLine.transform as RectTransform;
        return rt.sizeDelta.x;
    }

    public void SetData(string act)
    {
        actText.text = act;
        var height = actText.preferredHeight;

        var rt = actNode.transform as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }
}
