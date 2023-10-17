using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

using DataType = HanreiTokenizedData.HanreiTextTokenData.HanreiEventsData;

public class UIFishbone : MonoBehaviour
{

    [Header("References")]
    [SerializeField] GameObject timeLine;
    [SerializeField] RectTransform timeStampsContainer;
    [SerializeField] GameObject timeStampPrefab;
    [SerializeField] RectTransform eventsList;
    [SerializeField] GameObject eventButtonPrefab;
    [SerializeField] PinchableScrollView scrollView;
    [SerializeField] Toggle genkokuToggle;
    [SerializeField] Toggle hikokuToggle;
    [SerializeField] Toggle jijitsuToggle;
    [Header("Debug")]
    [SerializeField, ReadOnly] List<DataType> data = new List<DataType>();

    UITimeline uiTimeLine;
    Dictionary<(string, System.DateTime, System.DateTime), TimeStampData> eventMap = new Dictionary<(string, System.DateTime, System.DateTime), TimeStampData>();
    Dictionary<(string, System.DateTime, System.DateTime), int> timeMap = new Dictionary<(string, System.DateTime, System.DateTime), int>();

    private void Awake()
    {
        uiTimeLine = timeLine.GetComponent<UITimeline>();
        FishboneViewManager.Instance.OnShowData.AddListener(async (path, data) =>
        {
            TimelineManager.Instance.Clear();
            ClearData();
            uiTimeLine.ClearUI();
            await UniTask.DelayFrame(1);//ObjectがDestroyされるまで1フレーム待つ必要がある
            foreach (var d in data.datas)
            {
                foreach (var e in d.events)
                {
                    AddData(e);
                }
            }
            uiTimeLine.GenerateUI();
            GenerateUI();
            // SetEventsPosition();
        });
        genkokuToggle.onValueChanged.AddListener((e) =>
        {
            FilterUI(genkokuToggle.isOn, hikokuToggle.isOn, jijitsuToggle.isOn);
        });
        hikokuToggle.onValueChanged.AddListener((e) =>
        {
            FilterUI(genkokuToggle.isOn, hikokuToggle.isOn, jijitsuToggle.isOn);
        });
        jijitsuToggle.onValueChanged.AddListener((e) =>
        {
            FilterUI(genkokuToggle.isOn, hikokuToggle.isOn, jijitsuToggle.isOn);
        });
    }

    public bool is_top(DataType data_)
    {
        return data_.person.Contains("原告");
    }

    public void AddData(DataType data_)
    {
        if (data_.person == "" || data_.acts == "") return;
        if (data_.time.begin.value == 0 && data_.time.end.value == 0 && data_.time.point.value == 0) return;

        var time_text = "";
        System.DateTime begin_value = System.DateTime.MinValue;
        System.DateTime end_value = System.DateTime.MaxValue;
        var is_range = false;
        if (data_.time.point != null && data_.time.point.value != 0)
        {
            time_text += data_.time.point.text;
            begin_value = Utility.Convert(data_.time.point.value);
        }
        if (data_.time.begin != null && data_.time.begin.value != 0)
        {
            time_text += data_.time.begin.text;
            begin_value = Utility.Convert(data_.time.begin.value);
            is_range = true;
        }
        if (data_.time.end != null && data_.time.end.value != 0)
        {
            time_text += data_.time.end.text;
            end_value = Utility.Convert(data_.time.end.value);
            is_range = true;
        }

        data.Add(data_);
        var map_key = (data_.person, begin_value, end_value);
        if (eventMap.ContainsKey(map_key))
        {
            eventMap[(data_.person, begin_value, end_value)].acts.Add(data_.acts);
        }
        else
        {
            int time_id;
            var is_top = this.is_top(data_);
            if (is_range)
            {
                System.DateTime? b = null;
                if (begin_value != System.DateTime.MinValue) b = begin_value;
                System.DateTime? e = null;
                if (end_value != System.DateTime.MaxValue) e = end_value;
                time_id = TimelineManager.Instance.AddTime(b, e, time_text, is_top);
            }
            else
            {
                time_id = TimelineManager.Instance.AddTime(begin_value, time_text, is_top);
            }
            timeMap[map_key] = time_id;
            var dat = new TimeStampData
            {
                person = data_.person,
                time_node = null,
                acts = new List<string> { data_.acts },
                is_top = is_top,
                claim_state = data_.claim_state,
                issue_num = data_.issue_num,
            };
            eventMap[map_key] = dat;
            var eventButtonScr = Instantiate(eventButtonPrefab, eventsList).GetComponent<UIEventButton>();
            eventButtonScr.Init(time_id.ToString(), data_.person, time_text);
            eventButtonScr.GetComponent<Button>().onClick.AddListener(() =>
            {
                scrollView.SetViewCenter(dat.time_node);
            });
        }
    }

    void GenerateUI()
    {
        foreach (var i in eventMap)
        {
            var time_id = timeMap[i.Key];
            var obj = uiTimeLine.GetTimeTransform(time_id).gameObject;
            Debug.Log($"GenerateUI : {i.Key} {time_id} {obj.name}:{obj.transform.position}");
            i.Value.time_node = obj.transform as RectTransform;
            Debug.Log("debug A");
            var timeStampObj = Instantiate(timeStampPrefab, timeStampsContainer).GetComponent<UITimeStamp>();
            Debug.Log($"timestamp data {i.Value.time_node.position}");
            timeStampObj.SetData(i.Value);
            timeStampObj.SetPosition();
        }
    }
    void ClearData()
    {
        data.Clear();
        eventMap.Clear();
        timeMap.Clear();
        //uiTimeLine.ClearUI();
        foreach (Transform child in timeStampsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void FilterUI(bool genkoku, bool hikoku, bool jijitsu)
    {
        foreach (Transform i in timeStampsContainer)
        {
            var scr = i.GetComponent<UITimeStamp>();
            if (scr.matchFilter(genkoku, hikoku, jijitsu))
            {
                scr.Activate();
            }
            else
            {
                scr.Deactivate();
            }
        }
    }



}
