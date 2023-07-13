using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.Events;

public class DataLoader : MonoBehaviour
{
    [System.Serializable]
    public class OnDataLoadedEvent:UnityEvent<HanreiData>{}

    public string DataPath;
    public List<HanreiData> hanreiDatas=new List<HanreiData>();
    public OnDataLoadedEvent OnDataLoaded =new OnDataLoadedEvent();

    HanreiData LoadData(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        Debug.Log(datastr);
        return JsonUtility.FromJson<HanreiData>(datastr);
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("load hanrei data");
        string[] files = Directory.GetFiles(DataPath);
        foreach (var file in files)
        {
            if(!file.EndsWith(".json")) continue;
            Debug.Log($"loading {file}");
            var dat = LoadData(file);
            dat.filename = Path.GetFileName(file).Split(".")[0];
            hanreiDatas.Add(dat);
            OnDataLoaded.Invoke(dat);
        }
    }

}
