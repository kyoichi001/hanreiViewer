using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleDataFileUI : MonoBehaviour
{
    string pathText;
    [SerializeField] TMPro.TextMeshProUGUI path;
    [SerializeField] Button deleteButton;

    public class OnDeleteButtonClickedEvent : UnityEvent<string> { }
    public OnDeleteButtonClickedEvent OnDeleteButtonClicked { get; } = new OnDeleteButtonClickedEvent();
    public void Init(string path_)
    {
        pathText = path_;
        path.text = pathText;
    }
    void Awake()
    {
        deleteButton.onClick.AddListener(() =>
        {
            OnDeleteButtonClicked.Invoke(pathText);
        });
    }
}
