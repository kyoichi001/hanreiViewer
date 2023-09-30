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
        //�e�X�g�p
        var dat = new FishboneData();
        dat.filename = "test";
        var item1 = new HanreiEvent();
        item1.person = "����";
        item1.time = "1999�N";
        item1.acts = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        var item2 = new HanreiEvent();
        item2.person = "����";
        item2.time = "1999�N";
        item2.acts = "bbbbbbbbbbbbbbbbbbbbbbb";
        dat.events = new List<HanreiEvent>
        {
            item1,item2
        };

        OnDataLoaded.Invoke(dat);
    }
}
