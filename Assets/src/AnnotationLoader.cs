using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class AnnotationLoader : SingletonMonoBehaviour<AnnotationLoader>
{
    public string dirPath;
    private DataLoader loader;
    public List<AnotationData> anotationDatas = new List<AnotationData>();

    public class OnDataLoadedEvent : UnityEvent<AnotationData> { }
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();
    public class OnDataCangedEvent : UnityEvent<AnotationData> { }
    public OnDataCangedEvent OnDataCanged { get; }=new OnDataCangedEvent();


    List<TokenAnnotation> LoadData(string path)
    {
        if (!System.IO.File.Exists(path)) return new List<TokenAnnotation>();
        string content = "";
        using(var sr=new StreamReader(path))
        {
            content = sr.ReadToEnd();
        }
        Debug.Log(content);
        var list = new List<TokenAnnotation>();
        bool header = true;
        foreach (var row in content.Split("\n"))
        {
            if (header)
            {
                header = false;
                continue;
            }
            var l = row.Split(",");
            if (l.Length < 4) break;
            var anno = new TokenAnnotation();
            anno.textID = int.Parse(l[0]);
            anno.tokenID = int.Parse(l[1]);
            anno.targetID = int.Parse(l[2]);
            anno.type = (TokenRelationType)int.Parse(l[3]);
            list.Add(anno);
        }
        return list;
    }

    void SaveData(List<TokenAnnotation> data, string filename)
    {
        if (data == null) return;
        Debug.Log($"save relation filepath : {dirPath}/{filename}.csv");
        using (var sw = new StreamWriter($"{dirPath}/{filename}.csv"))
        {
            sw.WriteLine("textID,tokenID,targetID,type");
            foreach (var annotation in data)
            {
                var res = $"{annotation.textID},{annotation.tokenID},{annotation.targetID},{((int)annotation.type)}";
                sw.WriteLine(res);
            }
        }
    }


    private void Awake()
    {
        loader=GetComponent<DataLoader>();
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
            var dat = LoadData(file);
            var d = new AnotationData();
            d.annotations = dat;
            d.filename = Path.GetFileName(file).Split(".")[0];
            anotationDatas.Add(d);
            OnDataLoaded.Invoke(d);
        }
        OnDataCanged.AddListener((d) =>
        {
            SaveData(d.annotations,d.filename);
        });
    }
    private void OnApplicationQuit()
    {
        foreach(var d in anotationDatas)
        {
            SaveData(d.annotations, d.filename);
        }
    }
    public bool ExistsData(string filename)
    {
        foreach (var d in anotationDatas)
            if (d.filename == filename) return true;
        return false;
    }

    public List<TokenAnnotation> GetAnnotations(string filename)
    {
        //var res=new List<TokenAnnotation>();
        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            return d.annotations;
        }
        return new List<TokenAnnotation>();
    }

    public void AddRelation(string filename,int textID, int tokenID, int targetID, TokenRelationType type)
    {
        Debug.Log("add relation");
        if (!ExistsData(filename))
        {
            var dat= new List<TokenAnnotation>();
            var d = new AnotationData();
            d.annotations = dat;
            d.filename = filename;
            anotationDatas.Add(d);
        }
        foreach (var d in anotationDatas)
        {
            if(d.filename != filename) continue;
            var a = new TokenAnnotation();
            a.textID=textID;
            a.tokenID=tokenID;
            a.targetID=targetID;
            a.type=type;
            d.annotations.Add(a);
            OnDataCanged.Invoke(d);
        }
    }

    public bool ExistsAnnotation(string filename, int textID, int tokenID)
    {
        if (!ExistsData(filename)) return false;

        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            foreach (var i in d.annotations)
            {
                if (i.textID == textID && i.tokenID == tokenID) return true;
            }
        }
        return false;
    }

    public void RemoveRelation(string filename, int textID, int tokenID, int targetID)
    {
        if (!ExistsData(filename)) return;
        Debug.Log("remove relation");
        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            d.annotations.RemoveAll(x => x.textID == textID && x.tokenID == tokenID && x.targetID == targetID);
            OnDataCanged.Invoke(d);
        }
    }
    public void UpdateRelation(string filename, int textID, int tokenID, int targetID, TokenRelationType type)
    {
        Debug.Log("update relation");

        foreach (var d in anotationDatas)
        {
            OnDataCanged.Invoke(d);
        }
    }
}
