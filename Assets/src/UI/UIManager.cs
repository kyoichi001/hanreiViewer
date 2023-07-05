using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;
using static UnityEditor.Rendering.FilterWindow;

public class UIManager : MonoBehaviour
{
    public DataLoader dataLoader;
    UIDocument uIDocument;
    public VisualTreeAsset hanreiUXML;
    public VisualTreeAsset foldableSectionUXML;
    public VisualTreeAsset sectionUXML;
    public VisualTreeAsset tokenUXML;

    private void Awake()
    {
        uIDocument = GetComponent<UIDocument>();
        var submenuElement = uIDocument.rootVisualElement.Q("subMenu");
        dataLoader.OnDataLoaded.AddListener((dat) =>
        {
            var newVisualElement = new Button();
            newVisualElement.name = "AddButton";
            newVisualElement.AddToClassList("sample-button");
            newVisualElement.text = dat.filename;
            newVisualElement.clicked += () =>
            {
                ShowHanrei(dat);
            };
            submenuElement.Add(newVisualElement);
        });
    }

    bool hasChild(Section target, Section next)
    {
        return next.indent == target.indent + 1;
    }
    void generateTableOfContentDOM(VisualElement root, List<Section> sections)
    {
        var indentContainer = new List<VisualElement> { root };
        for (var i = 0; i < sections.Count - 1; i++)
        {
            var sectionData = sections[i];
            if (hasChild(sectionData, sections[i + 1]))
            {
                //セクションが子要素を含むならfoldoutにする
                var section = foldableSectionUXML.CloneTree();
                section.Q<Foldout>("header").text = sectionData.header + "  " + sectionData?.header_text;
                indentContainer[sectionData.indent - 1].Add(section);
                if (indentContainer.Count <= sectionData.indent)
                {
                    indentContainer.Add(section.Q<VisualElement>("unity-content"));
                }
                else
                {
                    indentContainer[sectionData.indent] = section.Q<VisualElement>("unity-content");
                }
            }
            else if(sectionData.header_text?.Length>0)
            {
                var section = sectionUXML.CloneTree();
                section.Q<Label>("header").text = sectionData.header+sectionData.header_text;
                section.Q<VisualElement>("sectionContainer").Remove(section.Q<VisualElement>("rightContainer"));
                indentContainer[sectionData.indent - 1].Add(section);
            }
        }
        var sectionDat = sections[sections.Count-1];
        var sectionDOM = sectionUXML.CloneTree();
        sectionDOM.Q<Label>("header").text = sectionDat.header + sectionDat.header_text;
        sectionDOM.Q<VisualElement>("sectionContainer").Remove(sectionDOM.Q<VisualElement>("rightContainer"));
        indentContainer[sectionDat.indent - 1].Add(sectionDOM);
    }

    void generateSectionsDOM(VisualElement root, List<Section> sections)
    {
        var indentContainer = new List<VisualElement>{root};
        for (var i = 0; i < sections.Count; i++)
        {
            var sectionData = sections[i];
            if (i < sections.Count - 1 &&
                (hasChild(sectionData, sections[i + 1]) ||
                sectionData.header_text?.Length > 0 && sectionData.texts.Count > 0
                ))
            {
                //セクションが子要素を含むか、header_text,text両方を含むならfoldoutにする
                var section = foldableSectionUXML.CloneTree();
                section.Q<Foldout>("header").text = sectionData.header + "  " + sectionData?.header_text;
                foreach (var text in sectionData.texts)
                {
                    foreach(var token in text.tokens)
                    {
                        var tokenDOM = tokenUXML.CloneTree();
                        tokenDOM.Q<Label>().text = token.text;
                        section.Q<VisualElement>("childContainer").Add(tokenDOM);
                    }
                }
                indentContainer[sectionData.indent - 1].Add(section);
                if (indentContainer.Count <= sectionData.indent)
                {
                    indentContainer.Add(section.Q<VisualElement>("unity-content"));
                }
                else
                {
                    indentContainer[sectionData.indent] = section.Q<VisualElement>("unity-content");
                }
            }
            else
            {
                var section = sectionUXML.CloneTree();
                section.Q<Label>("header").text = sectionData.header;
                section.Q<Label>("headerText").text = sectionData.header_text;
                if (sectionData.header_text == "" || sectionData.header_text == null)
                    section.Q<VisualElement>("rightContainer").Remove(
                        section.Q<Label>("headerText")
                        );
                foreach (var text in sectionData.texts)
                {
                    foreach(var token in text.tokens)
                    {
                        var tokenDOM = tokenUXML.CloneTree();
                        tokenDOM.Q<Label>().text=token.text;
                        section.Q<VisualElement>("textContainer").Add(tokenDOM);
                    }
                }
                indentContainer[sectionData.indent - 1].Add(section);
            }
        }
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
        generateSectionsDOM(t.Q<VisualElement>("mainTextContents"), data.contents.main_text.sections);
        t.Q<Foldout>("factReasonLabel").text = data.contents.fact_reason.header_text;
        generateSectionsDOM(t.Q<VisualElement>("factReasonContents"), data.contents.fact_reason.sections);
        container.Add(t);
        generateTableOfContentDOM(uIDocument.rootVisualElement.Q<ScrollView>("tableContainer"), data.contents.fact_reason.sections);
    }
}
