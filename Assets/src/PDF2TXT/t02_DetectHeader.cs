using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class t02_DetectHeader
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
            public string header;
            public List<string> texts = new List<string>();
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
        public Signature signature = new Signature();
        public Judgement judgement = new Judgement();
        public MainText mainText = new MainText();
        public FactReason factReason = new FactReason();
    }

    public OutputData Convert(t01_JustifySentence.OutputData data)
    {
        var res = new OutputData();

        var main_section_headers = new List<string> { "判決", "主文", "事実及び理由" };
        var rules = HeaderRuleLoader.Load("filepath");
        var current_phase = 0;
        var header_others = rules.FindAll((obj) => !obj.order);
        var raw_texts = new List<string>();
        var sections = new List<OutputData.Section>();
        var current_section = new OutputData.Section();
        foreach (var text in data.contents)
        {
            var t_ = text.Replace(" ", "").Replace("\t", "").Replace("　", "");
            if (main_section_headers.Contains(t_))
            {//主文、事実及び理由などの場合
                switch (current_phase)
                {
                    case 0:
                        res.signature.contents = raw_texts;
                        break;
                    case 1:
                        res.judgement.contents = raw_texts;
                        break;
                    case 2:
                        res.mainText.sections = sections;
                        current_section = new OutputData.Section();
                        break;
                }
                raw_texts.Clear();
                sections.Clear();
                current_phase++;
            }
            var header_flg = false;
            foreach (var h in header_others)
            {
                var re = new Regex(h.regex);
                if (Regex.IsMatch(t_, h.regex))
                {
                    if (current_section.header != "")
                    {
                        sections.Add(current_section);
                    }
                    current_section = new OutputData.Section
                    {
                        header = t_,
                        texts = new List<string>()
                    };
                    header_flg = true;
                    break;
                }
            }
            if (header_flg)
            {
                continue;
            }
            var a = text.Split(" ");
            if (a.Length >= 2)
            {
                sections.Add(current_section);
                current_section = new OutputData.Section
                {
                    header = a[0],
                    texts = new List<string> { string.Join("", a[1..]) }
                };
            }
            else
            {
                current_section.texts.Add(text);
            }
            raw_texts.Add(text);
        }
        sections.Add(current_section);
        res.factReason.sections = sections;

        return res;
    }
}
