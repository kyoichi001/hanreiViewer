using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    [SerializeField] DataLoader dataLoader;
    [SerializeField] AnnotationLoader annotationLoader;
    [Header("DOM")]
    [SerializeField] VisualTreeAsset hanreiUXML;

    UIDocument uIDocument;
    UIFilesController uIFilesController;
    UIContentsController uContentsController;
    UIContentsTableController uContentsTableController;
    UIAnnotationsController uAnnotationsController;

    PopoverManager popoverManager;

    private void Awake()
    {
        uIDocument = GetComponent<UIDocument>();
        uIFilesController = GetComponent<UIFilesController>();
        uContentsController = GetComponent<UIContentsController>();
        uContentsTableController = GetComponent<UIContentsTableController>();
        popoverManager = GetComponent<PopoverManager>();
        uAnnotationsController=GetComponent<UIAnnotationsController>();

        var submenuElement = uIDocument.rootVisualElement.Q("subMenu");
        dataLoader.OnDataLoaded.AddListener((dat) =>
        {
            uIFilesController.GenerateButton(dat);
        });
        uIFilesController.OnButtonClicked.AddListener((dat) => ShowHanrei(dat));
    }

    void ShowHanrei(HanreiData data)
    {
        var container = uIDocument.rootVisualElement.Q<ScrollView>("contentContainer");
        container.Clear();
        var t = hanreiUXML.CloneTree();
        t.Q<Label>("title").text = data.filename;
        // 書誌情報生成
        t.Q<Foldout>("signatureLabel").text = data.contents.signature.header_text;
        var signatureLst = t.Q<VisualElement>("signatureContents");
        foreach (var i in data.contents.signature.texts)
        {
            signatureLst.Add(new Label(i));
        }
        // 判決生成
        t.Q<Foldout>("judgementLabel").text = data.contents.judgement.header_text;
        var judgementLst = t.Q<VisualElement>("judgementContents");
        foreach (var i in data.contents.judgement.texts)
        {
            judgementLst.Add(new Label(i));
        }
        //主文生成
        t.Q<Foldout>("mainTextLabel").text = data.contents.main_text.header_text;
        uContentsController.generateSectionsDOM(
            t.Q<VisualElement>("mainTextContents"), 
            data.filename,
            data.contents.main_text.sections);
        uContentsController.OnTokenMouseOver.AddListener((token,tokenDOM) =>
        {
            //popoverManager.AddPopover();
        });
        uContentsController.OnTokenMouseOut.AddListener((token, tokenDOM) =>
        {
            //popoverManager.RemovePopover();
        });

        //事実及び理由生成
        t.Q<Foldout>("factReasonLabel").text = data.contents.fact_reason.header_text;
        uContentsController.generateSectionsDOM(
            t.Q<VisualElement>("factReasonContents"),
            data.filename,
            data.contents.fact_reason.sections);
        container.Add(t);
        uContentsTableController.generateTableOfContentDOM(
            uIDocument.rootVisualElement.Q<ScrollView>("tableContainer"), data.contents.fact_reason.sections
            );
        uContentsController.GenerateAnnotation();
        var annotationContainer=uIDocument.rootVisualElement.Q<ScrollView>("annotationContainer");
        uAnnotationsController.GenerateAnnotations(annotationContainer,data.filename, annotationLoader.GetAnnotations(data.filename));
    }
}
