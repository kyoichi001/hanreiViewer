using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaleless : MonoBehaviour
{
    public bool scaleX = true;
    public bool scaleY = true;

    // Update is called once per frame
    void Update()
    {
        var scale = transform.parent.lossyScale;
        transform.localScale = new Vector3(scaleX ? 1 / scale.x : 1, scaleY ? 1 / scale.y : 1, 1);
    }
}
