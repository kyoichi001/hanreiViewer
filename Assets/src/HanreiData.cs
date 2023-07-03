using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Section
{
    public string type;
    public string header;
    public string header_text;
    public string text;
    public int indent;
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
