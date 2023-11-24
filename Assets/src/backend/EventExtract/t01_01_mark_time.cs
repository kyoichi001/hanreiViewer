
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

using InputData = HanreiTokenizer.OutputData;


public class t01_01_mark_time
{
    [System.Serializable]
    public class Time
    {
        public int year;
        public int month;
        public int day;
        public Time() { }
        public Time(Time t)
        {
            this.year = t.year;
            this.month = t.month;
            this.day = t.day;
        }
        public Time(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }
        public int Value() => year * 10000 + month * 100 + day;
    }
    [System.Serializable]
    public class TimeValue
    {
        public string type;
        public string text;
        public int value;
    }

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
                public List<TimeValue> times = new();
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

    (Time, string) ExtractPointTime(TimeRule.Rule rule, string tango, Time befTime = null)
    {
        var res = new Time();
        var a = Regex.Match(rule.regex, tango);
        if (!a.Success) return (null, null);
        var gengo_str = a.Groups["gengo"].Value;
        var year_str = a.Groups["year"].Value;
        var month_str = a.Groups["month"].Value;
        var day_str = a.Groups["day"].Value;
        if (gengo_str != null)
        {
            res.year += gengo_str switch
            {
                "昭和" => 1925,
                "平成" => 1988,
                "令和" => 2018,
                _ => 0,
            };
        }
        if (year_str != null)
        {
            if (year_str == "元")
                res.year += 1;
            else
                res.year += int.Parse(year_str);
        }
        if (month_str != null)
            res.month = int.Parse(month_str);
        if (day_str != null)
            res.day = int.Parse(day_str);
        if (rule.same != null && befTime != null)
        {
            switch (rule.same)
            {
                case "year":
                    res.year = befTime.year;
                    break;
                case "month":
                    res.month = befTime.month;
                    break;
                case "day":
                    res.day = befTime.day;
                    break;
            }
        }
        return (res, a.Value);
    }
    string ExtractNotValueTime(TimeRule.Rule rule, string tango)
    {
        var a = Regex.Match(rule.regex, tango);
        if (!a.Success) return null;
        return a.Value;
    }
    (List<TimeValue>, Time) ExtractTimes(TimeRule rule, List<OutputData.TextData.Bunsetsu.Token> tokens, Time befTime = null)
    {
        var res = new List<TimeValue>();
        Time time = null;
        foreach (var tango in tokens)
        {
            foreach (var r in rule.point)
            {
                var (t, str) = ExtractPointTime(r, tango.text, befTime);
                if (t == null) continue;
                res.Add(new TimeValue
                {
                    type = "point",
                    text = str,
                    value = t.Value()
                });
                time = t;
            }
            foreach (var r in rule.begin)
            {
                var s = ExtractNotValueTime(r, tango.text);
                if (s == null) continue;
                res.Add(new TimeValue { type = "begin", text = s });
            }
            foreach (var r in rule.end)
            {
                var s = ExtractNotValueTime(r, tango.text);
                if (s == null) continue;
                res.Add(new TimeValue { type = "end", text = s });
            }
            foreach (var r in rule.other)
            {
                var s = ExtractNotValueTime(r, tango.text);
                if (s == null) continue;
                res.Add(new TimeValue { type = "other", text = s });
            }
        }
        return (res, time);
    }
    public OutputData Convert(InputData data, string timeRulePath)
    {
        var res = new OutputData(data);
        using (var reader = new StreamReader(timeRulePath))
        {
            string datastr = reader.ReadToEnd();
            var ruleDat = JsonUtility.FromJson<TimeRule>(datastr);
            Time befTime = null;
            foreach (var i in res.datas)
            {
                foreach (var b in i.bunsetsu)
                {
                    var (times, time) = ExtractTimes(ruleDat, b.tokens, befTime);
                    if (time != null) befTime = new Time(time);
                    if (times.Count != 0)
                    {
                        b.times = times;
                    }
                }
            }
        }
        return res;
    }
}