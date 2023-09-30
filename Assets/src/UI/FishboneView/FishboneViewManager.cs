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
        var item1 = new HanreiEvent
        {
            person = "����",
            time = "1999�N",
            value = 1999,
            acts = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        };
        var item2 = new HanreiEvent
        {
            person = "����",
            time = "1999�N",
            value = 1999,
            acts = "bbbbbbbbbbbbbbbbbbbbbbb"
        };
        var item3 = new HanreiEvent
        {
            person = "�퍐",
            time = "2001�N",
            value = 2001,
            acts = "cccc"
        };
        dat.events = new List<HanreiEvent>
        {
            item1,item2,item3
        };

        OnDataLoaded.Invoke(dat);
    }
}
