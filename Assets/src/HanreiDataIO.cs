using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HanreiDataIO : SingletonMonoBehaviour<HanreiDataIO>
{

    [SerializeField] string dataFilePath;

    HashSet<string> fileNames = new HashSet<string>();

    public List<string> GetFileNames()
    {
        fileNames.Clear();
        var res = new List<string>();
        if (!Directory.Exists(Application.dataPath + "/" + dataFilePath))
        {
            Directory.CreateDirectory(Application.dataPath + "/" + dataFilePath);
        }
        string[] names = Directory.GetFiles(Application.dataPath + "/" + dataFilePath, "*.json");
        foreach (var i in names)
        {
            var a = i.Split("__");
            var b = a[0].Split("\\");
            var c = b[^1].Split("/");
            fileNames.Add(c[^1]);
        }
        foreach (var i in fileNames)
        {
            res.Add(i);
        }
        return res;
    }

    public HanreiData GetTextData(string filename)
    {
        if (!fileNames.Contains(filename)) return null;
        var path = Application.dataPath + "/" + dataFilePath + "/" + filename + "__text.json";
        StreamReader reader = new StreamReader(path);
        string datastr = reader.ReadToEnd();
        reader.Close();
        return JsonUtility.FromJson<HanreiData>(datastr);
    }
    public string GetHanreiTitle(string filename)
    {
        if (!fileNames.Contains(filename)) return "";
        var dat = GetTextData(filename);
        foreach (var t in dat.contents.signature.texts)
        {
            if (t.EndsWith("事件"))
            {
                return t;
                var v = t.Split(" ");
                return v[^1];
            }
        }
        return "";
    }
    public HanreiTokenizedData GetTokenizedData(string filename)
    {
        if (!fileNames.Contains(filename)) return null;
        var path = Application.dataPath + "/" + dataFilePath + "/" + filename + "__tokenized.json";
        StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8);
        string datastr = reader.ReadToEnd();
        reader.Close();
        return JsonUtility.FromJson<HanreiTokenizedData>(datastr);
    }
}
