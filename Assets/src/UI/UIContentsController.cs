using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;


public class UIContentsController : MonoBehaviour
{
    public AnnotationLoader annotationLoader;
    [Header("DOM")]
    public VisualTreeAsset foldableSectionUXML;
    public VisualTreeAsset sectionUXML;
    public VisualTreeAsset bunsetsuUXML;
    public VisualTreeAsset tokenUXML;
    public VisualTreeAsset arrowUXML;

    string cullentFilename;
    Dictionary<(int, int), VisualElement> tokenMap = new Dictionary<(int, int), VisualElement>();
    Dictionary<VisualElement, (int, int)> domMap = new Dictionary<VisualElement, (int, int)>();

    public class OnTokenMouseOverEvent : UnityEvent<Token> { }
    public OnTokenMouseOverEvent OnTokenMouseOver = new OnTokenMouseOverEvent();
    public class OnTokenMouseOutEvent : UnityEvent<Token> { }
    public OnTokenMouseOutEvent OnTokenMouseOut = new OnTokenMouseOutEvent();
    public class OnTokenClickedEvent : UnityEvent<Token, VisualElement> { }
    public OnTokenClickedEvent OnTokenClicked = new OnTokenClickedEvent();
    public class OnTokenDraggingEvent : UnityEvent<Token, VisualElement> { }
    public OnTokenDraggingEvent OnTokenDragging = new OnTokenDraggingEvent();
    public class OnTokenDraggedEvent : UnityEvent<Token, VisualElement, Token, VisualElement> { }
    public OnTokenDraggedEvent OnTokenDragged = new OnTokenDraggedEvent();


    private void Start()
    {
        OnTokenDragged.AddListener((token, tokenDOM, target, targetDOM) =>
        {
            Debug.Log("add relation");
            var textID = domMap[tokenDOM].Item1;
            annotationLoader.AddRelation(cullentFilename, textID, token.id, target.id, TokenRelationType.None);
        });
    }

    bool dragging = false;
    Token currentToken;
    VisualElement currentTokenDOM;

    void GenerateText(VisualElement root, SectionText text)
    {
        foreach (var bunsetsu in text.bunsetu)
        {
            var bunsetsuDOM = bunsetsuUXML.CloneTree();
            foreach (var token in bunsetsu.tokens)
            {
                var tokenDOM = tokenUXML.CloneTree();
                tokenDOM.Q<Label>().text = token.text;
                tokenDOM.RegisterCallback<MouseOverEvent>((type) =>
                {
                    OnTokenMouseOver.Invoke(token);
                });
                tokenDOM.RegisterCallback<MouseOutEvent>((type) =>
                {
                    OnTokenMouseOut.Invoke(token);
                });
                tokenDOM.RegisterCallback<PointerDownEvent>((type) =>
                {
                    //Debug.Log($"token coord : {tokenDOM.worldBound}");
                    OnTokenClicked.Invoke(token, tokenDOM);
                    dragging = true;
                    currentToken = token;
                    currentTokenDOM = tokenDOM;
                });
                tokenDOM.RegisterCallback<PointerMoveEvent>((type) =>
                {
                    if (dragging)
                    {
                        //Debug.Log($"pointer coord : {Input.mousePosition.x},{Screen.height-Input.mousePosition.y}");
                        OnTokenDragging.Invoke(currentToken, currentTokenDOM);
                    }
                });
                tokenDOM.RegisterCallback<PointerUpEvent>((type) =>
                {
                    OnTokenDragged.Invoke(currentToken, currentTokenDOM, token, tokenDOM);
                    dragging = false;
                    currentToken = null;
                    currentTokenDOM = null;
                });
                tokenDOM.RegisterCallback<PointerEnterEvent>((type) => { });
                if (text.GetTokenEntity(token.id) == EntityType.Date)
                {
                    tokenDOM.style.backgroundColor = new Color(1, 0, 0, 0.4f);
                }
                bunsetsuDOM.Q<VisualElement>("tokenContainer").Add(tokenDOM);
                tokenMap.Add((text.text_id, token.id), tokenDOM);
                domMap.Add(tokenDOM, (text.text_id, token.id));
            }
            root.Add(bunsetsuDOM);
        }
    }

