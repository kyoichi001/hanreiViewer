using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHanreiSelectorButton : MonoBehaviour
{
    public UnityEvent OnShowFishbone { get; } = new UnityEvent();
    public UnityEvent OnShowTextview { get; } = new UnityEvent();

    [SerializeField] Color activeColor;
    [SerializeField] Color normalColor;

    [SerializeField] Button FishboneButton;
    [SerializeField] TMPro.TextMeshProUGUI fishboneText;
    [SerializeField] Button TextviewButton;
    Image background;

    void Awake()
    {
        background = GetComponent<Image>();
        FishboneButton.onClick.AddListener(() =>
        {
            OnShowFishbone.Invoke();
        });
        TextviewButton.onClick.AddListener(() =>
        {
            OnShowTextview.Invoke();
        });
    }
    public void Init(string title)
    {
        fishboneText.text = title;
    }
    public void Activate()
    {
        background.color = activeColor;
    }
    public void Deactivate()
    {
        background.color = normalColor;
    }

}
