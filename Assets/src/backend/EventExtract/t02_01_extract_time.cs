using System.Collections.Generic;
using System.Linq;
using InputData = t01_04_time_group.OutputData;
public class t02_01_extract_time
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
                public List<TimeObj> times = new();
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
    [System.Serializable]
    public class TimeObj
    {
        [System.Serializable]
        public class TimeValue
        {
            public string text;
            public int value;
        }
        public int event_time_id;
        public List<int> bnst_ids = new();
        public TimeValue point = null;
        public TimeValue begin = null;
        public TimeValue end = null;
    }
    (TimeObj, bool) Bnst2Time(OutputData.TextData.Bunsetsu bunsetsu, int event_time_id)
    {
        var res = new TimeObj
        {
            event_time_id = event_time_id,
            bnst_ids = new List<int> { bunsetsu.id }
        };
        bunsetsu.event_time_id = event_time_id;
        var mode = "point";
        var hasValue = false;
        foreach (var time in Enumerable.Reverse(bunsetsu.times))
        {
            switch (time.type)
            {
                case "point":
                    switch (mode)
                    {
                        case "point":
                            res.point = new TimeObj.TimeValue
                            {
                                text = time.text,
                                value = time.value
                            };
                            break;
                        case "begin":
                            res.begin = new TimeObj.TimeValue
                            {
                                text = time.text + res.begin.text,
                                value = time.value
                            };
                            break;
                        case "end":
                            res.end = new TimeObj.TimeValue
                            {
                                text = time.text + res.end.text,
                                value = time.value
                            };
                            break;
                    }
                    hasValue = true;
                    mode = "point";
                    break;
                case "begin":
                    if (mode != "point") continue;
                    mode = "begin";
                    res.begin = new TimeObj.TimeValue { text = time.text, value = 0 };
                    break;
                case "end":
                    if (mode != "point") continue;
                    mode = "end";
                    res.end = new TimeObj.TimeValue { text = time.text, value = 0 };
                    break;
                case "other":
                    break;
            }
        }
        return (res, hasValue);
    }
    (TimeObj, bool) TimeGroup2Time(List<OutputData.TextData.Bunsetsu> bnsts, List<int> bnstIds, int event_time_id)
    {
        var res = new TimeObj
        {
            event_time_id = event_time_id,
            bnst_ids = bnstIds
        };
        var hasValue = false;
        foreach (var bnst in bnsts)
        {
            if (!bnstIds.Contains(bnst.id)) continue;
            var (timeObj, hasValue_) = Bnst2Time(bnst, event_time_id);
            bnst.event_time_id = event_time_id;
            if (hasValue_) hasValue = true;
            else continue;
            if (timeObj.begin != null)
            {
                res.begin = Utility.DeepClone(timeObj.begin);
            }
            else if (timeObj.end != null)
            {
                res.end = Utility.DeepClone(timeObj.end);
            }
            else if (timeObj.point != null)
            {
                res.point = Utility.DeepClone(timeObj.point);
            }
        }
        return (res, hasValue);
    }
    (List<TimeObj>, int) ExtractTime(OutputData.TextData data, int event_time_id)
    {
        var res = new List<TimeObj>();
        int index = data.bunsetsu.Count - 1;
        var visited = new List<bool>(data.bunsetsu.Count);
        while (index >= 0)
        {
            var bnst = data.bunsetsu[index];
            if (visited[index])
            {
                index--;
                continue;
            }
            if (!bnst.is_rentaishi && bnst.times.Count != 0)
            {
                if (bnst.time_group.Count != 0)
                {
                    var (time_obj, hasValue) = TimeGroup2Time(data.bunsetsu, bnst.time_group, event_time_id);
                    foreach (var j in bnst.time_group) visited[j] = true;
                    if (hasValue)
                    {
                        res.Add(time_obj);
                        event_time_id++;
                    }
                }
                else
                {
                    var (time_obj, hasValue) = Bnst2Time(bnst, event_time_id);
                    if (hasValue)
                    {
                        res.Add(time_obj);
                        event_time_id++;
                    }
                }
            }
            visited[index] = true;
            index--;
        }
        return (Enumerable.Reverse(res).ToList(), event_time_id);
    }
    public OutputData Convert(InputData data)
    {
        var res = new OutputData(data);
        int event_time_id = 0;
        foreach (var d in res.datas)
        {
            var (times, id) = ExtractTime(d, event_time_id);
            event_time_id = id;
            if (times.Count != 0)
            {
                d.event_ = new OutputData.TextData.Event
                {
                    text_id = d.text_id,
                    times = times
                };
            }
        }
        return res;
    }

}