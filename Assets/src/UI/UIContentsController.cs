using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;

public class UIContentsController : MonoBehaviour
{
    public VisualTreeAsset foldableSectionUXML;
    public VisualTreeAsset sectionUXML;
    public VisualTreeAsset bunsetsuUXML;
    public VisualTreeAsset tokenUXML;

    public class OnTokenMouseOverEvent : UnityEvent<Token> { }
    public OnTokenMouseOverEvent OnTokenMouseOver;
    public class OnTokenMouseOutEvent : UnityEvent<Token> { }
    public OnTokenMouseOutEvent OnTokenMouseOut;

    bool hasChild(Section target, Section next)
    {
        return next.indent == target.indent + 1;
    }

    void GenerateText(VisualElement root,SectionText text)
    {
        foreach (var bunsetsu in text.bunsetu)
        {
            var bunsetsuDOM = bunsetsuUXML.CloneTree();
            foreach (var token in bunsetsu.tokens)
            {
                var tokenDOM = tokenUXML.CloneTree();
                tokenDOM.Q<Label>().text = token.text;
                tokenDOM.RegisterCallback<MouseOverEvent>((type) => OnTokenMouseOver.Invoke(token));
                tokenDOM.RegisterCallback<MouseOutEvent>((type) => OnTokenMouseOut.Invoke(token));
                tokenDOM.tooltip = token.id.ToString();// tokenのIDとDOMを結び付け、検索できるようにする（こうするべきではない）
                if (text.GetTokenEntity(token.id) == EntityType.Date)
                {
                    tokenDOM.style.backgroundColor = new Color(1, 0, 0, 0.4f);
                }
                bunsetsuDOM.Q<VisualElement>("tokenContainer").Add(tokenDOM);
            }
            root.Add(bunsetsuDOM);
        }
    }

    void GenerateAnnotation(VisualElement root,VisualElement textDOM, int tokenID, TokenRelation relation)
    {
        var leftPos = new Vector2();
        var rightPos = new Vector2();
        foreach(var dom in textDOM.Children())
        {
            if (dom.tooltip == tokenID.ToString())
            {
                leftPos = new Vector2(dom.resolvedStyle.left,dom.resolvedStyle.top);
            }
            else if (dom.tooltip==relation.targetID.ToString())
            {
                rightPos = new Vector2(dom.resolvedStyle.left, dom.resolvedStyle.top);
            }
        }
        GenerateArrow(root,leftPos,rightPos);
    }

    void GenerateArrow(VisualElement root,Vector2 leftPos,Vector2 rightPos)
    {
        //TODO:
    }


    public void generateSectionsDOM(VisualElement root, List<Section> sections)
    {
        var indentContainer = new List<VisualElement> { root };
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
                    GenerateText(section.Q<VisualElement>("childContainer"), text);
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
                    GenerateText(section.Q<VisualElement>("textContainer"), text);
                }
                indentContainer[sectionData.indent - 1].Add(section);
            }
        }
    }

}
