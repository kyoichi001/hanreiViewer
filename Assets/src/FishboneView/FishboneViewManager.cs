using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FishboneViewManager : SingletonMonoBehaviour<FishboneViewManager>
{
    [System.Serializable]
    public class OnFilenamesLoadedEvent : UnityEvent<List<(string, string)>> { }
    public OnFilenamesLoadedEvent OnFilenamesLoaded { get; } = new OnFilenamesLoadedEvent();
    public class OnShowDataEvent : UnityEvent<string, HanreiTokenizedData> { }
    public OnShowDataEvent OnShowData { get; } = new OnShowDataEvent();
    async void Start()
    {
        var token = this.GetCancellationTokenOnDestroy();
        var filenames = HanreiRepository.Instance.GetNames(Application.dataPath + "/" + "Datas");
        var a = new List<(string, string)>();
        foreach (var f in filenames)
        {
            var title = await HanreiRepository.Instance.GetHanreiTitle(f, token);
            a.Add((title, f));
        }
        OnFilenamesLoaded.Invoke(a);
    }
    public async UniTask ShowFishboneUI(string path, CancellationToken token)
    {
        var d = await HanreiRepository.Instance.GetTokenizedData(path, token);
        OnShowData.Invoke(path, d);
    }
}
