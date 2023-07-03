using System;
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
        t.Q<Foldout>("signatureLabel").text = data.contents.signature.header_text;
        var signatureLst = t.Q<VisualElement>("signatureContents");
        foreach (var i in data.contents.signature.texts)
        {
            var text = new Label();
            text.text = i;
            signatureLst.Add(text);
        }
        t.Q<Foldout>("judgementLabel").text = data.contents.judgement.header_text;
        var judgementLst = t.Q<VisualElement>("judgementContents");
        foreach (var i in data.contents.judgement.texts)
        {
            var text = new Label();
            text.text = i;
            judgementLst.Add(text);
        }
        t.Q<Foldout>("mainTextLabel").text = data.contents.main_text.header_text;
        var mainTexttLst = t.Q<VisualElement>("mainTextContents");
        var indentContainer =new List<VisualElement>();
        indentContainer.Add(t.Q<VisualElement>("mainTextContents"));
        foreach (var i in data.contents.main_text.sections)
        {
            var section = sectionUXML.CloneTree();
            section.Q<Foldout>("header").text = i.header;
            section.Q<Label>("headerText").text = i.header_text;
            if (i.header_text == "" || i.header_text==null)
                section.Q<VisualElement>("unity-content").Remove(
                    section.Q<Label>("headerText")
                    );
            foreach(var text in i.texts)
            {
                var textDOM = new Label();
                textDOM.text = text.raw_text;
                section.Q<VisualElement>("childContainer").Add(textDOM);
            }
            Debug.Log(indentContainer);
            indentContainer[i.indent-1].Add(section);
            if (indentContainer.Count <= i.indent)
            {
                indentContainer.Add(section.Q<VisualElement>("unity-content"));
            }
            else
            {
                indentContainer[i.indent] = section.Q<VisualElement>("unity-content");
            }
        }
        t.Q<Foldout>("factReasonLabel").text = data.contents.fact_reason.header_text;
        var factReasonLst = t.Q<VisualElement>("factReasonContents");
        indentContainer = new List<VisualElement>();
        indentContainer.Add(t.Q<VisualElement>("factReasonContents"));
        foreach (var i in data.contents.fact_reason.sections)
        {
            var section = sectionUXML.CloneTree();
            section.Q<Foldout>("header").text = i.header;
            section.Q<Label>("headerText").text = i.header_text;
            if (i.header_text == "" || i.header_text == null)
                section.Q<VisualElement>("unity-content").Remove(
                    section.Q<Label>("headerText")
                    );
            foreach (var text in i.texts)
            {
                var textDOM = new Label();
                textDOM.text = text.raw_text;
                section.Q<VisualElement>("childContainer").Add(textDOM);
            }
            Debug.Log(indentContainer);
            indentContainer[i.indent - 1].Add(section);
            if (indentContainer.Count <= i.indent)
            {
                Debug.Log($"add container {indentContainer.Count} : {i.indent}");
                indentContainer.Add(section.Q<VisualElement>("unity-content"));
            }
            else
            {
                indentContainer[i.indent] = section.Q<VisualElement>("unity-content");
            }
        }
        container.Add(t);
    }
}
