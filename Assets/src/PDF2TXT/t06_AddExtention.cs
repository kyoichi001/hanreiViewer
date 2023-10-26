using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class t06_AddExtention
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




}
