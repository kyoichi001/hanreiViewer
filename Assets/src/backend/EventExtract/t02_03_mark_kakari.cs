using System.Collections.Generic;
using InputData = t02_02_extract_people.OutputData;
public class t02_03_mark_kakari
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
                public Person person = null;
                public List<int> time_group = new();
                public int event_time_id;
                public bool time_kakari;
                public bool person_kakari;
            }
            public int text_id;
            public string text;
            public List<Bunsetsu> bunsetsu = new();
            public class Event
            {
                public int text_id;
                public List<t02_01_extract_time.TimeObj> times = new();
                [System.Serializable]
                public class People
                {
                    public int id;
                    public string person;
                    public string joshi;
                }
                public List<People> people = new();
            }
            public Event event_ = null;
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
    void CheckRentaishi_(int root, Graph g, List<bool> timeFlagList, List<bool> personFlagList, List<OutputData.TextData.Bunsetsu> bnsts)
    {
        if (personFlagList[root] || bnsts[root].person != null)
            foreach (var child in g.g[root])
                personFlagList[child] = true;
        if (timeFlagList[root] || bnsts[root].times != null && bnsts[root].times.Exists((t) => t.type == "point"))
            foreach (var child in g.g[root])
                timeFlagList[child] = true;
        foreach (var child in g.g[root])
        {
            CheckRentaishi_(root, g, timeFlagList, personFlagList, bnsts);
        }
    }
    (List<bool>, List<bool>) CheckRentaishi(List<OutputData.TextData.Bunsetsu> bnsts)
    {
        var timeFlagList = new List<bool>(bnsts.Count);
        var personFlagList = new List<bool>(bnsts.Count);
        var li = new List<List<int>>(bnsts.Count + 1);
        foreach (var bnst in bnsts)
            if (bnst.id != -1) li[bnst.to].Add(bnst.id);
        var g = new Graph(li);
        CheckRentaishi_(bnsts[^1].id, g, timeFlagList, personFlagList, bnsts);
        return (timeFlagList, personFlagList);
    }
    public OutputData Convert(InputData data)
    {
        var res = new OutputData(data);
        foreach (var content in res.datas)
        {
            var (tt, pp) = CheckRentaishi(content.bunsetsu);
            foreach (var bunsetsu in content.bunsetsu)
            {
                if (tt[bunsetsu.id]) bunsetsu.time_kakari = tt[bunsetsu.id];
                if (pp[bunsetsu.id]) bunsetsu.person_kakari = pp[bunsetsu.id];
            }
        }
        return res;
    }
}