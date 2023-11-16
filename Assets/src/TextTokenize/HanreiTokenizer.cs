using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HanreiTokenizer : MonoBehaviour
{
    [System.Serializable]
    public class OutputData
    {
        [System.Serializable]
        public class TextData
        {
            public int text_id;
            public string text;
            public List<CaboChaRes.CaboChaBunsetsu> bunsetsu = new List<CaboChaRes.CaboChaBunsetsu>();
        }
        public List<TextData> datas = new List<TextData>();
    }


    public async UniTask<OutputData> Tokenize(string filepath)
    {
        var res = new OutputData();
        CabochaWrapper.Instance.StartProcess();
        using (var reader = new StreamReader(filepath))
        {
            string datastr = await reader.ReadToEndAsync();
            var dat = JsonUtility.FromJson<t06_AddExtention.OutputData>(datastr);
            foreach (var d in dat.factReason.sections)
            {
                foreach (var t in d.texts)
                {
                    var r = await CabochaWrapper.Instance.CaboCha(t.text);
                    res.datas.Add(new OutputData.TextData
                    {
                        text_id = t.text_id,
                        text = t.text,
                    });
                    res.datas[^1].bunsetsu = new List<CaboChaRes.CaboChaBunsetsu>(r.bunsetsus);
                }
            }
        }
        CabochaWrapper.Instance.EndProcess();
        return res;
    }
}
