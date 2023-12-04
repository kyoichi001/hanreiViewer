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
    readonly Dictionary<string, List<HanreiTokenizedData.HanreiTextTokenData.HanreiEventsData>> eventDataCache = new();
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
        if (hanreiDataCache.ContainsKey(filename))
        {
            return hanreiDataCache[filename];
        }
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
        if (hanreiTokenizedDataCache.ContainsKey(filename))
        {
            return hanreiTokenizedDataCache[filename];
        }
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
        return dat.datas.Find((d) => d.text_id == textID);
    }
    public async UniTask<List<HanreiTokenizedData.HanreiTextTokenData>>
    GetTextPrev(string filename, int textID, int count, CancellationToken token)
    {
        var dat = await GetTokenizedData(filename, token);
        var index = dat.datas.FindIndex((d) => d.text_id == textID);
        if (index <= count - 1 || index == -1) return null;
        return dat.datas.GetRange(index - count, count);
    }
    public async UniTask<List<HanreiTokenizedData.HanreiTextTokenData>>
    GetTextNext(string filename, int textID, int count, CancellationToken token)
    {
        var dat = await GetTokenizedData(filename, token);
        var index = dat.datas.FindIndex((d) => d.text_id == textID);
        if (index >= dat.datas.Count - count || index == -1) return null;
        return dat.datas.GetRange(index + 1, count);
    }
    public async UniTask<Section>
    GetTextSection(string filename, int textID, CancellationToken token)
    {
        var dat = await GetTextData(filename, token);
        //Debug.Log("aaaaaaa:" + textID);
        foreach (var section in dat.contents.fact_reason.sections)
        {
            var index = section.texts.Find((t) => t.text_id == textID);
            if (index == null) continue;
            /*foreach (var t in section.texts)
               {
                   Debug.Log(t.text_id);
               }*/
            return section;
        }
        return null;
    }
    public async UniTask<Section>
    GetParentSection(string filename, Section section, CancellationToken token)
    {
        var dat = await GetTextData(filename, token);
        Section parent = null;
        foreach (var s in dat.contents.fact_reason.sections)
        {
            if (s.indent == section.indent - 1) parent = s;
            if (s.indent == section.indent)
            {
                if (s.header == section.header && s.type == section.type && s.texts.Equals(s.texts))
                {
                    return parent;
                }
            }
        }
        return null;
    }
    public async UniTask<List<Section>>
    GetParentsSection(string filename, Section section, CancellationToken token)
    {
        var dat = await GetTextData(filename, token);
        var res = new List<Section>();
        for (int i = 0; i < section.indent - 1; i++) res.Add(null);
        foreach (var s in dat.contents.fact_reason.sections)
        {
            if (s.indent < section.indent)
            {
                Debug.Log(s.indent + "," + section.indent + "," + res.Count);
                res[s.indent - 1] = s;
            }
            if (s.indent == section.indent)
            {
                if (s.header == section.header && s.type == section.type && s.texts.Count == section.texts.Count)
                {
                    return res;
                }
            }
        }
        Debug.Log("section not found");
        return null;
    }
    public async UniTask<HanreiTokenizedData.HanreiTextTokenData.HanreiEventsData>
    GetEvent(string filename, int eventID, CancellationToken token)
    {
        if (eventDataCache.ContainsKey(filename))
        {
            return eventDataCache[filename][eventID];
        }
        eventDataCache[filename] = new List<HanreiTokenizedData.HanreiTextTokenData.HanreiEventsData>();
        var dat = await GetTokenizedData(filename, token);
        foreach (var d in dat.datas)
        {
            foreach (var e in d.events)
            {
                eventDataCache[filename].Add(e);
            }
        }
        return eventDataCache[filename][eventID];
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