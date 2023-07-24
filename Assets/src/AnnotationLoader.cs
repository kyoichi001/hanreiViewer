using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using static Unity.VisualScripting.AnnotationUtility;
using static UnityEngine.Rendering.DebugUI.Table;

public class AnnotationLoader : SingletonMonoBehaviour<AnnotationLoader>
{
    public string dirPath;
    private DataLoader loader;
    public List<AnotationData> anotationDatas = new List<AnotationData>();

    public class OnDataLoadedEvent : UnityEvent<AnotationData> { }
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();
    public class OnDataCangedEvent : UnityEvent<AnotationData> { }
    public OnDataCangedEvent OnDataChanged { get; }=new OnDataCangedEvent();


    (List<TokenRelation>, List<TokenTag>) LoadData(string path)
    {
        if (!System.IO.File.Exists(path)) return (new List<TokenRelation>(), new List<TokenTag>());
        string content = "";
        using (var sr = new StreamReader(path))
        {
            content = sr.ReadToEnd();
        }
        Debug.Log(content);
        var list = new List<TokenRelation>();
        var fileDat = content.Split("\n");
        var i = 1;//header”ò‚Î‚µ
        while (fileDat[i][0] != '#')
        {
            var l = fileDat[i].Split(",");
            if (l.Length < 4) break;
            var anno = new TokenRelation
            {
                textID = int.Parse(l[0]),
                tokenID = int.Parse(l[1]),
                targetID = int.Parse(l[2]),
                type = (TokenRelationType)int.Parse(l[3])
            };
            list.Add(anno);
            i++;
        }
        i++;//#tags
        i++;//#tags header
        var tags = new List<TokenTag>();
        while (i < list.Count)
        {
            var l = fileDat[i].Split(",");
            if (l.Length < 3) break;
            var anno = new TokenTag
            {
                textID = int.Parse(l[0]),
                tokenID = int.Parse(l[1]),
                type = (TokenTagType)int.Parse(l[2])
            };
            tags.Add(anno);
        }
        return (list, tags);
    }

    void SaveData(AnotationData data)
    {
        if (data == null) return;
        Debug.Log($"save relation filepath : {dirPath}/{data.filename}.csv");
        using (var sw = new StreamWriter($"{dirPath}/{data.filename}.csv"))
        {
            sw.WriteLine("textID,tokenID,targetID,type");
            foreach (var annotation in data.annotations)
            {
                var res = $"{annotation.textID},{annotation.tokenID},{annotation.targetID},{((int)annotation.type)}";
                sw.WriteLine(res);
            }
            sw.WriteLine("#tags");
            sw.WriteLine("textID,tokenID,type");
            foreach (var tag in data.tags)
            {
                var res = $"{tag.textID},{tag.tokenID},{((int)tag.type)}";
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
            var (dat,tags) = LoadData(file);
            var d = new AnotationData
            {
                annotations = dat,
                tags=tags,
                filename = Path.GetFileName(file).Split(".")[0]
            };
            anotationDatas.Add(d);
            OnDataLoaded.Invoke(d);
        }
        OnDataChanged.AddListener((d) =>
        {
            SaveData(d);
        });
    }
    private void OnApplicationQuit()
    {
        foreach(var d in anotationDatas)
        {
            SaveData(d);
        }
    }
    public bool ExistsData(string filename)
    {
        foreach (var d in anotationDatas)
            if (d.filename == filename) return true;
        return false;
    }

    public List<TokenRelation> GetAnnotations(string filename)
    {
        //var res=new List<TokenAnnotation>();
        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            return d.annotations;
        }
        return new List<TokenRelation>();
    }
    public AnotationData GetAnotationData(string filename)
    {
        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            return d;
        }
        return null;
    }

    public void AddRelation(string filename,int textID, int tokenID, int targetID, TokenRelationType type)
    {
        Debug.Log("add relation");
        if (!ExistsData(filename))
        {
            var dat= new List<TokenRelation>();
            var d = new AnotationData
            {
                annotations = dat,
                filename = filename
            };
            anotationDatas.Add(d);
        }
        foreach (var d in anotationDatas)
        {
            if(d.filename != filename) continue;
            var a = new TokenRelation
            {
                textID = textID,
                tokenID = tokenID,
                targetID = targetID,
                type = type
            };
            d.annotations.Add(a);
            OnDataChanged.Invoke(d);
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
            OnDataChanged.Invoke(d);
        }
    }
    public void UpdateRelation(string filename, int textID, int tokenID, int targetID, TokenRelationType type)
    {
        Debug.Log("update relation");
        if (!ExistsData(filename))
        {
            AddRelation(filename,textID,tokenID,targetID,type);
            return;
        }

        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            d.annotations.Find(x=>x.textID == textID && x.tokenID==tokenID && x.targetID==targetID).type=type;
            OnDataChanged.Invoke(d);
        }
    }

    public void AddTag(string filename, int textID, int tokenID,TokenTagType type)
    {
        if (!ExistsData(filename)) return;
        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            d.tags.Add(new TokenTag
            {
                textID = textID,
                tokenID = tokenID,
                type = type
            });
            OnDataChanged.Invoke(d);
        }
    }
    public void RemoveTag(string filename, int textID, int tokenID)
    {
        if (!ExistsData(filename)) return;
        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue; 
            d.tags.RemoveAll(x => x.textID == textID && x.tokenID == tokenID);
            OnDataChanged.Invoke(d);
        }
    }
    public void UpdateTag(string filename, int textID, int tokenID, TokenTagType type)
    {
        Debug.Log("update tag");
        if (!ExistsData(filename))
        {
            AddTag(filename, textID, tokenID, type);
            return;
        }

        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            d.tags.Find(x=>x.textID == textID && x.tokenID==tokenID).type=type;
            OnDataChanged.Invoke(d);
        }
    }
}
