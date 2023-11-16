using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HanreiRepository : Singleton<HanreiRepository>
{
    readonly Dictionary<string, HanreiData> hanreiDataCache = new();
    readonly Dictionary<string, HanreiTokenizedData> hanreiTokenizedDataCache = new();
    public List<string> GetNames(string directory)//pathから __**.jsonのみ抜いたものがfilenameとして得られる
    {
        HashSet<string> s = new();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        foreach (var i in Directory.GetFiles(directory, "*.json"))
        {
            var c = i.Split("__")[0];
            s.Add(c);
        }
        return s.ToList();
    }
    public async UniTask<HanreiData> GetTextData(string filename, CancellationToken token)
    {
        var path = filename + "__text.json";
        using (var reader = new System.IO.StreamReader(path, System.Text.Encoding.UTF8))
        {
            string datastr = await reader.ReadToEndAsync();
            token.ThrowIfCancellationRequested();
            hanreiDataCache[filename] = JsonUtility.FromJson<HanreiData>(datastr);
            return hanreiDataCache[filename];
        }
    }
    public async UniTask<HanreiTokenizedData> GetTokenizedData(string filename, CancellationToken token)
    {
        var path = filename + "__tokenized.json";
        using (var reader = new System.IO.StreamReader(path, System.Text.Encoding.UTF8))
        {
            string allLines = await reader.ReadToEndAsync();
            token.ThrowIfCancellationRequested();
            hanreiTokenizedDataCache[filename] = JsonUtility.FromJson<HanreiTokenizedData>(allLines);
            return hanreiTokenizedDataCache[filename];
        }
    }

    public async UniTask<string> GetHanreiTitle(string filename, CancellationToken token)
    {
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
    public async UniTask<HanreiTokenizedData.HanreiTextTokenData>
    GetText(string filename, int textID, CancellationToken token)
    {
        var dat = await GetTokenizedData(filename, token);
        foreach (var j in dat.datas)
            if (textID == j.text_id) return j;
        return null;
    }
    public async UniTask<HanreiTokenizedData.HanreiTextTokenData.HanreiTokenizedBunsetsuData.HanreiTokenData>
    GetToken(string filename, int textID, int tokenID, CancellationToken token)
    {
        var dat = await GetText(filename, textID, token);
        foreach (var k in dat.bunsetsu)
        {
            foreach (var l in k.tokens)
                if (tokenID == l.id) return l;
        }
        return null;
    }

}