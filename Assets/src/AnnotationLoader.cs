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

    AnotationData LoadData(string path)
    {
        var reader = new StreamReader(path);
        var datastr = reader.ReadToEnd();
        reader.Close();
        Debug.Log(datastr);
        var list = new List<TokenAnnotation>();
        foreach (var row in datastr.Split("\n"))
        {
            var l = row.Split(",");
            var anno = new TokenAnnotation();
            anno.textID = int.Parse(l[0]);
            anno.tokenID = int.Parse(l[0]);
            var ss = new List<TokenRelation>
            {
                new TokenRelation(),
                new TokenRelation(),
                new TokenRelation(),
            };
            ss[0].targetID = int.Parse(l[0]);
            ss[0].type = (TokenRelationType)int.Parse(l[0]);
            ss[1].targetID = int.Parse(l[0]);
            ss[1].type = (TokenRelationType)int.Parse(l[0]);
            ss[2].targetID = int.Parse(l[0]);
            ss[2].type = (TokenRelationType)int.Parse(l[0]);
            anno.relations = ss;
            list.Add(anno);
        }
        AnotationData res = new AnotationData();
        res.filename = path;
        res.annotations = list;
        return res;
    }

    void SaveData(AnotationData data, string filepath)
    {
        if (data == null) return;
        var sw = new StreamWriter(filepath);
        foreach(var annotation in data.annotations)
        {
            var res = $"{annotation.textID},{annotation.tokenID},tag,annotationTag,";
            foreach(var i in annotation.relations)
            {
                res += $"{i.targetID},{i.type},";
            }
            sw.WriteLine(res);
        }
    }


    private void Awake()
    {
        loader=GetComponent<DataLoader>();
        loader.OnDataLoaded.AddListener((data) =>
        {
            //LoadData(data.filename);
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("load annotation data");
        string[] files = Directory.GetFiles(dirPath);
        foreach (var file in files)
        {
            if (!file.EndsWith(".json")) continue;
            Debug.Log($"loading {file}");
            var dat = LoadData(file);
            dat.filename = Path.GetFileName(file);
            anotationDatas.Add(dat);
            OnDataLoaded.Invoke(dat);
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
            SaveData(d, d.filename);
        }
    }

    void AddRelation(int textID, int tokenID, int targetID, TokenRelationType type)
    {

        foreach (var d in anotationDatas)
        {
            SaveData(d, d.filename);
        }
    }
    void RemoveRelation()
    {

        foreach (var d in anotationDatas)
        {
            SaveData(d, d.filename);
        }
    }
    void UpdateRelation()
    {

        foreach (var d in anotationDatas)
        {
            SaveData(d, d.filename);
        }
    }
}
