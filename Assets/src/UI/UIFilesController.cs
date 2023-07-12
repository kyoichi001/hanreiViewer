using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIFilesController : MonoBehaviour
{
    [System.Serializable]
    public class OnButtonClickedEvent : UnityEvent<HanreiData> { }

    public OnButtonClickedEvent OnButtonClicked;

    public void GenerateButton(HanreiData dat)
    {
        UIDocument uIDocument = GetComponent<UIDocument>();
        var submenuElement = uIDocument.rootVisualElement.Q("subMenu");
        var newVisualElement = new Button();
        newVisualElement.name = "AddButton";
        newVisualElement.AddToClassList("sample-button");
        newVisualElement.text = dat.filename;
        newVisualElement.clicked += () => OnButtonClicked.Invoke(dat);
        submenuElement.Add(newVisualElement);
    }

}
