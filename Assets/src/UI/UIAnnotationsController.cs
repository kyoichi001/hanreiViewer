using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAnnotationsController : MonoBehaviour
{
    [Header("DOM")]
    [SerializeField] VisualTreeAsset annotationRow;
    public void GenerateAnnotations(VisualElement root,string filename,List<TokenAnnotation> annotations,Action<TokenAnnotation> deleteClicked=null)
    {
        foreach(var i in annotations)
        {
            var dom= annotationRow.CloneTree();
            var button = dom.Q<Button>("deleteButton");
            if(deleteClicked != null)
                button.clicked += ()=>deleteClicked(i);
            var textIDDOM = dom.Q<Label>("textID");
            textIDDOM.text = i.textID.ToString();
            var tokenIDDOM = dom.Q<Label>("tokenID");
            tokenIDDOM.text = i.tokenID.ToString();
            var tokenTextDOM = dom.Q<Label>("tokenText");
            tokenTextDOM.text = DataLoader.Instance.GetToken(filename,i.textID,i.tokenID).text;
            var targetIDDOM = dom.Q<Label>("targetID");
            targetIDDOM.text = i.targetID.ToString();
            var targetTextDOM = dom.Q<Label>("targetText");
            targetTextDOM.text = DataLoader.Instance.GetToken(filename, i.textID, i.targetID).text;
            var typeDOM = dom.Q<Label>("type");
            typeDOM.text = i.type.ToString();
            root.Add(dom);
        }
    }
}
