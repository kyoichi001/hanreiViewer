using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class AnnotationLoader : MonoBehaviour
{
    public string dirPath;
    private DataLoader loader;
    public List<AnotationData> anotationDatas = new List<AnotationData>();

    [System.Serializable]
    public class OnDataLoadedEvent : UnityEvent<AnotationData> { }
    public OnDataLoadedEvent OnDataLoaded = new OnDataLoadedEvent();

    List<TokenAnnotation> LoadData(string path)
    {
        var reader = new StreamReader(path);
        var datastr = reader.ReadToEnd();
        reader.Close();
        Debug.Log(datastr);
        var list = new List<TokenAnnotation>();
        bool header = true;
        foreach (var row in datastr.Split("\n"))
        {
            if (header)
            {
                header = false;
                continue;
            }
            var l = row.Split(",");
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
        var sw = new StreamWriter($"{dirPath}/{filename}.csv");
        sw.WriteLine("textID,tokenID,targetID,rType1");
        foreach (var annotation in data)
        {
            var res = $"{annotation.textID},{annotation.tokenID},{annotation.targetID},{annotation.type}";
            sw.WriteLine(res);
        }
        sw.Close();
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
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnApplicationQuit()
    {
        foreach(var d in anotationDatas)
        {
            SaveData(d.annotations, d.filename);
        }
    }
    public List<TokenAnnotation> GetAnnotations(string filename)
    {
        //var res=new List<TokenAnnotation>();
        foreach (var d in anotationDatas)
        {
            if (d.filename != filename) continue;
            return d.annotations;
        }
        return null;
    }

    public void AddRelation(string filename,int textID, int tokenID, int targetID, TokenRelationType type)
    {
        Debug.Log("add relation");
        foreach (var d in anotationDatas)
        {
            if(d.filename != filename) continue;
            var a = new TokenAnnotation();
            a.textID=textID;
            a.tokenID=tokenID;
            a.targetID=targetID;
            a.type=type;
            d.annotations.Add(a);
            SaveData(d.annotations, d.filename);
        }
    }
    public void RemoveRelation(string filename, int textID, int tokenID, int targetID)
    {

        Debug.Log("remove relation");
        foreach (var d in anotationDatas)
        {
            SaveData(d.annotations, d.filename);
        }
    }
    public void UpdateRelation(string filename, int textID, int tokenID, int targetID, TokenRelationType type)
    {
        Debug.Log("update relation");

        foreach (var d in anotationDatas)
        {
            SaveData(d.annotations, d.filename);
        }
    }
}
