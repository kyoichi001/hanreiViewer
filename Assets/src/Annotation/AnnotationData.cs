using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TokenRelationType
{
    None
}
public enum TokenTagType
{
    None,
    Person,
    Time,
    Act,
    PersonN,
    TimeN,
    ActN
}


[System.Serializable]
public class TokenRelation
{
    public int textID;
    public int tokenID;
    public int targetID;
    public TokenRelationType type;
}
[System.Serializable]
public class TokenTag
{
    public int textID;
    public int tokenID;
    public TokenTagType type;
}

[System.Serializable]
public class AnotationData
{
    public string filename;
    public List<TokenRelation> annotations=new List<TokenRelation>();
    public List<TokenTag> tags=new List<TokenTag>();
}