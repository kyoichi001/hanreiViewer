using System.Collections.Generic;
using System.Linq;
using InputData = t02_03_mark_kakari.OutputData;
public class t02_04_extract_act
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
            [System.Serializable]
            public class Events
            {
                public string person;
                public string time;
                public int value;
                public string acts;
            }
            public Event event_ = null;
            public List<Events> events = new();
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
    bool IsShugo(OutputData.TextData.Bunsetsu bst)
    {
        if (bst.is_rentaishi || bst.person == null) return false;
        foreach (var tango in bst.tokens)
        {
            var tgs = tango.Tags();
            if ("はがも".Contains(tango.text) && tgs.Contains("助詞")) return true;
        }
        return false;
    }
    List<OutputData.TextData.Events> ExtractEvents(OutputData.TextData dat)
    {
        var res = new List<OutputData.TextData.Events>();
        var extracts = false;
        foreach (var bnst in dat.bunsetsu)
        {
            if (bnst.times == null) continue;
            foreach (var time in bnst.times)
            {
                if (time.type == "point")
                {
                    extracts = true;
                    break;
                }
            }
        }
        if (!extracts) return res;
        string extract_time = null;
        int? extract_time_value = null;
        var acts = "";
        var person = "";
        foreach (var bnst in dat.bunsetsu)
        {
            if (bnst.time_kakari || bnst.person_kakari) continue;
            if (bnst.is_rentaishi)
            {
                foreach (var tango in bnst.tokens) acts += tango.text;
                continue;
            }
            if (bnst.times != null)
            {
                string t = null;
                int? t_v = null;
                foreach (var time in bnst.times)
                {
                    if (time.type == "point" || time.type == "begin" || time.type == "end")
                    {
                        foreach (var t_obj in bnst.times)
                            t += t_obj.text;
                        t_v = time.value;
                        break;
                    }
                }
                if (t != null)
                {
                    if (extract_time != null)
                    {
                        res.Add(new OutputData.TextData.Events
                        {
                            person = person,
                            time = extract_time,
                            value = extract_time_value ?? 0,
                            acts = acts
                        });
                        acts = "";
                        person = "";
                    }
                    extract_time = t;
                    extract_time_value = t_v;
                }
                continue;
            }
            if (bnst.person != null)
            {
                if (!IsShugo(bnst))
                {
                    foreach (var tango in bnst.tokens)
                        acts += tango.text;
                    continue;
                }
                if (person != "")
                {
                    res.Add(new OutputData.TextData.Events
                    {
                        person = person,
                        time = extract_time,
                        value = extract_time_value ?? 0,
                        acts = acts
                    });
                    extract_time = null;
                    extract_time_value = null;
                    acts = "";
                }
                person = bnst.person.content;
                continue;
            }
            foreach (var tango in bnst.tokens)
                acts += tango.text;
        }
        if (extract_time != null && person != "")
        {
            res.Add(new OutputData.TextData.Events
            {
                person = person,
                time = extract_time,
                value = extract_time_value ?? 0,
                acts = acts
            });
        }
        return res;
    }
    public OutputData Convert(InputData data)
    {
        var res = new OutputData(data);
        foreach (var content in res.datas)
        {
            if (content.event_?.times == null) continue;
            var ddd = ExtractEvents(content);
            if (ddd.Count != 0) content.events = ddd;
            foreach (var e in ddd)
            {
                if (e.person == "" || (e.time ?? "") == "" || e.acts == "")
                    continue;
            }
        }
        return res;
    }
}