using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class t01_JustifySentence
{
    [System.Serializable]
    public class InputData
    {
        [System.Serializable]
        public class Header
        {
            public int page_count;
        }
        [System.Serializable]
        public class Page
        {
            [System.Serializable]
            public class Content
            {
                public string text;
                public float x;
                public float y;
            }
            public int page;
            public List<Content> contents = new List<Content>();
        }
        public Header header = new Header();
        public List<Page> pages = new List<Page>();

    }
    [System.Serializable]
    public class OutputData
    {
        public List<string> contents = new List<string>();
    }

    public OutputData Convert(InputData data)
    {
        OutputData res = new OutputData();
        foreach (var page in data.pages)
        {
            page.contents.Sort((a, b) =>
            {
                if (a.y != b.y)
                    return -a.y.CompareTo(b.y);
                return a.x.CompareTo(b.x);
            });
            var i = 1;
            while (i < page.contents.Count)
            {
                if (page.contents[i - 1].y == page.contents[i].y)
                {
                    page.contents[i - 1].text += page.contents[i].text;
                    page.contents.RemoveAt(i);
                    continue;
                }
                i++;
            }
            foreach (var c in page.contents)
            {
                c.text = c.text.Trim().Replace("\n", "").Replace("\t", "").Replace("（", "(").Replace("）", ")");
                if (c.text != "" && !Regex.IsMatch(c.text, @"^[-ー \d]+$"))
                    res.contents.Add(c.text);
            }
        }
        return res;
    }

}
