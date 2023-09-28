using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawArrow : MonoBehaviour
{
    [Header("From")]
    public RectTransform ui1;
    public float ui1Margin = 0;
    public RectTransform fromHead;
    [Header("To")]
    public RectTransform ui2;
    public float ui2Margin = 0;
    public RectTransform toHead;

    DrawLine drawLine;

    private void Awake()
    {
        drawLine=GetComponentInChildren<DrawLine>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        drawLine.ui1= ui1;
        drawLine.ui1Margin=ui1Margin;
        drawLine.ui2= ui2;
        drawLine.ui2Margin=ui2Margin;

        if (toHead != null)
        {
            toHead.position= drawLine.GetToPos();
            toHead.rotation=Quaternion.Euler(0,0,drawLine.GetAngle()*Mathf.Rad2Deg+90);
        }
        if (fromHead != null)
        {
            fromHead.position = drawLine.GetFromPos();
            fromHead.rotation=Quaternion.Euler(0,0,drawLine.GetAngle() * Mathf.Rad2Deg-90);
        }

    }
}
