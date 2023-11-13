using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public class OnFilenamesLoadedEvent : UnityEvent<List<(string, string)>> { }
    public OnFilenamesLoadedEvent OnFilenamesLoaded { get; } = new OnFilenamesLoadedEvent();
    public class OnShowDataEvent : UnityEvent<string, HanreiTokenizedData> { }
    public OnShowDataEvent OnShowData { get; } = new OnShowDataEvent();
    readonly Dictionary<string, HanreiTokenizedData> data = new Dictionary<string, HanreiTokenizedData>();
    private void Awake()
    {
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data_) =>
        {
            Debug.Log($"Data loaded {path}");
            var c = Path.GetFileNameWithoutExtension(path).Split("__")[0];
            data[c] = data_;
        });
        /*var a = new System.DateTime(2000, 1, 1);
        var b = new System.DateTime(2003, 4, 1);
        var c = new System.DateTime(2005, 12, 10);
        var d = new System.DateTime(2010, 12, 31);
        Debug.Log((b - a).TotalDays);
        Debug.Log((c - a).TotalDays);
        Debug.Log((d - a).TotalDays);*/
    }
    async void Start()
    {
        var filenames = HanreiDataIO.Instance.GetFileNames();
        var a = new List<(string, string)>();
        foreach (var f in filenames)
        {
            var title = await HanreiDataIO.Instance.GetHanreiTitle(f);
            a.Add((title, f));
        }
        OnFilenamesLoaded.Invoke(a);
    }
    public void ShowFishboneUI(string path)
    {
        OnShowData.Invoke(path, data[path]);
    }
}
