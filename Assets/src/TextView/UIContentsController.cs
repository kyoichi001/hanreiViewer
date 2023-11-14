using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class UIContentsController : MonoBehaviour
{
    [Header("DOM")]
    [SerializeField] VisualTreeAsset foldableSectionUXML;
    [SerializeField] VisualTreeAsset sectionUXML;
    [SerializeField] VisualTreeAsset bunsetsuUXML;
    [SerializeField] VisualTreeAsset tokenUXML;
    [SerializeField] VisualTreeAsset tokenContextUXML;
    [Header("annotation")]
    [SerializeField] AnnotationLoader annotationLoader;
    [SerializeField] VisualTreeAsset arrowUXML;

    string cullentFilename;
    // Dictionary<(int, int), VisualElement> tokenMap = new Dictionary<(int, int), VisualElement>();
    // Dictionary<VisualElement, (int, int)> domMap = new Dictionary<VisualElement, (int, int)>();

    public class OnTokenMouseOverEvent : UnityEvent<Bunsetsu.Token, VisualElement> { }
    public OnTokenMouseOverEvent OnTokenMouseOver { get; } = new OnTokenMouseOverEvent();
    public class OnTokenMouseOutEvent : UnityEvent<Bunsetsu.Token, VisualElement> { }
    public OnTokenMouseOutEvent OnTokenMouseOut { get; } = new OnTokenMouseOutEvent();
    public class OnTokenClickedEvent : UnityEvent<Bunsetsu.Token, VisualElement> { }
    public OnTokenClickedEvent OnTokenClicked { get; } = new OnTokenClickedEvent();
    public class OnTokenDraggingEvent : UnityEvent<Bunsetsu.Token, VisualElement> { }
    public OnTokenDraggingEvent OnTokenDragging { get; } = new OnTokenDraggingEvent();
    public class OnTokenDraggedEvent : UnityEvent<Bunsetsu.Token, VisualElement, Bunsetsu.Token, VisualElement> { }
    public OnTokenDraggedEvent OnTokenDragged { get; } = new OnTokenDraggedEvent();

    private void Start()
    {
        /*OnTokenDragged.AddListener((token, tokenDOM, target, targetDOM) =>
        {
            if (!domMap.ContainsKey(tokenDOM) || !domMap.ContainsKey(targetDOM))
            {
                Debug.LogError($"key does not found token1:{tokenDOM} token2:{targetDOM}");
                return;
            }
            if (token.id == target.id) return;
            var col = new Color(0, 0, 0);
            var borderWidth = 0f;
            SetBorderColor(tokenDOM, col);
            SetBorderWidth(tokenDOM, borderWidth);
            var textID = domMap[tokenDOM].Item1;
            Debug.Log("add relation");
            annotationLoader.AddRelation(cullentFilename, textID, token.id, target.id, TokenRelationType.None);
        });
        OnTokenDragging.AddListener((token, tokenDOM) =>
        {
            var col = new Color(1, 0.5f, 0.5f);
            var borderWidth = 3f;
            SetBorderColor(tokenDOM, col);
            SetBorderWidth(tokenDOM, borderWidth);
        });*/
    }

    bool dragging = false;
    Bunsetsu.Token currentToken;
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
                    /*foreach (var i in annotationLoader.GetAnnotations(cullentFilename))
                    {
                        var token1 = (i.textID, i.tokenID);
                        var token2 = (i.textID, i.targetID);
                        if (i.textID != text.text_id) continue;
                        if (i.tokenID == token.id || i.targetID == token.id)
                        {
                            HighlightRelation(tokenMap[token1], tokenMap[token2]);
                            break;
                        }
                    }*/
                });
                tokenDOM.RegisterCallback<MouseOutEvent>((type) =>
                {
                    var dom = type.target as VisualElement;
                    OnTokenMouseOut.Invoke(token, dom);
                    /*foreach (var i in annotationLoader.GetAnnotations(cullentFilename))
                    {
                        var token1 = (i.textID, i.tokenID);
                        var token2 = (i.textID, i.targetID);
                        if (i.textID != text.text_id) continue;
                        if (i.tokenID == token.id || i.targetID == token.id)
                        {
                            ResetHighlight(tokenMap[token1], tokenMap[token2]);
                            break;
                        }
                    }*/
                });
                tokenDOM.RegisterCallback<PointerDownEvent>((type) =>
                {
                    //Debug.Log($"token clicked ({type.button})");
                    if (type.button == 0) //left click
                    {
                        var dom = type.target as VisualElement;

                        OnTokenClicked.Invoke(token, dom);
                        dragging = true;
                        currentToken = token;
                        currentTokenDOM = tokenDOM;
                    }
                    else if (type.button == 1) // right click
                    {
                        var contextDOM = tokenContextUXML.CloneTree();
                        contextDOM.Q<Label>("textID").text = text.text_id.ToString();
                        contextDOM.Q<Label>("tokenID").text = token.id.ToString();
                        contextDOM.Q<Label>("tokenText").text = token.text;
                        contextDOM.Q<DropdownField>().value = "None";
                        contextDOM.Q<DropdownField>().choices = new List<string>() {
                            "None","����", "�l��", "�s��" ,"����-N", "�l��-N", "�s��-N" ,
                        };
                        contextDOM.Q<DropdownField>().RegisterValueChangedCallback((e) =>
                        {
                            Debug.Log($"value changed {e.newValue}");
                            var type = e.newValue switch
                            {
                                "None" => TokenTagType.None,
                                _ => TokenTagType.None
                            };
                            AnnotationLoader.Instance.AddTag(cullentFilename, text.text_id, token.id, type);
                        });
                        PopoverManager.Instance.AddPopover(contextDOM, $"{token.text}");
                    }
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
                    if (dragging)
                    {
                        //var dom = type.target as VisualElement; //���ꂾ�Ƃ��܂�Dictionary��key�ƈ�v���Ȃ��B�Q�Ɛ�͓��������ʕ������H�L���X�g���Ă��邩��H
                        OnTokenDragged.Invoke(currentToken, currentTokenDOM, token, tokenDOM);
                        dragging = false;
                        currentToken = null;
                        currentTokenDOM = null;
                    }
                });
                tokenDOM.RegisterCallback<PointerEnterEvent>((type) => { });
                if (text.GetTokenEntity(token.id) == EntityType.Date)
                {
                    tokenDOM.style.backgroundColor = new Color(1, 0, 0, 0.4f);
                }
                bunsetsuDOM.Q<VisualElement>("tokenContainer").Add(tokenDOM);
                //tokenMap.Add((text.text_id, token.id), tokenDOM);
                //domMap.Add(tokenDOM, (text.text_id, token.id));
            }
            root.Add(bunsetsuDOM);
        }
    }

    void GenerateAnnotationArrow(VisualElement root, VisualElement textDOM, int tokenID, int targetID, TokenRelationType type)
    {
        /*var leftPos = new Vector2();
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
        GenerateArrow(root, leftPos, rightPos, 3);*/
    }

    void GenerateMarker(VisualElement root, VisualElement textDOM, int tokenID, int targetID, TokenRelationType type)
    {
        /*var marker = new VisualElement();
        marker.style.backgroundColor = new Color(1, 0, 0);
        foreach (var dom in textDOM.Children())
        {
            if (domMap[dom].Item2 == tokenID)
            {
                marker.transform.position = dom.transform.position;
            }
        }
        root.Add(marker);*/
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
    bool HasAnnotation(SectionText text, List<TokenRelation> annotations)
    {
        foreach (var annotation in annotations)
        {
            if (annotation.textID == text.text_id) return true;
        }
        return false;
    }

    public void generateSectionsDOM(VisualElement root, string filename, List<Section> sections)
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
    void SetBorderColor(VisualElement dom, Color col)
    {
        dom.style.borderLeftColor = col;
        dom.style.borderTopColor = col;
        dom.style.borderBottomColor = col;
        dom.style.borderRightColor = col;
    }
    void SetBorderWidth(VisualElement dom, float width)
    {
        dom.style.borderLeftWidth = width;
        dom.style.borderTopWidth = width;
        dom.style.borderBottomWidth = width;
        dom.style.borderRightWidth = width;
    }
    public void ClearAnnotation()
    {
        /* foreach (var i in tokenMap.Values)
         {
             var col = new Color(0f, 0, 0f,0);
             var borderWidth = 0f;
             SetBorderColor(i, col);
             SetBorderWidth(i, borderWidth);
         }*/
    }
    public void GenerateAnnotation(List<TokenRelation> annotations = null)
    {
        /*Debug.Log($"generate annotation {annotationLoader.GetAnnotations(cullentFilename).Count}");
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
            SetBorderColor(tokenMap[token1], col);
            SetBorderWidth(tokenMap[token1], borderWidth);
            SetBorderColor(tokenMap[token2], col);
            SetBorderWidth(tokenMap[token2], borderWidth);
        }*/
    }
}
