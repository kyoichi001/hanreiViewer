using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(UIFilesController))]
[RequireComponent(typeof(UIContentsController))]
[RequireComponent(typeof(UIContentsTableController))]
public class UIManager : MonoBehaviour
{
    [Header("DOM")]
    [SerializeField] VisualTreeAsset hanreiUXML;

    UIDocument uIDocument;
    UIFilesController uIFilesController;
    UIContentsController uContentsController;
    UIContentsTableController uContentsTableController;

    HanreiData currentData;

    private void Awake()
    {
        uIDocument = GetComponent<UIDocument>();
        uIFilesController = GetComponent<UIFilesController>();
        uContentsController = GetComponent<UIContentsController>();
        uContentsTableController = GetComponent<UIContentsTableController>();

        var submenuElement = uIDocument.rootVisualElement.Q("subMenu");
        FileTextExtracter.Instance.OnDataLoaded.AddListener((dat) =>
        {
            uIFilesController.GenerateButton(submenuElement, dat);
        });
        /*AnnotationLoader.Instance.OnDataChanged.AddListener((d) =>
        {
            RenewHanrei();
        });*/
        uIFilesController.OnButtonClicked.AddListener((dat) =>
        {
            try
            {
                ShowHanrei(dat);
            }
            catch (Exception e)
            {
                Akak.Debug.PrintError(e.Message);
            }
        });
    }

    void GenerateStatistics(VisualElement root, HanreiData data)
    {
        var wordsCount = 0;
        var textsCount = 0;
        var tokensCount = 0;
        foreach (var i in data.contents.fact_reason.sections)
        {
            foreach (var j in i.texts)
            {
                foreach (var k in j.bunsetu)
                {
                    foreach (var l in k.tokens)
                    {
                        wordsCount += l.text.Length;
                        tokensCount++;
                    }
                }
                textsCount++;
            }
        }
        root.Q<Label>("wordsCount").text = wordsCount.ToString();
        root.Q<Label>("textsCount").text = textsCount.ToString();
        root.Q<Label>("tokensCount").text = tokensCount.ToString();
    }

    void ShowHanrei(HanreiData data)
    {
        currentData = data;
        var container = uIDocument.rootVisualElement.Q<ScrollView>("contentContainer");
        container.Clear();
        var t = hanreiUXML.CloneTree();
        t.Q<Label>("title").text = data.filename;
        t.Q<Foldout>("signatureLabel").text = data.contents.signature.header_text;
        var signatureLst = t.Q<VisualElement>("signatureContents");
        foreach (var i in data.contents.signature.texts)
        {
            signatureLst.Add(new Label(i));
        }
        t.Q<Foldout>("judgementLabel").text = data.contents.judgement.header_text;
        var judgementLst = t.Q<VisualElement>("judgementContents");
        foreach (var i in data.contents.judgement.texts)
        {
            judgementLst.Add(new Label(i));
        }
        t.Q<Foldout>("mainTextLabel").text = data.contents.main_text.header_text;
        uContentsController.generateSectionsDOM(
            t.Q<VisualElement>("mainTextContents"),
            data.filename,
            data.contents.main_text.sections);
        uContentsController.OnTokenMouseOver.AddListener((token, tokenDOM) =>
        {
            //popoverManager.AddPopover();
        });
        uContentsController.OnTokenMouseOut.AddListener((token, tokenDOM) =>
        {
            //popoverManager.RemovePopover();
        });

        t.Q<Foldout>("factReasonLabel").text = data.contents.fact_reason.header_text;
        uContentsController.generateSectionsDOM(
            t.Q<VisualElement>("factReasonContents"),
            data.filename,
            data.contents.fact_reason.sections);
        container.Add(t);
        uContentsTableController.generateTableOfContentDOM(
            uIDocument.rootVisualElement.Q<ScrollView>("tableContainer"), data.contents.fact_reason.sections
            );
        GenerateStatistics(uIDocument.rootVisualElement, data);
    }
    void RenewHanrei()
    {
        GenerateStatistics(uIDocument.rootVisualElement, currentData);
    }
}
