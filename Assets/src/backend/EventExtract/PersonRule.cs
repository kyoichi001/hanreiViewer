using System.Collections.Generic;

[System.Serializable]
public class PersonRule
{
    [System.Serializable]
    public class Rule
    {
        public string type;
        public string c;
    }
    public string name;
    public string types;
    public Rule rule;
    public static List<PersonRule> Convert(PersonRule rule)
    {
        var res = new List<PersonRule>();
        if (rule.rule.c.Contains("_ps"))
        {
            var str = new List<string> { "原告", "被告" };
            foreach (var i in str)
                res.Add(new PersonRule
                {
                    name = rule.name.Replace("_ps", i),
                    types = rule.types,
                    rule = new Rule
                    {
                        type = rule.rule.type,
                        c = rule.rule.c.Replace("_ps", i)
                    }
                });
        }
        else
        {
            res.Add(rule);
        }
        return res;
    }
}

[System.Serializable]
public class PersonRuleFile
{
    public List<PersonRule> rules = new();
    public static PersonRuleFile Convert(PersonRuleFile rule_)
    {
        var li = new List<PersonRule>();
        foreach (var rule in rule_.rules)
        {
            var l = PersonRule.Convert(rule);
            li.AddRange(l);
        }
        return new PersonRuleFile
        {
            rules = li
        };
    }
}