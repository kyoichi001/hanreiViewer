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
    public class OnFilenamesLoadedEvent : UnityEvent<List<(string, string)>> { }
    public OnFilenamesLoadedEvent OnFilenamesLoaded { get; } = new OnFilenamesLoadedEvent();
    public class OnShowDataEvent : UnityEvent<string, HanreiTokenizedData> { }
    public OnShowDataEvent OnShowData { get; } = new OnShowDataEvent();
    Dictionary<string, HanreiTokenizedData> data = new Dictionary<string, HanreiTokenizedData>();
    private void Awake()
    {
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data_) =>
        {
            Debug.Log($"Data loaded {path}");
            var a = path.Split("__");
            var b = a[0].Split("\\");
            var c = b[b.Length - 1].Split("/");
            data[c[c.Length - 1]] = data_;
        });
    }
    void Start()
    {
        var filenames = HanreiDataIO.Instance.GetFileNames();
        var a = new List<(string, string)>();
        foreach (var f in filenames)
        {
            a.Add((HanreiDataIO.Instance.GetHanreiTitle(f), f));
        }
        OnFilenamesLoaded.Invoke(a);
    }
    public void ShowFishboneUI(string path)
    {
        OnShowData.Invoke(path, data[path]);
    }
}
