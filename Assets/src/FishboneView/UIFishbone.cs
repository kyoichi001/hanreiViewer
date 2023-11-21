using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

using DataType = HanreiTokenizedData.HanreiTextTokenData.HanreiEventsData;
using System.Threading;

public class UIFishbone : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] GameObject timeStampPrefab;
    [SerializeField] GameObject eventButtonPrefab;
    [SerializeField] GameObject modalPrefab;

    [Header("References")]
    [SerializeField] GameObject timeLine;
    [SerializeField] RectTransform timeStampsContainer;
    [SerializeField] RectTransform eventsList;
    [SerializeField] Toggle genkokuToggle;
    [SerializeField] Toggle hikokuToggle;
    [SerializeField] Toggle jijitsuToggle;
    [SerializeField] Slider timelineSlider;
    [Header("Debug")]
    [SerializeField, ReadOnly] List<DataType> data = new();

    UITimeline uiTimeLine;
    readonly Dictionary<(string, System.DateTime, System.DateTime), TimeStampData> eventMap = new();
    readonly Dictionary<(string, System.DateTime, System.DateTime), int> timeMap = new();
    List<UITimeStamp> timeStamps = new();
    private void Awake()
    {
        var token = this.GetCancellationTokenOnDestroy();
        uiTimeLine = timeLine.GetComponent<UITimeline>();
        FishboneViewManager.Instance.OnShowData.AddListener(async (path, data) =>
        {
            TimelineManager.Instance.Clear();
            ClearData();
            uiTimeLine.ClearUI();
            await UniTask.DelayFrame(1);//ObjectがDestroyされてから1フレーム待つ必要がある
            var event_id = 0;
            foreach (var d in data.datas)
            {
                foreach (var e in d.events)
                {
                    AddData(d.text_id, e, event_id);
                    event_id++;
                }
            }
            uiTimeLine.GenerateUI();
            await GenerateUI(token);
            SetPosition();
        });
        genkokuToggle.onValueChanged.AddListener(async (e) =>
        {
            await FilterUI(genkokuToggle.isOn, hikokuToggle.isOn, jijitsuToggle.isOn, token);
        });
        hikokuToggle.onValueChanged.AddListener(async (e) =>
        {
            await FilterUI(genkokuToggle.isOn, hikokuToggle.isOn, jijitsuToggle.isOn, token);
        });
        jijitsuToggle.onValueChanged.AddListener(async (e) =>
        {
            await FilterUI(genkokuToggle.isOn, hikokuToggle.isOn, jijitsuToggle.isOn, token);
        });
        timelineSlider.value = 300;//yearUnitLength;
        timelineSlider.onValueChanged.AddListener((e) =>
        {
            uiTimeLine.PinchTimeline(e);
            SetPosition();
        });
    }

    public bool is_top(DataType data_)
    {
        return data_.person.Contains("原告");
    }

    public void AddData(int text_id, DataType data_, int event_id)
    {
        if (data_.person == "" || data_.acts == "") return;
        if (data_.time.begin.value == 0 && data_.time.end.value == 0 && data_.time.point.value == 0) return;

        System.DateTime begin_value = System.DateTime.MinValue;
        System.DateTime end_value = System.DateTime.MaxValue;
        var is_range = false;
        if (data_.time.point != null && data_.time.point.value != 0)
        {
            begin_value = Utility.Convert(data_.time.point.value);
        }
        if (data_.time.begin != null && data_.time.begin.value != 0)
        {
            begin_value = Utility.Convert(data_.time.begin.value);
            is_range = true;
        }
        if (data_.time.end != null && data_.time.end.value != 0)
        {
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
                time_id = TimelineManager.Instance.AddTime(b, e, Time2Text(data_.time), is_top);
            }
            else
            {
                time_id = TimelineManager.Instance.AddTime(begin_value, Time2Text(data_.time), is_top);
            }
            timeMap[map_key] = time_id;
            var dat = new TimeStampData
            {
                text_id = text_id,
                event_id = event_id,
                person = data_.person,
                time_node = null,
                acts = new List<string> { data_.acts },
                is_top = is_top,
            };
            eventMap[map_key] = dat;
        }
    }

    async UniTask GenerateUI(CancellationToken token)
    {
        foreach (var i in eventMap)
        {
            var time_id = timeMap[i.Key];
            var obj = uiTimeLine.GetTimeTransform(time_id).gameObject;
            i.Value.time_node = obj.transform as RectTransform;
            var timeStampObj = Instantiate(timeStampPrefab, timeStampsContainer).GetComponent<UITimeStamp>();
            timeStampObj.SetData(i.Value);
            timeStamps.Add(timeStampObj);
            var dat = await HanreiRepository.Instance.GetText(FishboneViewManager.Instance.GetCurrentPath(), i.Value.text_id, token);
            var eve = await HanreiRepository.Instance.GetEvent(FishboneViewManager.Instance.GetCurrentPath(), i.Value.event_id, token);
            timeStampObj.OnEventClicked.AddListener((id) =>
            {
                var obj = ModalManager.Instance.AddModal(modalPrefab);
                obj.GetComponent<HanreiTextModal>().Init(dat.text, i.Value.person, Time2Text(eve.time), i.Value.acts[id]);
            });
            var eventButtonScr = Instantiate(eventButtonPrefab, eventsList).GetComponent<UIEventButton>();
            eventButtonScr.Init(time_id.ToString(), eve.person, Time2Text(eve.time));
            eventButtonScr.GetComponent<Button>().onClick.AddListener(async () =>
            {
                await CameraController.Instance.SetCenter(timeStampObj.transform.position);
                await timeStampObj.GetComponent<UIColorSet>().SetColorEffect(Color.yellow);
            });
        }
    }
    void SetPosition()
    {
        foreach (var i in timeStamps)
        {
            i.SetPosition();
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

    public async UniTask FilterUI(bool genkoku, bool hikoku, bool jijitsu, CancellationToken token)
    {
        foreach (Transform i in timeStampsContainer)
        {
            var scr = i.GetComponent<UITimeStamp>();
            var flag = await scr.matchFilter(genkoku, hikoku, jijitsu, token);
            if (flag)
            {
                scr.Activate();
            }
            else
            {
                scr.Deactivate();
            }
        }
    }

    string Time2Text(DataType.HanreiEventTimeData time)
    {
        var time_text = "";
        if (time.point != null && time.point.value != 0) time_text += time.point.text;
        if (time.begin != null && time.begin.value != 0) time_text += time.begin.text;
        if (time.end != null && time.end.value != 0) time_text += time.end.text;
        return time_text;
    }

}
