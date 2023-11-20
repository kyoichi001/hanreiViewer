using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
            public Section()
            {
            }
            public Section(Section s)
            {
                header = s.header;
                texts = new List<string>(s.texts);
            }
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

    public OutputData Convert(t01_JustifySentence.OutputData data, string headerRulePath)
    {
        var res = new OutputData();

        var main_section_headers = new List<string> { "判決", "主文", "事実及び理由" };
        var rules = HeaderRuleLoader.Load(headerRulePath);
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
                        res.signature.header_text = "";
                        res.signature.contents = new List<string>(raw_texts);
                        break;
                    case 1:
                        res.judgement.header_text = t_;
                        res.judgement.contents = new List<string>(raw_texts);
                        break;
                    case 2:
                        sections.Add(current_section);
                        res.mainText.header_text = t_;
                        foreach (var section in sections)
                        {
                            if (section.header != "" && section.header != null)
                                res.mainText.sections.Add(new OutputData.Section(section));
                        }
                        break;
                }
                current_section = new OutputData.Section();
                raw_texts.Clear();
                sections.Clear();
                current_phase++;
                continue;
            }
            var header_flg = false;
            foreach (var h in header_others)
            {
                if (Regex.IsMatch(t_, "^" + h.regex + "$"))
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
            var a = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (a.Length >= 2)
            {
                //Debug.Log(string.Join(",", a));
                if (current_section.header != "")
                {
                    sections.Add(current_section);
                }
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
        res.factReason.header_text = "事実及び理由";
        foreach (var section in sections)
        {
            if (section.header != "" && section.header != null)
                res.factReason.sections.Add(new OutputData.Section(section));
        }

        return res;
    }
}
