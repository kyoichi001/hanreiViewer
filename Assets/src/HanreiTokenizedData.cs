using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HanreiTokenizedData
{
    [System.Serializable]
    public class HanreiTextTokenData
    {
        [System.Serializable]
        public class HanreiTokenizedBunsetsuData
        {
            [System.Serializable]
            public class HanreiTokenData
            {
                public int id;
                public string text;
                public string tag;
            }

            public int id;
            public string text;
            public int to;
            public List<HanreiTokenData> tokens;
        }
        [System.Serializable]
        public class HanreiEventsData
        {
            [System.Serializable]
            public class HanreiEventTimeData
            {
                [System.Serializable]
                public class HanreiEventTimeValueData
                {
                    public string text;
                    public int value;
                }

                public int event_time_id;
                public List<int> bnst_ids;
                public HanreiEventTimeValueData point;
                public HanreiEventTimeValueData begin;
                public HanreiEventTimeValueData end;
            }

            public string person;
            public string acts;
            public string claim_state;
            public int issue_num;
            public HanreiEventTimeData time;
        }

        public int text_id;
        public string text;
        public string claim_state;
        public int issue_num;
        public List<HanreiTokenizedBunsetsuData> bunsetsu;
        public List<HanreiEventsData> events;
    }

    public List<HanreiTextTokenData> datas;
}
