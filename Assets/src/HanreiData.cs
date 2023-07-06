using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Entity
{
    public int id;
    public string text;
    public string label;
    public string start;
    public string end;
}

[System.Serializable]
public class Token
{
    public int id;
    public string text;
    public string tag;
    public int ent;
}

[System.Serializable]
public class Bunsetsu
{
    public int id;
    public string text;
    public List<Token> tokens;
    public List<Entity> ents;
}

[System.Serializable]
public class SectionText
{
    public string raw_text;
    public List<Bunsetsu> bunsetu;
}

[System.Serializable]
public class Section
{
    public string type;
    public string header;
    public string header_text;
    public int indent;
    public List<SectionText> texts=new List<SectionText>();
}

[System.Serializable]
public class Signature
{
    public string header_text;
    public  List<string> texts =new List<string>();
}

[System.Serializable]
public class Judgement
{
    public string header_text;
    public  List<string> texts = new List<string>();
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
public class HanreiContent
{
    public Signature signature;
    public Judgement judgement;
    public MainText main_text;
    public FactReason fact_reason;
}
[System.Serializable]
public class HanreiData
{
    public string filename;
    public HanreiContent contents;
}
