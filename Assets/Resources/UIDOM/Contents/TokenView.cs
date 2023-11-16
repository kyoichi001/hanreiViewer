using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TokenView : VisualElement
{
    private VisualElement _container;
    private Label _textDOM;
    private string text => _textDOM.text;

    Bunsetsu.Token tokenData;
    int textID;
    string filename;

    public enum TokenUIState
    {
        None,
        Dragging,
    }
    [Flags]
    public enum TokenFlags
    {
        None = 0b_00000000,
        //HasAnnotationTime   = 0b_00000001,
        //HasAnnotationPerson = 0b_00000010,
        //HasAnnotationAct    = 0b_00000100,
        HasAnnotation = 0b_00000111,
        HasEventTime = 0b_00001000,
        HasEventPerson = 0b_00010000,
        HasEventAct = 0b_00100000,
    }
    private TokenFlags flags { get; } = TokenFlags.None;

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription() { name = "text" };
        private UxmlEnumAttributeDescription<TokenFlags> _flags = new UxmlEnumAttributeDescription<TokenFlags>() { name = "flags" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var dom = (TokenView)ve;
            dom._textDOM.text = _text.GetValueFromBag(bag, cc);
            dom.setFlagsDOM(_flags.GetValueFromBag(bag, cc));
        }
    }
    public new class UxmlFactory : UxmlFactory<TokenView, UxmlTraits>
    {

    }

    public TokenView()
    {
        var treeAsset = Resources.Load<VisualTreeAsset>("UIDOM/Contents/tokenUXML");
        var uidom = treeAsset.Instantiate();
        var text = uidom.Q<Label>();
        var button = uidom.Q<VisualElement>("tokenContainer");
        _container = new VisualElement();
        _container.RegisterCallback<PointerDownEvent>((e) =>
        {
            if (e.button == 1)
            {
                OpenContextMenu();
            }
        });
        _textDOM = new Label();
        _container.Add(_textDOM);
        hierarchy.Add(_container);
    }
    public TokenView(string text) : this()
    {
        _textDOM.text = text;
    }


    void OpenContextMenu()
    {

        var contextAsset = Resources.Load<VisualTreeAsset>("UIDOM/tokenContextUXML");
        var contextDOM = contextAsset.CloneTree();
        contextDOM.Q<Label>("textID").text = textID.ToString();
        contextDOM.Q<Label>("tokenID").text = tokenData.id.ToString();
        contextDOM.Q<Label>("tokenText").text = tokenData.text;

        PopoverManager.Instance.AddPopover(contextDOM, $"{tokenData.text}");
    }

    void setFlagsDOM(TokenFlags flags)
    {
        if ((flags & TokenFlags.HasAnnotation) != TokenFlags.None)
        {

        }
    }

}
