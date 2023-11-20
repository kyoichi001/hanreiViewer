using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class t06_AddExtention
{
    [System.Serializable]
    public class OutputData
    {
        [System.Serializable]
        public class Signature
        {
            public string header_text;
            public List<string> contents = new List<string>();
        }
        [System.Serializable]
        public class Judgement
        {
            public string header_text;
            public List<string> contents = new List<string>();
        }
        [System.Serializable]
        public class Section
        {
            [System.Serializable]
            public class Text
            {
                public int text_id;
                public string text;
                public string raw_text;
            }
            public string type;
            public string header;
            public List<Text> texts = new List<Text>();
            public int indent;
            public int issueNum;
            public string claimState;
            public List<SelifData> selifs = new List<SelifData>();
            public List<BlacketsData> blackets = new List<BlacketsData>();
        }
        [System.Serializable]
        public class MainText
        {
            public string header_text;
            public List<Section> sections = new List<Section>();
        }
        [System.Serializable]
        public class FactReason
        {
            public string header_text;
            public List<Section> sections = new List<Section>();
        }
        [System.Serializable]
        public class SelifData
        {
            public int targetSelif;
            public string content;
            public int targetTextID;
            public int textID;
        }
        [System.Serializable]
        public class BlacketsData
        {
            public int position;
            public string content;
            public int targetTextID;
            public int textID;
        }
        public Signature signature = new Signature();
        public Judgement judgement = new Judgement();
        public MainText mainText = new MainText();
        public FactReason factReason = new FactReason();

    }

    string sanitize(string text)
    {
        var d = new List<string>{
            "１","1","２","2","３","3","４","4","５","5","６","6","７","7","８","8","９","9","０","0"
        };
        for (int i = 0; i < 10; i++)
        {
            text = text.Replace(d[i * 2], d[i * 2 + 1]);
        }
        return text;
    }
    //header_textは実装していないため、テキストのはやくに争点nが出現したものをそのセクションでの争点とする
    int? hasIssueExpression(string text)
    {
        var t = sanitize(text);
        var a = Regex.Matches(t, @"争点(?<num>\d+)");
        foreach (Match m in a)
        {
            if (m.Success)
            {
                var n = m.Groups["num"].Value;
                Debug.Log($"issue num extracted {n}");
                return int.Parse(n);
            }
        }
        return null;
    }
    string hasClaimExpression(string text, int indent)
    {
        //var genkokuRegex="(\\(|【|〔)原告.*の主張(\\)|】|〕)";
        //var hikokuRegex="(\\(|【|〔)被告.*の主張(\\)|】|〕)";
        var i = text.IndexOf("主張");
        if (i != -1)
        {
            var t = text.Substring(0, i);
            if (t.Contains("原告")) return "genkoku";//原告の主張とか
            if (t.Contains("被告")) return "hikoku";
        }
        if (text.Contains("判断") && indent == 1) return "saibanjo";
        return null;
    }
    string hasJudgeExpression(string text, int indent)
    {
        if (text.Contains("裁判所の判断") && indent == 1) return "saibanjo";
        return null;
    }

    OutputData.Section ConvertSection(t05_SplitSentence.OutputData.Section section)
    {
        var res = new OutputData.Section
        {
            type = section.type,
            header = section.header,
            indent = section.indent,
        };
        foreach (var text in section.texts)
        {
            res.texts.Add(new OutputData.Section.Text
            {
                text_id = text.text_id,
                text = text.text,
                raw_text = text.raw_text
            });
        }
        foreach (var selif in section.selifs)
        {
            res.selifs.Add(new OutputData.SelifData
            {
                targetSelif = selif.targetSelif,
                content = selif.content,
                targetTextID = selif.targetTextID,
                textID = selif.textID
            });
        }
        foreach (var blacket in section.blackets)
        {
            res.blackets.Add(new OutputData.BlacketsData
            {
                position = blacket.position,
                content = blacket.content,
                targetTextID = blacket.targetTextID,
                textID = blacket.textID
            });
        }
        return res;
    }

    public OutputData Convert(t05_SplitSentence.OutputData data)
    {
        var res = new OutputData();
        res.signature.header_text = data.signature.header_text;
        res.signature.contents = data.signature.contents;
        res.judgement.header_text = data.judgement.header_text;
        res.judgement.contents = data.judgement.contents;
        res.mainText.header_text = data.mainText.header_text;
        foreach (var section in data.mainText.sections)
        {
            res.mainText.sections.Add(ConvertSection(section));
        }
        res.factReason.header_text = data.factReason.header_text;

        int? issueIndent = null;
        int? befIssueNum = null;
        int? claimIndent = null;
        string befClaimState = null;
        foreach (var t in data.factReason.sections)
        {
            if (t.texts.Count == 0) continue;
            res.factReason.sections.Add(ConvertSection(t));
            var indent = t.indent;
            var header = t.header;
            var issueNum = hasIssueExpression(t.texts[0].raw_text);
            string claimState = null;
            var claimState1 = hasClaimExpression(t.header, indent);
            var judgeState = hasJudgeExpression(t.texts[0].raw_text, indent);
            if (issueNum != null)
            {
                issueIndent = indent;
                befIssueNum = issueNum;
            }
            if (claimState1 != null)
            {
                claimIndent = indent;
                claimState = claimState1;
                befClaimState = claimState;
            }
            if (judgeState != null)
            {
                claimIndent = indent;
                claimState = judgeState;
                befClaimState = claimState;
            }
            if (issueNum == null && issueIndent != null)
            {
                if (indent > issueIndent)
                {
                    issueNum = befIssueNum;
                }
                else
                {
                    befIssueNum = null;
                    issueIndent = null;
                }
            }
            if (claimState == null && claimIndent != null)
            {
                if (indent > claimIndent)
                {
                    claimState = befClaimState;
                }
                else
                {
                    befClaimState = null;
                    claimIndent = null;
                }
            }
            if (issueNum != null)
            {
                res.factReason.sections[^1].issueNum = issueNum ?? -1;
            }
            if (claimState != null)
            {
                res.factReason.sections[^1].claimState = claimState ?? "";
            }
        }
        return res;
    }

}