    void GenerateAnnotationArrow(VisualElement root, VisualElement textDOM, int tokenID, int targetID, TokenRelationType type)
    {
        var leftPos = new Vector2();
        var rightPos = new Vector2();
        foreach (var dom in textDOM.Children())
        {
            if (domMap[dom].Item2 == tokenID)
            {
                leftPos = new Vector2(dom.resolvedStyle.left, dom.resolvedStyle.top);
            }
            else if (domMap[dom].Item2 == targetID)
            {
                rightPos = new Vector2(dom.resolvedStyle.left, dom.resolvedStyle.top);
            }
        }
        GenerateArrow(root, leftPos, rightPos, 3);
    }

    void GenerateMarker(VisualElement root, VisualElement textDOM, int tokenID, int targetID, TokenRelationType type)
    {
        var marker = new VisualElement();
        marker.style.backgroundColor = new Color(1, 0, 0);
        foreach (var dom in textDOM.Children())
        {
            if (domMap[dom].Item2 == tokenID)
            {
                marker.transform.position = dom.transform.position;
            }
        }
        root.Add(marker);
    }

    void GenerateArrow(VisualElement root, Vector2 leftPos, Vector2 rightPos, int height)
    {
        //TODO:
        var dom = arrowUXML.CloneTree();
        dom.Q<VisualElement>("arrowContainer").style.width = rightPos.x - leftPos.x;
        dom.Q<VisualElement>("arrowContainer").style.height = height;
        dom.Q<VisualElement>("arrowContainer").style.height = height;
        root.Add(dom);
    }
    void HighlightRelation(VisualElement tokenDOM, VisualElement tokenDOM2)
    {
        tokenDOM.style.backgroundColor = new Color(0, 0.6f, 0);
        tokenDOM2.style.backgroundColor = new Color(0, 0.6f, 0);
    }
    void ResetHighlight(VisualElement tokenDOM, VisualElement tokenDOM2)
    {
        tokenDOM.style.backgroundColor = new Color(0, 0, 0, 0);
        tokenDOM2.style.backgroundColor = new Color(0, 0, 0, 0);
    }

    bool hasChild(Section target, Section next)
    {
        return next.indent == target.indent + 1;
    }
    bool HasAnnotation(SectionText text, List<TokenAnnotation> annotations)
    {
        foreach (var annotation in annotations)
        {
            if (annotation.textID == text.text_id) return true;
        }
        return false;
    }

    public void generateSectionsDOM(VisualElement root, string filename, List<Section> sections, List<TokenAnnotation> annotations)
    {
        cullentFilename = filename;
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
                        foreach (var annotation in annotations)
                        {
                            GenerateMarker(
                                section.Q<VisualElement>("childContainer"),
                                section.Q<VisualElement>("childContainer"),
                                annotation.tokenID,
                                annotation.targetID,
                                annotation.type
                                );
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

    public void ClearAnnotation()
    {

    }
    public void GenerateAnnotation()
    {
        foreach (var i in annotationLoader.GetAnnotations(cullentFilename))
        {
            var token1 = (i.textID, i.tokenID);
            var token2 = (i.textID, i.targetID);
            tokenMap[token1].RegisterCallback<MouseOverEvent>((type) =>
            {
                HighlightRelation(tokenMap[token1], tokenMap[token2]);
            });
            tokenMap[token1].RegisterCallback<MouseOutEvent>((type) =>
            {
                ResetHighlight(tokenMap[token1], tokenMap[token2]);
            });
            tokenMap[token2].RegisterCallback<MouseOverEvent>((type) =>
            {
                HighlightRelation(tokenMap[token1], tokenMap[token2]);
            });
            tokenMap[token2].RegisterCallback<MouseOutEvent>((type) =>
            {
                ResetHighlight(tokenMap[token1], tokenMap[token2]);
            });
        }
    }
}
