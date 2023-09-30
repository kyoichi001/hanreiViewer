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
    [SerializeField] string fishboneDataPath;

    UIFishbone fishbone;
    private void Awake()
    {
        fishbone = FindObjectOfType<UIFishbone>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //テスト用
        var dat = new FishboneData();
        dat.filename = "test";
        var item1 = new HanreiEvent();
        item1.person = "原告";
        item1.time = "1999年";
        item1.acts = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        var item2 = new HanreiEvent();
        item2.person = "原告";
        item2.time = "1999年";
        item2.acts = "bbbbbbbbbbbbbbbbbbbbbbb";
        dat.events = new List<HanreiEvent>
        {
            item1,item2
        };

        OnDataLoaded.Invoke(dat);
    }
}
