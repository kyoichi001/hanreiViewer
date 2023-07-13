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
            var ss = new List<TokenRelation>
            {
                new TokenRelation(),
                new TokenRelation(),
                new TokenRelation(),
            };
            ss[0].targetID = int.Parse(l[2]);
            ss[0].type = (TokenRelationType)int.Parse(l[3]);
            ss[1].targetID = int.Parse(l[4]);
            ss[1].type = (TokenRelationType)int.Parse(l[5]);
            ss[2].targetID = int.Parse(l[6]);
            ss[2].type = (TokenRelationType)int.Parse(l[7]);
            anno.relations = ss;
            list.Add(anno);
        }
        return list;
    }

    void SaveData(List<TokenAnnotation> data, string filepath)
    {
        if (data == null) return;
        var sw = new StreamWriter(filepath);
        sw.WriteLine("textID,tokenID,rID1,rType1,rID2,rType2,rID3,rType3");
        foreach (var annotation in data)
        {
            var res = $"{annotation.textID},{annotation.tokenID}";
            foreach (var i in annotation.relations)
            {
                res += $",{i.targetID},{i.type}";
            }
            sw.WriteLine(res);
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

    public void AddRelation(int textID, int tokenID, int targetID, TokenRelationType type)
    {

        foreach (var d in anotationDatas)
        {
            SaveData(d.annotations, d.filename);
        }
    }
    public void RemoveRelation()
    {

        foreach (var d in anotationDatas)
        {
            SaveData(d.annotations, d.filename);
        }
    }
    public void UpdateRelation()
    {

        foreach (var d in anotationDatas)
        {
            SaveData(d.annotations, d.filename);
        }
    }
}
