using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public class UIManager : MonoBehaviour
{
    public DataLoader dataLoader;
    UIDocument uIDocument;
public    VisualTreeAsset hanreiUXML;
public    VisualTreeAsset sectionUXML;
    private void Awake()
    {
        uIDocument=GetComponent<UIDocument>();
        var submenuElement = uIDocument.rootVisualElement.Q("subMenu");
        dataLoader.OnDataLoaded.AddListener((dat) =>
        {
            var newVisualElement = new Button();
            newVisualElement.name = "AddButton";
            newVisualElement.AddToClassList("sample-button");
            newVisualElement.text = dat.filename;
            newVisualElement.clicked += () => {
                ShowHanrei(dat);
            };
            submenuElement.Add(newVisualElement);
        });
    }

    void ShowHanrei(HanreiData data)
    {
        var container = uIDocument.rootVisualElement.Q<ScrollView>("contentContainer");
        container.Clear();
        var t = hanreiUXML.CloneTree();
        t.Q<Label>("title").text = data.filename;
        t.Q<Label>("signatureLabel").text = data.contents.signature.header_text;
        var signatureLst = t.Q<VisualElement>("signatureContents");
        foreach (var i in data.contents.signature.texts)
        {
            var text = new Label();
            text.text = i;
            signatureLst.Add(text);
        }
        t.Q<Label>("judgementLabel").text = data.contents.judgement.header_text;
        var judgementLst = t.Q<VisualElement>("judgementContents");
        foreach (var i in data.contents.judgement.texts)
        {
            var text = new Label();
            text.text = i;
            judgementLst.Add(text);
        }
        t.Q<Label>("mainTextLabel").text = data.contents.main_text.header_text;
        var mainTexttLst = t.Q<VisualElement>("mainTextContents");
        foreach (var i in data.contents.main_text.sections)
        {
            var section = sectionUXML.CloneTree();
            section.Q<Label>("header").text = i.header;
            section.Q<Label>("headerText").text = i.header_text;
            if (i.header_text == "" || i.header_text==null)
                section.Q<VisualElement>("rightContainer").Remove(
                    section.Q<Label>("headerText")
                    );
            section.Q<Label>("text").text = i.text;
            if (i.text == "" || i.text == null)
                section.Q<VisualElement>("rightContainer").Remove(
                    section.Q<Label>("text")
                    );
            mainTexttLst.Add(section);
        }
        t.Q<Label>("factReasonLabel").text = data.contents.fact_reason.header_text;
        var factReasonLst = t.Q<VisualElement>("factReasonContents");
        foreach (var i in data.contents.fact_reason.sections)
        {
            var section = sectionUXML.CloneTree();
            section.Q<Label>("header").text = i.header;
            section.Q<Label>("headerText").text = i.header_text;
            if (i.header_text == "" || i.header_text == null)
                section.Q<VisualElement>("rightContainer").Remove(
                    section.Q<Label>("headerText")
                    );
            section.Q<Label>("text").text = i.text;
            if (i.text == "" || i.text == null)
                section.Q<VisualElement>("rightContainer").Remove(
                    section.Q<Label>("text")
                    );
            factReasonLst.Add(section);
        }
        container.Add(t);
    }
}
