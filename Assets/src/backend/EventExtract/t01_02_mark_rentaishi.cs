using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using InputData = t01_01_mark_time.OutputData;

public class t01_02_mark_rentaishi
{
    [System.Serializable]
    public class OutputData
    {
        [System.Serializable]
        public class TextData
        {
            [System.Serializable]
            public class Bunsetsu
            {
                [System.Serializable]
                public class Token
                {
                    public int id;
                    public string text;
                    public string tag;
                    public string[] Tags() => tag.Split("-");
                }
                public List<Token> tokens = new();
                public string text;
                public int id;
                public int to;
                public List<t01_01_mark_time.TimeValue> times = new();
                public bool is_rentaishi;
            }
            public int text_id;
            public string text;
            public List<Bunsetsu> bunsetsu = new();
        }
        public List<TextData> datas = new();
        public OutputData(InputData data)
        {
            foreach (var i in data.datas)
            {
                var dat = new TextData
                {
                    text_id = i.text_id,
                    text = i.text
                };
                foreach (var b in i.bunsetsu)
                {
                    var bst = new TextData.Bunsetsu
                    {
                        id = b.id,
                        to = b.to,
                        text = b.text
                    };
                    foreach (var t in b.tokens)
                    {
                        bst.tokens.Add(new TextData.Bunsetsu.Token
                        {
                            id = t.id,
                            text = t.text,
                            tag = t.tag
                        });
                    }
                    dat.bunsetsu.Add(bst);
                }
                datas.Add(dat);
            }
        }
    }
    bool IsMeishi(OutputData.TextData.Bunsetsu bst)
    {
        foreach (var tango in bst.tokens)
        {
            var tgs = tango.Tags();
            if (tgs.Contains("動詞") || tgs.Contains("形容詞") || tgs.Contains("副詞")) return false;
        }
        return true;
    }
    bool IsRentai(OutputData.TextData.Bunsetsu bst)
    {
        foreach (var tango in bst.tokens)
        {
            var tgs = tango.Tags();
            if (tgs.Contains("連体詞")) return true;
        }
        return false;
    }
    void CheckRentaishi_(int root, Graph g, List<bool> flagList, List<OutputData.TextData.Bunsetsu> bsts)
    {
        if (g == null) return;
        foreach (var child in g.g[root])
        {
            if (flagList[root] || IsRentai(bsts[child]) || IsMeishi(bsts[root]))
            {
                flagList[child] = true;
            }
            CheckRentaishi_(child, g, flagList, bsts);
        }
    }
    List<bool> CheckRentaishi(List<OutputData.TextData.Bunsetsu> bsts)
    {
        var flagList = new List<bool>(bsts.Count);
        var graphList = new List<List<int>>(bsts.Count + 1);
        foreach (var bst in bsts)
        {
            if (bst.to == -1) continue;
            graphList[bst.to].Add(bst.id);
        }
        var g = new Graph(graphList);
        CheckRentaishi_(bsts[^1].id, g, flagList, bsts);
        return flagList;
    }
    public OutputData Convert(InputData data)
    {
        var res = new OutputData(data);
        foreach (var d in res.datas)
        {
            var b = CheckRentaishi(d.bunsetsu);
            foreach (var bst in d.bunsetsu)
            {
                bst.is_rentaishi = b[bst.id];
            }
        }
        return res;
    }
}