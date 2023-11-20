using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class t04_IgnoreHeaderText
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
            public string text;
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

    public OutputData Convert(t03_SplitSection.OutputData data)
    {
        var res = new OutputData();
        res.signature.header_text = data.signature.header_text;
        res.signature.contents = data.signature.contents;
        res.judgement.header_text = data.judgement.header_text;
        res.judgement.contents = data.judgement.contents;

        res.mainText.header_text = data.mainText.header_text;
        foreach (var content in data.mainText.sections)
        {
            var s = new OutputData.Section
            {
                type = content.type,
                header = content.header,
                text = string.Join("", content.texts),
                indent = content.indent
            };
            res.mainText.sections.Add(s);
        }
        res.factReason.header_text = data.factReason.header_text;
        foreach (var content in data.factReason.sections)
        {
            var s = new OutputData.Section
            {
                type = content.type,
                header = content.header,
                text = string.Join("", content.texts),
                indent = content.indent
            };
            res.factReason.sections.Add(s);
        }
        return res;
    }

}
