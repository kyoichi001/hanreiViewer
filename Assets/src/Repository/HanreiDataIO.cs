using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HanreiDataIO : SingletonMonoBehaviour<HanreiDataIO>
{

    [SerializeField] string dataFilePath;

    readonly HashSet<string> fileNames = new HashSet<string>();

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
            var c = Path.GetFileNameWithoutExtension(i).Split("__")[0];
            fileNames.Add(c);
        }
        foreach (var i in fileNames)
        {
            res.Add(i);
        }
        return res;
    }

    public async UniTask<HanreiData> GetTextData(string filename, CancellationToken token)
    {
        if (!fileNames.Contains(filename)) return null;
        var path = Application.dataPath + "/" + dataFilePath + "/" + filename + "__text.json";
        using (var reader = new System.IO.StreamReader(path, System.Text.Encoding.UTF8))
        {
            string datastr = await reader.ReadToEndAsync();
            token.ThrowIfCancellationRequested();
            return JsonUtility.FromJson<HanreiData>(datastr);
        }
    }
    public async UniTask<string> GetHanreiTitle(string filename, CancellationToken token)
    {
        if (!fileNames.Contains(filename)) return "";
        var dat = await GetTextData(filename, token);
        foreach (var t in dat.contents.signature.texts)
        {
            if (t.EndsWith("事件"))
            {
                return t;
            }
        }
        return "";
    }
    public async UniTask<HanreiTokenizedData> GetTokenizedData(string filename, CancellationToken token)
    {
        if (!fileNames.Contains(filename)) return null;
        var path = Application.dataPath + "/" + dataFilePath + "/" + filename + "__tokenized.json";
        using (var reader = new System.IO.StreamReader(path, System.Text.Encoding.UTF8))
        {
            string allLines = await reader.ReadToEndAsync();
            token.ThrowIfCancellationRequested();
            return JsonUtility.FromJson<HanreiTokenizedData>(allLines);
        }
    }
}
