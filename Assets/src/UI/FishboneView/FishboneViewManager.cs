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
    public class OnShowDataEvent : UnityEvent<string, HanreiTokenizedData> { }
    public OnShowDataEvent OnShowData { get; } = new OnShowDataEvent();
    Dictionary<string, HanreiTokenizedData> data = new Dictionary<string, HanreiTokenizedData>();
    private void Awake()
    {
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data_) =>
        {
            data[path] = data_;
        });
    }
    public void ShowFishboneUI(string path)
    {
        OnShowData.Invoke(path, data[path]);
    }
}
