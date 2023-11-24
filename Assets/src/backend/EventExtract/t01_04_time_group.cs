using System.Collections.Generic;
using InputData = t01_03_mark_person.OutputData;
public class t01_04_time_group
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
                [System.Serializable]
                public class Person
                {
                    public string content;
                }
                public List<Token> tokens = new();
                public string text;
                public int id;
                public int to;
                public List<t01_01_mark_time.TimeValue> times = new();
                public bool is_rentaishi;
                public Person person;
                public List<int> time_group = new();
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

    bool HasValue(OutputData.TextData.Bunsetsu bst)
    {
        foreach (var t in bst.times)
        {
            if (t.type == "point") return true;
        }
        return false;
    }
    void AddTimeGroup_(int root, Graph g, List<List<int>> groupList, List<OutputData.TextData.Bunsetsu> bsts)
    {
        foreach (var child in g.g[root])
        {
            AddTimeGroup_(child, g, groupList, bsts);
            if (bsts[root].times.Count != 0 && bsts[child].times.Count != 0)
            {
                if (!HasValue(bsts[root]) && !HasValue(bsts[child])) continue;
                groupList[root].AddRange(groupList[child]);
            }
        }
    }
    void AddTimeGroup(List<OutputData.TextData.Bunsetsu> bsts)
    {
        var group_list = new List<List<int>>(bsts.Count);
        var li = new List<List<int>>(bsts.Count + 1);
        foreach (var bst in bsts)
        {
            if (bst.to != -1) li[bst.to].Add(bst.id);
        }
        var g = new Graph(li);
        AddTimeGroup_(bsts[^1].id, g, group_list, bsts);
        foreach (var bst in bsts)
        {
            if (group_list[bst.id].Count > 1)
            {
                bst.time_group = new List<int>(group_list[bst.id]);
            }
        }
    }
    public OutputData Convert(InputData data)
    {
        var res = new OutputData(data);
        foreach (var dat in res.datas)
        {
            AddTimeGroup(dat.bunsetsu);
        }
        return res;
    }
}