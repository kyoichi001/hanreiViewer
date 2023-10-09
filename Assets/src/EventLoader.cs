using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class EventLoader : MonoBehaviour
{
    [SerializeField] string dirPath;
    public class OnDataLoadedEvent : UnityEvent<EventFileData> { }
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();

    EventData LoadData(string path)
    {
        StreamReader reader = new StreamReader(path);
        string datastr = reader.ReadToEnd();
        reader.Close();
        Debug.Log(datastr);
        return JsonUtility.FromJson<EventData>(datastr);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("load annotation data");
        string[] files = Directory.GetFiles(dirPath);
        foreach (var file in files)
        {
            if (!file.EndsWith(".csv")) continue;
            Debug.Log($"loading {file}");
            var events = LoadData(file);
            var d = new EventFileData
            {
                filename = Path.GetFileName(file).Split(".")[0],
                data = events
            };
            OnDataLoaded.Invoke(d);
        }
    }
}

