using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Threading;

public class EventDataLoader : SingletonMonoBehaviour<EventDataLoader>
{
    public string filepath;

    [SerializeField] List<HanreiTokenizedData> datas = new List<HanreiTokenizedData>();
    public class OnDataLoadedEvent : UnityEvent<string, HanreiTokenizedData> { }
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();
    async UniTask<HanreiTokenizedData> LoadData(string path, CancellationToken token)
    {
        using (var reader = new StreamReader(path, System.Text.Encoding.UTF8))
        {
            string datastr = await reader.ReadToEndAsync();
            token.ThrowIfCancellationRequested();
            var jsonData = JsonUtility.FromJson<HanreiTokenizedData>(datastr);
            //Debug.Log(datastr);
            foreach (var data in jsonData.datas)
            {
                foreach (var e in data.events)
                {
                    e.issue_num = data.issue_num;
                    e.claim_state = data.claim_state ?? "";
                }
            }
            return jsonData;
        }
    }

    async void Start()
    {
        var token = this.GetCancellationTokenOnDestroy();
        string[] files = Directory.GetFiles(Application.dataPath + "/" + filepath);
        foreach (var file in files)
        {
            if (!file.EndsWith(".json")) continue;
            //Debug.Log($"loading {file}");
            var dat = await LoadData(file, token);
            datas.Add(dat);
            OnDataLoaded.Invoke(file, dat);
        }
    }
}
