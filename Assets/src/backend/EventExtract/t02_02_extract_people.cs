using System.Collections.Generic;
using System.Linq;
using InputData = t02_01_extract_time.OutputData;
public class t02_02_extract_people
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

    List<OutputData.TextData.Event.People> ExtractMainPeople(OutputData.TextData dat)
    {
        var res = new List<OutputData.TextData.Event.People>();
        foreach (var bnst in dat.bunsetsu)
        {
            if (bnst.is_rentaishi || bnst.person == null) continue;
            foreach (var tango in bnst.tokens)
            {
                var tgs = tango.Tags();
                if (!tgs.Contains("名詞") && !tgs.Contains("助詞")) continue;
                if ("はがも".Contains(tango.text) && tgs.Contains("助詞"))
                {
                    res.Add(new OutputData.TextData.Event.People
                    {
                        id = bnst.id,
                        person = bnst.person.content,
                        joshi = tango.text
                    });
                    break;
                }
            }
        }
        return res;
    }
    public OutputData Convert(InputData data)
    {
        var res = new OutputData(data);
        foreach (var content in res.datas)
        {
            var people = ExtractMainPeople(content);
            if (people.Count == 0) continue;
            if (content.event_ != null)
            {
                content.event_.people = people;
            }
            else
            {
                content.event_ = new OutputData.TextData.Event
                {
                    people = people
                };
            }
        }
        return res;
    }

}