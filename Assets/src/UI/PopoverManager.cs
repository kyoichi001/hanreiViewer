using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PopoverManager : MonoBehaviour
{
     UIDocument popoverContainer;
    Dictionary<string, VisualElement> popoverMap=new Dictionary<string, VisualElement>();

    private void Awake()
    {
        popoverContainer = GetComponent<UIDocument>();  
    }

    public void AddPopover(VisualTreeAsset popover,string id,Vector2? position=null)
    {
        var dom = popover.CloneTree();
        if(position.HasValue)
        {
            dom.transform.position = position.Value;
        }
        else
        {
            dom.transform.position = new Vector3(0,0,0);
        }
        popoverContainer.rootVisualElement.Add(dom);
        popoverMap[id] = dom;
    }

    public void RemovePopover(string id)
    {
        popoverContainer.rootVisualElement.Remove(popoverMap[id]);
        popoverMap.Remove(id);
    }

}
