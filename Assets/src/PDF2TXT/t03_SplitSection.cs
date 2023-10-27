using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class t03_SplitSection
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
            public string type;
            public string header;
            public List<string> texts = new List<string>();
            public int indent;
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

    List<OutputData.Section> DetectHeader(HeaderChecker headerChecker, List<t02_DetectHeader.OutputData.Section> sections)
    {
        var headerList = new HeaderList(headerChecker);
        var texts = new List<string>();

        var currentHeaderType = "";
        var currentHeader = "";
        var res = new List<OutputData.Section>();
        foreach (var section in sections)
        {
            var header = section.header;
            var t = section.texts;
            if (headerChecker.matchHeader(header))
            {
                var (headerType, headerText, txt) = headerChecker.GetHeaderType(header);
                Debug.Log($"match header {header} {headerType} {headerText}");
                var flag1 = headerList.IsNextHeader(headerType, headerText);
                var flag2 = headerChecker.isCollect(headerType, string.Join("", texts));
                if (flag1 && flag2)
                {
                    if (currentHeaderType != "")
                    {
                        Debug.Log($"add section {currentHeaderType} {currentHeader}");
                        var sectionObj = new OutputData.Section
                        {
                            type = currentHeaderType,
                            header = currentHeader,
                            texts = texts,
                            indent = headerList.CurrentIndent()
                        };
                        res.Add(sectionObj);
                    }
                    headerList.AddHeader(headerType, headerText);
                    currentHeader = headerText;
                    currentHeaderType = headerType;
                    texts = t;
                }
                else
                {
                    if (t.Count == 0)
                    {
                        texts.Add(header);
                    }
                    else
                    {
                        t[0] = header + t[0];
                        texts.AddRange(t);
                    }
                }
            }
            else
            {
                if (t.Count == 0)
                {
                    texts.Add(header);
                }
                else
                {
                    t[0] = header + t[0];
                    texts.AddRange(t);
                }
            }
        }
        var sectionObj1 = new OutputData.Section
        {
            type = currentHeaderType,
            header = currentHeader,
            texts = texts,
            indent = headerList.CurrentIndent()
        };
        res.Add(sectionObj1);
        return res;
    }

    public OutputData Convert(t02_DetectHeader.OutputData data, string headerRulePath)
    {
        var rules = HeaderRuleLoader.Load(headerRulePath);
        var headerChecker = new HeaderChecker(rules);
        var res = new OutputData();
        res.signature.header_text = data.signature.header_text;
        res.signature.contents = data.signature.contents;
        res.judgement.header_text = data.judgement.header_text;
        res.judgement.contents = data.judgement.contents;

        var main_text_sections = DetectHeader(headerChecker, data.mainText.sections);
        var fact_reason_sections = DetectHeader(headerChecker, data.factReason.sections);
        res.mainText.header_text = data.mainText.header_text;
        res.mainText.sections = main_text_sections;
        res.factReason.header_text = data.factReason.header_text;
        res.factReason.sections = fact_reason_sections;
        return res;
    }
}
