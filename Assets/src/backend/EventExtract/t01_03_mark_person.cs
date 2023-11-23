using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using InputData = t01_02_mark_rentaishi.OutputData;
public class t01_03_mark_person
{
    [System.Serializable]
    public class OutputData
    {
        [System.Serializable]
        public class TextData
        {
            [System.Serializable]
            public class Bunsetsu
            {
                [System.Serializable]
                public class Token
                {
                    public int id;
                    public string text;
                    public string tag;
                    public string[] Tags() => tag.Split("-");
                }
                [System.Serializable]
                public class Person
                {
                    public string content;
                }
                public List<Token> tokens = new();
                public string text;
                public int id;
                public int to;
                public List<t01_01_mark_time.TimeValue> times = new();
                public bool is_rentaishi;
                public Person person;
            }
            public int text_id;
            public string text;
            public List<Bunsetsu> bunsetsu = new();
        }
        public List<TextData> datas = new();
        public OutputData(InputData data)
        {
            foreach (var i in data.datas)
            {
                var dat = new TextData
                {
                    text_id = i.text_id,
                    text = i.text
                };
                foreach (var b in i.bunsetsu)
                {
                    var bst = new TextData.Bunsetsu
                    {
                        id = b.id,
                        to = b.to,
                        text = b.text
                    };
                    foreach (var t in b.tokens)
                    {
                        bst.tokens.Add(new TextData.Bunsetsu.Token
                        {
                            id = t.id,
                            text = t.text,
                            tag = t.tag
                        });
                    }
                    dat.bunsetsu.Add(bst);
                }
                datas.Add(dat);
            }
        }
    }
    bool MatchRule(OutputData.TextData.Bunsetsu.Token token, PersonRule rule)
    {
        switch (rule.types)
        {
            case "c":
                return token.text == rule.rule.c;
            case "type":
                var tags = token.Tags();
                return tags.Contains(rule.rule.c);
            case "regex":
                return Regex.Match(token.text, "^" + rule.rule.c + "$").Success;
        }
        return false;
    }
    OutputData.TextData.Bunsetsu.Person MarkPerson(List<PersonRule> rules, OutputData.TextData.Bunsetsu bunsetsu)
    {
        var bnst = Utility.DeepClone(bunsetsu);
        var flg = false;
        var content = "";
        foreach (var tango in bunsetsu.tokens)
        {
            var tgs = tango.Tags();
            //if "名詞" not in tgs and  "助詞" not in tgs:return bunsetsu
            //if "名詞" in tgs and "読点" not in tgs:content=tango.content
            //if "助詞" in tgs:
            //    flg = tango.content in ["は", "が", "も"]
            //    if not flg:return bunsetsu #それ以外の助詞が入っていたら排除
            //if content!="" and flg:break
            foreach (var rule in rules)
            {
                if (MatchRule(tango, rule))
                {
                    return new OutputData.TextData.Bunsetsu.Person
                    {
                        content = tango.text
                    };
                }
            }
        }
        if (flg)
        {
            return new OutputData.TextData.Bunsetsu.Person
            {
                content = content
            };
        }
        return null;
    }
    public async UniTask<OutputData> Convert(InputData data, string personRulePath, CancellationToken token)
    {
        var res = new OutputData(data);
        using (var reader = new System.IO.StreamReader(personRulePath, System.Text.Encoding.UTF8))
        {
            string allLines = await reader.ReadToEndAsync();
            token.ThrowIfCancellationRequested();
            var rules = JsonUtility.FromJson<PersonRuleFile>(allLines);
            rules = PersonRuleFile.Convert(rules);
            foreach (var content in res.datas)
            {
                if (content.text == "") continue;
                foreach (var bunsetsu in content.bunsetsu)
                {
                    bunsetsu.person = MarkPerson(rules.rules, bunsetsu);
                }
            }
        }
        return res;
    }
}