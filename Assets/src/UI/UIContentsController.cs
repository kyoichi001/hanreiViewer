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

    public class OnTokenMouseOverEvent : UnityEvent<Token, VisualElement> { }
    public OnTokenMouseOverEvent OnTokenMouseOver = new OnTokenMouseOverEvent();
    public class OnTokenMouseOutEvent : UnityEvent<Token, VisualElement> { }
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
            if (!domMap.ContainsKey(tokenDOM) || !domMap.ContainsKey(targetDOM))
            {
                Debug.LogError($"key does not found token1:{tokenDOM} token2:{targetDOM}");
                return;
            }
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
                    var dom = type.target as VisualElement;
                    //Debug.Log(dom.Q<Label>().text);
                    OnTokenMouseOver.Invoke(token, dom);
                });
                tokenDOM.RegisterCallback<MouseOutEvent>((type) =>
                {
                    var dom = type.target as VisualElement;
                    OnTokenMouseOut.Invoke(token, dom);
                });
                tokenDOM.RegisterCallback<PointerDownEvent>((type) =>
                {
                    //Debug.Log($"token coord : {tokenDOM.worldBound}");
                    var dom = type.target as VisualElement;
                    OnTokenClicked.Invoke(token, dom);
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
                    //var dom = type.target as VisualElement; //���ꂾ�Ƃ��܂�Dictionary��key�ƈ�v���Ȃ��B�Q�Ɛ�͓��������ʕ������H�L���X�g���Ă��邩��H
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
                //�Z�N�V�������q�v�f���܂ނ��Aheader_text,text�������܂ނȂ�foldout�ɂ���
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
        Debug.Log($"generate annotation {annotationLoader.GetAnnotations(cullentFilename).Count}");
        foreach (var i in annotationLoader.GetAnnotations(cullentFilename))
        {
            var token1 = (i.textID, i.tokenID);
            var token2 = (i.textID, i.targetID);
            if (!tokenMap.ContainsKey(token1) || !tokenMap.ContainsKey(token2))
            {
                Debug.LogError($"key does not found token1:{token1} token2:{token2}");
                continue;
            }

            var col = new Color(0.5f, 1, 0.5f);
            var borderWidth = 3f;

            tokenMap[token1].style.borderLeftColor = col;
            tokenMap[token1].style.borderLeftWidth = borderWidth;
            tokenMap[token1].style.borderTopColor = col;
            tokenMap[token1].style.borderTopWidth = borderWidth;
            tokenMap[token1].style.borderBottomColor = col;
            tokenMap[token1].style.borderBottomWidth = borderWidth;
            tokenMap[token1].style.borderRightColor = col;
            tokenMap[token1].style.borderRightWidth = borderWidth;

            tokenMap[token2].style.borderLeftColor = col;
            tokenMap[token2].style.borderLeftWidth = borderWidth;
            tokenMap[token2].style.borderTopColor = col;
            tokenMap[token2].style.borderTopWidth = borderWidth;
            tokenMap[token2].style.borderBottomColor = col;
            tokenMap[token2].style.borderBottomWidth = borderWidth;
            tokenMap[token2].style.borderRightColor = col;
            tokenMap[token2].style.borderRightWidth = borderWidth;

            tokenMap[token1].RegisterCallback<MouseOverEvent>((type) =>
            {
                Debug.Log("on annotation hover");
                HighlightRelation(tokenMap[token1], tokenMap[token2]);
            });
            tokenMap[token1].RegisterCallback<MouseOutEvent>((type) =>
            {
                Debug.Log("on annotation hover exit");
                ResetHighlight(tokenMap[token1], tokenMap[token2]);
            });
            tokenMap[token2].RegisterCallback<MouseOverEvent>((type) =>
            {
                Debug.Log("on annotation hover");
                HighlightRelation(tokenMap[token1], tokenMap[token2]);
            });
            tokenMap[token2].RegisterCallback<MouseOutEvent>((type) =>
            {
                Debug.Log("on annotation hover exit");
                ResetHighlight(tokenMap[token1], tokenMap[token2]);
            });
        }
        OnTokenDragging.AddListener((token, tokenDOM) =>
        {
            var col = new Color( 1, 0.5f, 0.5f);
            var borderWidth = 3f;
            tokenDOM.style.borderLeftColor = col;
            tokenDOM.style.borderLeftWidth = borderWidth;
            tokenDOM.style.borderTopColor = col;
            tokenDOM.style.borderTopWidth = borderWidth;
            tokenDOM.style.borderBottomColor = col;
            tokenDOM.style.borderBottomWidth = borderWidth;
            tokenDOM.style.borderRightColor = col;
            tokenDOM.style.borderRightWidth = borderWidth;
        });
        OnTokenDragged.AddListener((token, tokenDOM, targetToken, targetDOM) =>
        {
            var col = new Color(0,0,0);
            var borderWidth = 0f;
            tokenDOM.style.borderLeftColor = col;
            tokenDOM.style.borderLeftWidth = borderWidth;
            tokenDOM.style.borderTopColor = col;
            tokenDOM.style.borderTopWidth = borderWidth;
            tokenDOM.style.borderBottomColor = col;
            tokenDOM.style.borderBottomWidth = borderWidth;
            tokenDOM.style.borderRightColor = col;
            tokenDOM.style.borderRightWidth = borderWidth;
        });
    }
}
