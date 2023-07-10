using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TokenRelationType
{
    None
}

[System.Serializable]
public class TokenRelation
{
    public int targetID;
    public TokenRelationType type;
}

[System.Serializable]
public class TokenAnnotation
{
    public int textID;
    public int tokenID;
    public List<TokenRelation> relations;
}

[System.Serializable]
public class AnotationData
{
    public string filename;
    public List<TokenAnnotation> annotations;


    void AddRelation(int textID,int tokenID,int targetID,TokenRelationType type)
    {

    }
    void RemoveRelation()
    {

    }
    void UpdateRelation()
    {

    }
}