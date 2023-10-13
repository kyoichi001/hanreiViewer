using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIFilesController : MonoBehaviour
{
    public class OnButtonClickedEvent : UnityEvent<HanreiData> { }

    public OnButtonClickedEvent OnButtonClicked=new OnButtonClickedEvent();

    public void GenerateButton(VisualElement root,HanreiData dat)
    {
        var newVisualElement = new Button();
        newVisualElement.name = "AddButton";
        newVisualElement.AddToClassList("sample-button");
        newVisualElement.text = dat.filename;
        newVisualElement.clicked += () => OnButtonClicked.Invoke(dat);
        root.Add(newVisualElement);
    }

}
