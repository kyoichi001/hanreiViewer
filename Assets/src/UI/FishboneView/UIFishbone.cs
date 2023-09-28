using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFishbone : MonoBehaviour
{

    [SerializeField] GameObject timeLine;
    [SerializeField] Transform timeStampsContainer;
    [SerializeField] GameObject timeStampPrefab;
    [SerializeField] List<HanreiEvent> data=new List<HanreiEvent>();

    public void SetData(List<HanreiEvent> data)
    {
        this.data = data;
    }
}
