using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class t05_SplitSentence
{
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
        public class SelifData
        {
            public int targetSelif;
            public string content;
            public int targetTextID;
            public int textID;
        }
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

    bool isAllZero(Dictionary<char, int> table, char ignoreKey = '\0')
    {
        foreach (var i in table)
        {
            if ((ignoreKey != '\0' || i.Key != ignoreKey) && table[i.Key] != 0)
                return false;
        }
        return true;
    }
    List<string> ExtractKakko(string text)
    {
        var res = new List<string>();
        var resWOKakko = "";
        var resKakko = "";
        var pairKakko = new Dictionary<char, char>();
        pairKakko['('] = ')';
        pairKakko[')'] = '(';
        pairKakko['「'] = '」';
        pairKakko['」'] = '「';
        var kakkoCount = new Dictionary<char, int>();
        kakkoCount['('] = 0;
        kakkoCount['「'] = 0;
        var targetKakko = '\0';
        if (text == "")
        {
            res.Add("");
            return res;
        }
        if (text[0] == '(' || text[0] == '「')
        {
            targetKakko = text[0];
            kakkoCount[text[0]]++;
        }
        if (kakkoCount['('] >= 1 || kakkoCount['「'] >= 1)
        {
            resKakko += text[0];
        }
        else
        {
            resWOKakko += text[0];
        }
        foreach (var t in text)
        {
            if (t == ')' || t == '」')
            {
                kakkoCount[pairKakko[t]]--;
                if (kakkoCount[pairKakko[t]] == 0 && pairKakko[t] == targetKakko)
                {
                    res.Add(resKakko + t);
                    resKakko = "";
                    kakkoCount = new Dictionary<char, int>();
                    kakkoCount['('] = 0;
                    kakkoCount['「'] = 0;
                    targetKakko = '\0';
                    continue;
                }
            }
            if (t == '(' || t == '「')
            {
                kakkoCount[t]++;
                if (isAllZero(kakkoCount, t))
                {
                    targetKakko = t;
                    res.Add(resWOKakko);
                    resWOKakko = "";
                }
            }
            if (targetKakko != '\0')
            {
                resKakko += t;
            }
            else
            {
                resWOKakko += t;
            }
        }
        if (targetKakko != '\0')
        {
            res.Add(resKakko);
        }
        else
        {
            res.Add(resWOKakko);
        }
        return res;
    }
    List<string> SplitText(string text)
    {
        var a = Regex.Replace(text, "。([^」\\)〕])", "。$\\1");
        var d = a.Split("$");
        if (d.Length == 0) return new List<string> { text };
        return new List<string>(d);
    }
    List<string> SplitTexts(string text)
    {
        var texts = ExtractKakko(text);
        var texts2 = new List<string>();
        foreach (var t in texts)
        {
            if (t == "") continue;
            if (t[0] == '(' || t[0] == '「')
            {
                texts2.Add(t);
                continue;
            }
            foreach (var i in SplitText(t))
            {
                if (i == "") continue;
                texts2.Add(i);
            }
        }
        return texts2;
    }
    public class TextData
    {
        public string text;
        public List<OutputData.SelifData> selifs = new List<OutputData.SelifData>();
        public List<OutputData.BlacketsData> blackets = new List<OutputData.BlacketsData>();
        public string raw_text;
    }
    TextData TextToData(List<string> texts)
    {
        var res = new TextData();
        var selifs = new List<OutputData.SelifData>();
        var blackets = new List<OutputData.BlacketsData>();
        var text = "";
        var raw_text = "";
        var current_selif_count = 0;
        foreach (var t in texts)
        {
            if (t[0] == '「')
            {
                selifs.Add(new OutputData.SelifData
                {
                    targetSelif = current_selif_count,
                    content = t
                });
                current_selif_count++;
                text += "「セリフ」";
            }
            else if (t[0] == '(' || t[0] == '（')
            {
                blackets.Add(new OutputData.BlacketsData
                {
                    position = text.Length,
                    content = t,
                });
            }
            else
            {
                text += t;
            }
            raw_text += t;
        }
        res.text = text;
        res.selifs = selifs;
        res.blackets = blackets;
        res.raw_text = raw_text;
        return res;
    }
    (OutputData.Section, int) ConvertSection(t04_IgnoreHeaderText.OutputData.Section section, int firstTextID = 0)
    {
        var res = new OutputData.Section();
        var textInputs = new List<string>();
        var texts = new List<OutputData.Section.Text>();
        var selifs = new List<OutputData.SelifData>();
        var blackets = new List<OutputData.BlacketsData>();
        var targetTextID = 0;
        var textID = firstTextID;
        foreach (var i in SplitTexts(section.text))
        {
            if (i[^1] == '。' || i[^1] == '．')
            {
                textInputs.Add(i);
                var dat = TextToData(textInputs);
                texts.Add(new OutputData.Section.Text
                {
                    text_id = textID,
                    text = dat.text,
                    raw_text = string.Join("", textInputs)
                });
                targetTextID = textID;
                textID++;
                foreach (var selif in dat.selifs)
                {
                    selif.targetTextID = targetTextID;
                    selif.textID = textID;
                    textID++;
                }
                selifs.AddRange(dat.selifs);
                foreach (var blacket in dat.blackets)
                {
                    blacket.targetTextID = targetTextID;
                    blacket.textID = textID;
                    textID++;
                }
                blackets.AddRange(dat.blackets);
                textInputs.Clear();
            }
            else
            {
                textInputs.Add(i);
            }
        }
        var dat1 = TextToData(textInputs);
        if (dat1.text != "")
        {
            texts.Add(new OutputData.Section.Text
            {
                text_id = textID,
                text = dat1.text,
                raw_text = string.Join("", textInputs)
            });
            targetTextID = textID;
            textID++;
            foreach (var selif in dat1.selifs)
            {
                selif.targetTextID = targetTextID;
                selif.textID = textID;
                textID++;
            }
            selifs.AddRange(dat1.selifs);
            foreach (var blacket in dat1.blackets)
            {
                blacket.targetTextID = targetTextID;
                blacket.textID = textID;
                textID++;
            }
            blackets.AddRange(dat1.blackets);
        }
        res.type = section.type;
        res.header = section.header;
        res.indent = section.indent;
        if (texts.Count > 0) res.texts = texts;
        if (selifs.Count > 0) res.selifs = selifs;
        if (blackets.Count > 0) res.blackets = blackets;
        return (res, textID);
    }

    public OutputData Convert(t04_IgnoreHeaderText.OutputData data)
    {
        var res = new OutputData();
        res.signature.header_text = data.signature.header_text;
        res.signature.contents = data.signature.contents;
        res.judgement.header_text = data.judgement.header_text;
        res.judgement.contents = data.judgement.contents;

        var textID = 0;
        res.mainText.header_text = data.mainText.header_text;
        foreach (var section in data.mainText.sections)
        {
            var (s, a) = ConvertSection(section, textID);
            textID = a;
            res.mainText.sections.Add(s);
        }
        res.factReason.header_text = data.factReason.header_text;
        foreach (var section in data.factReason.sections)
        {
            var (s, a) = ConvertSection(section, textID);
            textID = a;
            res.factReason.sections.Add(s);
        }
        return res;
    }

}
