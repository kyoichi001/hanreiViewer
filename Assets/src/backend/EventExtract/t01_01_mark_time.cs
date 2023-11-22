
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using InputData = HanreiTokenizer.OutputData;
using OutputData = HanreiTokenizer.OutputData;

[System.Serializable]
public class Time
{
    public int year;
    public int month;
    public int day;
    public Time() { }
    public Time(int year, int month, int day)
    {
        this.year = year;
        this.month = month;
        this.day = day;
    }
}

public class t01_01_mark_time
{

    Time ExtractPointTime(TimeRule.Rule rule, string tango, Time befTime = null)
    {
        var res = new Time();
        var a = Regex.Match(rule.regex, tango);
        if (!a.Success) return null;
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
            {
                res.year += 1;
            }
            else
            {
                res.year += int.Parse(year_str);
            }
        }
        if (month_str != null)
        {
            res.month = int.Parse(month_str);
        }
        if (day_str != null)
        {
            res.day = int.Parse(day_str);
        }
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
        return null;
    }

    public void Convert(InputData data, string timeRulePath)
    {
        using (var reader = new StreamReader(timeRulePath))
        {
            string datastr = reader.ReadToEnd();
            var ruleDat = JsonUtility.FromJson<TimeRule>(datastr);
        }
    }
}