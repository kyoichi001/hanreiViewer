using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HanreiEvent
{
    public string person;
    public string time;
    public int value;
    public string acts;
}
public class EventData
{
    public List<HanreiEvent> events=new List<HanreiEvent>();
}

[System.Serializable]
public class EventFileData
{
    public string filename;
    public EventData data;
}