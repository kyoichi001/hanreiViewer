using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class EventDataLoader : SingletonMonoBehaviour<EventDataLoader>
{
    public string filepath;

    [SerializeField] List<HanreiTokenizedData> datas = new List<HanreiTokenizedData>();
    public class OnDataLoadedEvent : UnityEvent<string, HanreiTokenizedData> { }
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();
    HanreiTokenizedData LoadData(string path)
    {
        StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8);
        string datastr = reader.ReadToEnd();
        reader.Close();
        //Debug.Log(datastr);
        var jsonData = JsonUtility.FromJson<HanreiTokenizedData>(datastr);
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

    // Start is called before the first frame update
    void Start()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/" + filepath);
        foreach (var file in files)
        {
            if (!file.EndsWith(".json")) continue;
            //Debug.Log($"loading {file}");
            var dat = LoadData(file);
            datas.Add(dat);
            OnDataLoaded.Invoke(file, dat);
        }
    }
}
