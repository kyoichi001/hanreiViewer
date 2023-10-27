
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class Rule
{
    public string name;
    public List<string> data = new List<string>();
    public string regex;
    public string ignore_letters;
    public string ignore_letters_after;
    public bool order;
}

public class HeaderRuleLoader
{

    [System.Serializable]
    class RuleData
    {
        public List<Rule> rules = new List<Rule>();
    }
    public static List<Rule> Load(string filepath)
    {
        StreamReader reader = new StreamReader(filepath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        var data = JsonUtility.FromJson<RuleData>(datastr);
        return data.rules;
    }
}

public class HeaderRule
{
    public Rule data;
    public bool isHead(string str)
    {
        if (data.data == null || data.data.Count == 0) return false;
        return str == data.data[0];
    }
    //header_type, headerがデータ内に存在するものかどうか
    public bool isValid(string text)
    {
        if (data.data == null) return Regex.IsMatch(text, "^" + data.regex + "$");//もし順番を定義するdataが存在しない場合、順番は考慮せず、正規表現にマッチするかのみ考慮
        return data.data.Contains(text);
    }
    public bool match(string text)
    {
        return Regex.IsMatch(text, "^" + data.regex + "$");
    }
    public int? GetIndex(string text)
    {
        if (data.data == null)
        {
            return null;
        }
        return data.data.IndexOf(text);
    }
}
public class HeaderChecker
{
    public List<HeaderRule> rules = new List<HeaderRule>();
    public HeaderChecker(List<Rule> rules_)
    {
        foreach (var rule in rules_)
        {
            rules.Add(new HeaderRule
            {
                data = rule
            });
        }
    }
    public bool matchHeader(string text)
    {
        foreach (var rule in rules)
        {
            if (rule.match(text)) return true;
        }
        return false;
    }
    HeaderRule GetRule(string header_type)
    {
        return rules.Find((r) => r.data.name == header_type);
    }
    public bool isCollect(string header_type, string old_text)
    {
        var rule = GetRule(header_type);
        if (rule.data.ignore_letters == null || rule.data.ignore_letters == "" || old_text == "") return true;
        return !rule.data.ignore_letters.Contains(old_text[^1]);
    }
    public (string, string, string) GetHeaderType(string text)
    {
        foreach (var rule in rules)
        {
            var m = Regex.Match(text, "^" + rule.data.regex);
            if (m.Success)
            {
                return (rule.data.name, m.Value, text[(m.Index + m.Length)..]);
            }
        }
        return ("unknown", "", text);
    }
    public bool isHead(string header)
    {
        foreach (var rule in rules)
        {
            if (rule.isHead(header)) return true;
        }
        return false;
    }
    public bool isValid(string header_type, string header)
    {
        if (rules.FindIndex((r) => r.data.name == header_type) == -1) return false;
        var rule = GetRule(header_type);
        return rule.isValid(header);
    }
    public int? getIndex(string header_type, string header)
    {
        if (rules.FindIndex((r) => r.data.name == header_type) == -1) return null;
        var rule = GetRule(header_type);
        return rule.GetIndex(header);
    }
}

public class HeaderList
{
    HeaderChecker headerChecker;
    List<int> headerIndexes = new List<int>();
    List<string> headers = new List<string>();
    List<string> headerTypes = new List<string>();

    public int CurrentIndent()
    {
        return headerTypes.Count;
    }
    bool HasBefore(string headerType)
    {
        return headerTypes.Contains(headerType);
    }
    bool IgnoresOrder(string headerType)
    {
        return !headerChecker.rules.Find((r) => r.data.name == headerType).data.order;
    }
    public bool IsNextHeader(string headerType, string header)
    {
        if (headerTypes.Count == 0)
        {
            return headerChecker.isHead(header);
        }
        if (!headerChecker.isValid(headerType, header)) return false;
        var targetIndex = headerChecker.getIndex(headerType, header);
        if (headerTypes[^1] == headerType)
        {
            if (IgnoresOrder(headerType))
            {
                Debug.Log($"ignore order {headerType}");
                return true;
            }
            return targetIndex == headerIndexes[^1] + 1;
        }
        if (HasBefore(headerType))
        {
            if (IgnoresOrder(headerType)) return true;
            var index = headerTypes.IndexOf(headerType);
            return targetIndex == headerIndexes[index] + 1;
        }
        if (IgnoresOrder(headerType))
        {
            Debug.Log($"ignore order {headerType}");
            return true;
        }
        return headerChecker.isHead(header);
    }
    public void AddHeader(string headerType, string header)
    {
        if (headerTypes.Count == 0)
        {
            headerTypes.Add(headerType);
            headers.Add(header);
            headerIndexes.Add(0);
            return;
        }
        if (headerTypes[^1] == headerType)
        {
            headers[^1] = header;
            headerIndexes[^1]++;
            return;
        }
        if (HasBefore(headerType))
        {
            while (headerTypes[^1] != headerType)
            {
                headerTypes.RemoveAt(headerTypes.Count - 1);
                headers.RemoveAt(headers.Count - 1);
                headerIndexes.RemoveAt(headerIndexes.Count - 1);
            }
            headers[^1] = header;
            headerIndexes[^1]++;
            return;
        }
        headerTypes.Add(headerType);
        headers.Add(header);
        headerIndexes.Add(0);
    }
    public void Reset()
    {
        headers.Clear();
        headerIndexes.Clear();
        headerTypes.Clear();
    }
    public HeaderList(HeaderChecker headerChecker_)
    {
        headerChecker = headerChecker_;
    }
}