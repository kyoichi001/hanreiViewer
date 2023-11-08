using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeType
{
    point,
    begin_end,
    begin,
    end
}

[System.Serializable]
public class DurationTime
{
    public System.DateTime? begin;
    public System.DateTime? end;
    public TimeType timeType;

    public override string ToString()
    {
        return begin.ToString() + ":" + end.ToString();
    }
}
