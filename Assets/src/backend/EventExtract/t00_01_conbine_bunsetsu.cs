using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using InputData = HanreiTokenizer.OutputData;
using OutputData = HanreiTokenizer.OutputData;
public class t00_01_conbine_bunsetsu
{
    bool isMeishi(CaboChaRes.CaboChaBunsetsu bst)
    {
        foreach (var token in bst.tokens)
        {
            var tags = token.tag.Split("-");
            if (!(tags.Contains("名詞") || tags.Contains("補助記号")))
                return false;
        }
        return true;
    }
    List<CaboChaRes.CaboChaBunsetsu> MergeTree(List<CaboChaRes.CaboChaBunsetsu> bsts, int bst1, int bst2)
    {
        var res = Utility.DeepClone(bsts);
        var newTokens = new List<CaboChaRes.CaboChaToken>();
        newTokens.AddRange(bsts[bst1].tokens);
        newTokens.AddRange(bsts[bst2].tokens);
        res[bst1] = new CaboChaRes.CaboChaBunsetsu
        {
            id = bsts[bst1].id,
            to = bsts[bst2].to,
            text = bsts[bst1].text + bsts[bst2].text,
            tokens = newTokens
        };
        res.RemoveAt(bst2);
        foreach (var i in res)
        {
            if (i.id > res[bst1].id) i.id--;
            if (i.to > res[bst1].id) i.to--;
        }
        foreach (var i in res)
        {
            if (i.to == i.id)
                Akak.Debug.LogWarn("bst.to==bst.id " + i.id);
        }
        return res;
    }
    public OutputData Convert(InputData data)
    {
        var res = Utility.DeepClone(data);
        foreach (var i in res.datas)
        {
            var combineToNext = new List<bool>(i.bunsetsu.Count);
            for (int j = 0; j < i.bunsetsu.Count - 1; j++)
            {
                if (isMeishi(i.bunsetsu[j]) && i.bunsetsu[j].to == i.bunsetsu[j + 1].id)
                {
                    combineToNext[j] = true;
                }
            }
            int index = i.bunsetsu.Count - 1;
            while (true)
            {
                if (index < 0) break;
                if (combineToNext[index - 1])
                {
                    i.bunsetsu = MergeTree(i.bunsetsu, index - 1, index);
                    combineToNext.RemoveAt(index - 1);
                }
            }
        }
        return res;
    }
}
