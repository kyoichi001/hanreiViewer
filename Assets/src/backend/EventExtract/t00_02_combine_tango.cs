using System.Collections.Generic;
using System.Linq;


using InputData = HanreiTokenizer.OutputData;
using OutputData = HanreiTokenizer.OutputData;
public class t00_02_combine_tango
{
    bool IsMeishi(CaboChaRes.CaboChaToken token)
    {
        var tags = token.tag.Split("-");
        return !tags.Contains("句点") && !tags.Contains("読点") &&
        (tags.Contains("名詞") || tags.Contains("接尾辞") || tags.Contains("接頭辞") || tags.Contains("補助記号"));
    }
    List<CaboChaRes.CaboChaToken> CombineTangos(List<CaboChaRes.CaboChaToken> tangos)
    {
        var res = Utility.DeepClone(tangos);
        int index = 0;
        while (true)
        {
            if (index + 1 >= res.Count) break;
            if (IsMeishi(res[index]) && IsMeishi(res[index + 1]))
            {
                res[index] = new CaboChaRes.CaboChaToken
                {
                    text = res[index].text + res[index + 1].text,
                    tag = "名詞"
                };
                res.RemoveAt(index + 1);
            }
            else
            {
                index++;
            }
        }
        return res;
    }
    public OutputData Convert(InputData data)
    {
        var res = Utility.DeepClone(data);
        foreach (var d in res.datas)
        {
            foreach (var b in d.bunsetsu)
            {
                b.tokens = CombineTangos(b.tokens);
            }
        }
        return res;
    }

}