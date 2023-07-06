using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EntityType
{
    None,
    Date
}

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
    public List<Token> tokens=new List<Token>();

}

[System.Serializable]
public class SectionText
{
    public string raw_text;
    public List<Bunsetsu> bunsetu = new List<Bunsetsu>();
    public List<Entity> ents = new List<Entity>();

    public EntityType GetTokenEntity(int tokenId)
    {
        foreach (var b in bunsetu)
        {
            foreach (var token in b.tokens)
            {
                if (token.id != tokenId) continue;
                foreach (var ent in ents)
                {
                    if (ent.id != token.ent) continue;
                    if (ent.label == "Date") return EntityType.Date;
                    else return EntityType.None;
                }
            }
        }
        return EntityType.None;
    }
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
