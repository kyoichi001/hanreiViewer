using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PopoverManager : MonoBehaviour
{
     UIDocument popoverContainer;
    Dictionary<string, VisualElement> popoverMap;

    private void Awake()
    {
        popoverContainer = GetComponent<UIDocument>();  
    }

    public void AddPopover(VisualTreeAsset popover,string id,Vector2 position)
    {
        var dom = popover.CloneTree();
        dom.style.left=position.x;
        dom.style.top=position.y;
        popoverContainer.rootVisualElement.Add(dom);
        popoverMap[id] = dom;
    }

    public void RemovePopover(string id)
    {
        popoverContainer.rootVisualElement.Remove(popoverMap[id]);
        popoverMap.Remove(id);
    }

}
