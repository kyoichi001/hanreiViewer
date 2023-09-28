using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIAnnotationsController : MonoBehaviour
{
    [Header("DOM")]
    [SerializeField] VisualTreeAsset annotationRow;
    [SerializeField] VisualTreeAsset tagRow;

    public class OnRelationDeletedEvent : UnityEvent<TokenRelation> { }
    public OnRelationDeletedEvent OnRelationDeleted { get; } = new OnRelationDeletedEvent();
    public class OnTagDeletedEvent : UnityEvent<TokenTag> { }
    public OnTagDeletedEvent OnTagDeleted { get; } = new OnTagDeletedEvent();

    public class OnRelationTypeChangedEvent:UnityEvent<TokenRelation,TokenRelationType?>{}
    public OnRelationTypeChangedEvent OnRelationTypeChanged { get; }=new OnRelationTypeChangedEvent();
    public class OnTagTypeChangedEvent : UnityEvent<TokenTag, TokenTagType?> { }
    public OnTagTypeChangedEvent OnTagTypeChanged { get; } = new OnTagTypeChangedEvent();

    public void GenerateAnnotations(VisualElement root,
        AnotationData data
        )
    {
        root.Clear();
        foreach (var i in data.annotations)
        {
            var dom = annotationRow.CloneTree();
            var button = dom.Q<Button>("deleteButton");
                button.clicked += () => OnRelationDeleted.Invoke(i);
            var textIDDOM = dom.Q<Label>("textID");
            textIDDOM.text = i.textID.ToString();
            var tokenIDDOM = dom.Q<Label>("tokenID");
            tokenIDDOM.text = i.tokenID.ToString();
            var tokenTextDOM = dom.Q<Label>("tokenText");
            tokenTextDOM.text = DataLoader.Instance.GetToken(data.filename, i.textID, i.tokenID).text;
            var targetIDDOM = dom.Q<Label>("targetID");
            targetIDDOM.text = i.targetID.ToString();
            var targetTextDOM = dom.Q<Label>("targetText");
            targetTextDOM.text = DataLoader.Instance.GetToken(data.filename, i.textID, i.targetID).text;
            var typeDOM = dom.Q<EnumField>("type");
            typeDOM.Init(i.type);
            typeDOM.RegisterValueChangedCallback(
                (e) => OnRelationTypeChanged.Invoke(i,e.newValue as TokenRelationType?)
                );
            root.Add(dom);
        }

        foreach (var i in data.tags)
        {
            var dom = tagRow.CloneTree();
            var button = dom.Q<Button>("deleteButton");
                button.clicked += () => OnTagDeleted.Invoke(i);
            var textIDDOM = dom.Q<Label>("textID");
            textIDDOM.text = i.textID.ToString();
            var tokenIDDOM = dom.Q<Label>("tokenID");
            tokenIDDOM.text = i.tokenID.ToString();
            var tokenTextDOM = dom.Q<Label>("tokenText");
            tokenTextDOM.text = DataLoader.Instance.GetToken(data.filename, i.textID, i.tokenID).text;
            var typeDOM = dom.Q<EnumField>("type");
            typeDOM.Init(i.type);
            typeDOM.RegisterValueChangedCallback(
                (e) => OnTagTypeChanged.Invoke(i,e.newValue as TokenTagType?)
                );
            root.Add(dom);
        }
    }
}
