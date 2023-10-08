using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FishboneData
{
    public string filename;
    public List<HanreiEvent> events;
}

public class FishboneViewManager : SingletonMonoBehaviour<FishboneViewManager>
{
    [System.Serializable]
    public class OnDataLoadedEvent : UnityEvent<FishboneData> { }
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();
    UIFishbone fishbone;
    private void Awake()
    {
        fishbone = FindObjectOfType<UIFishbone>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
