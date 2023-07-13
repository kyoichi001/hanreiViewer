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
    public VisualTreeAsset arrowUXML;
    public AnnotationLoader annotationLoader;


    public class OnTokenMouseOverEvent : UnityEvent<Token> { }
    public OnTokenMouseOverEvent OnTokenMouseOver=new OnTokenMouseOverEvent();
    public class OnTokenMouseOutEvent : UnityEvent<Token> { }
    public OnTokenMouseOutEvent OnTokenMouseOut=new OnTokenMouseOutEvent();
    public class OnTokenClickedEvent : UnityEvent<Token> { }
    public OnTokenClickedEvent OnTokenClicked=new OnTokenClickedEvent();
    public class OnTokenDraggingEvent : UnityEvent<Token> { }
    public OnTokenDraggingEvent OnTokenDragging=new OnTokenDraggingEvent();
    public class OnTokenDraggedEvent : UnityEvent<Token,Token> { }
    public OnTokenDraggedEvent OnTokenDragged=new OnTokenDraggedEvent();


    private void Start()
    {
        OnTokenDragged.AddListener((token,target) =>
        {
            var textID = 0;
            var tokenID = token.id;
            annotationLoader.AddRelation(textID,tokenID, target.id,TokenRelationType.None);
        });
    }

    bool dragging = false;
    Token currentToken;
    Token targetToken;

    bool hasChild(Section target, Section next)
    {
        return next.indent == target.indent + 1;
    }

    void GenerateText(VisualElement root, SectionText text)
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
                tokenDOM.RegisterCallback<PointerDownEvent>((type) =>
                {
                    Debug.Log($"on pointer down : {token.text}");
                    OnTokenClicked.Invoke(token);
                    dragging = true;
                });
                tokenDOM.RegisterCallback<PointerMoveEvent>((type) =>
                {
                    Debug.Log($"on pointer move : {token.text}");
                    if (dragging)
                        OnTokenDragging.Invoke(token);
                });
                tokenDOM.RegisterCallback<PointerUpEvent>((type) =>
                {
                    Debug.Log($"on pointer up : {token.text}");
                    OnTokenDragged.Invoke(token, targetToken);
                    dragging = false;
                });
                tokenDOM.RegisterCallback<PointerEnterEvent>((type) =>
                {
                    Debug.Log($"on pointer enter : {token.text}");
                    if (dragging)
                    {
                        targetToken = token;
                    }
                });
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
        GenerateArrow(root,leftPos,rightPos,3);
    }
    void GenerateMarker(VisualElement root, VisualElement textDOM, int tokenID, TokenRelation relation)
    {
        var marker=new VisualElement();
        marker.style.backgroundColor=new Color(1, 0,0);
        foreach (var dom in textDOM.Children())
        {
            if (dom.tooltip == tokenID.ToString())
            {
                marker.transform.position=dom.transform.position;
            }
        }
        root.Add(marker);
    }

    void GenerateArrow(VisualElement root,Vector2 leftPos,Vector2 rightPos,int height)
    {
        //TODO:
        var dom = arrowUXML.CloneTree();
        dom.Q<VisualElement>("arrowContainer").style.width= rightPos.x - leftPos.x;
        dom.Q<VisualElement>("arrowContainer").style.height = height;
        dom.Q<VisualElement>("arrowContainer").style.height = height;
        root.Add(dom);
    }
    bool HasAnnotation(SectionText text,List<TokenAnnotation> annotations)
    {
        foreach(var annotation in annotations)
        {
            if (annotation.textID == text.text_id) return true;
        }
        return false;
    }

    public void generateSectionsDOM(VisualElement root, List<Section> sections, List<TokenAnnotation> annotations)
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
                    if (HasAnnotation(text, annotations))
                    {
                        foreach(var annotation in annotations)
                        {
                            foreach(var relation in annotation.relations)
                            {
                                GenerateMarker(
                                    section.Q<VisualElement>("childContainer"),
                                    section.Q<VisualElement>("childContainer"),
                                    annotation.tokenID,
                                    relation
                                    );
                            }
                        }
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
                    GenerateText(section.Q<VisualElement>("textContainer"), text);
                }
                indentContainer[sectionData.indent - 1].Add(section);
            }
        }
    }
}
