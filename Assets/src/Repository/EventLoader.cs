using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EventLoader : MonoBehaviour
{
    [SerializeField] string dirPath;
    public class OnDataLoadedEvent : UnityEvent<EventFileData> { }
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();

    async UniTask<EventData> LoadData(string path)
    {
        using (var reader = new StreamReader(path))
        {
            string datastr = await reader.ReadToEndAsync();
            Debug.Log(datastr);
            return JsonUtility.FromJson<EventData>(datastr);
        }
    }
    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("load annotation data");
        string[] files = Directory.GetFiles(dirPath);
        foreach (var file in files)
        {
            if (!file.EndsWith(".csv")) continue;
            Debug.Log($"loading {file}");
            var events = await LoadData(file);
            var d = new EventFileData
            {
                filename = Path.GetFileName(file).Split(".")[0],
                data = events
            };
            OnDataLoaded.Invoke(d);
        }
    }
}

