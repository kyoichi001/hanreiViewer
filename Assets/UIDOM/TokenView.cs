using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TokenView : VisualElement
{
    private VisualElement _container;
    private Label _text;
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription() { name = "text" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var dom = (TokenView)ve;
            dom._text.text = _text.GetValueFromBag(bag, cc);
        }
    }
    public new class UxmlFactory : UxmlFactory<TokenView, UxmlTraits>
    {
    }

    public TokenView()
    {
        _container = new VisualElement();
        _text = new Label();
        _container.Add(_text);
        hierarchy.Add(_container);
    }
    public TokenView(string text):this()
    {
        _text.text = text;
    }
}
