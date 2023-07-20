using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.Events;

public class DataLoader : MonoBehaviour
{
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
    public Token GetToken(string filename,int textID,int tokenID)
    {
        foreach(var i in hanreiDatas)
        {
            if (i.filename != filename) continue;
            foreach(var j in i.contents.fact_reason.sections)
            {
                foreach(var k in j.texts)
                {
                    foreach(var l in k.bunsetu)
                    {
                        foreach(var m in l.tokens)
                        {
                            if (textID == k.text_id && tokenID == m.id) return m;
                        }
                    }
                }
            }
        }
        return null;
    }

}
