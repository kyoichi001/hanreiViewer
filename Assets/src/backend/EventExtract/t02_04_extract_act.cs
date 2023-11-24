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
    void ExtractEvents(OutputData.TextData dat)
    {

    }
}