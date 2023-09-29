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
        actText=actNode.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public float CalcHeight()
    {
        return 0;
    }
    public void SetActWidth(float width)
    {

    }
    public void SetBoneWidth(float width)
    {

    }
    public void SetData(string act)
    {
        actText.text = act;
    }




}
