using System.Collections.Generic;

[System.Serializable]
public class TimeRule
{
    [System.Serializable]
    public class Rule
    {
        public string regex;
        public string same;
    }
    public List<Rule> point = new();
    public List<Rule> begin = new();
    public List<Rule> end = new();
    public List<Rule> other = new();
}