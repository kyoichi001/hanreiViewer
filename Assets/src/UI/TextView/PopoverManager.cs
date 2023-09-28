using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PopoverManager : SingletonMonoBehaviour<PopoverManager>
{
    UIDocument popoverContainer;
    Dictionary<string, VisualElement> popoverMap = new Dictionary<string, VisualElement>();

    private void Awake()
    {
        popoverContainer = GetComponent<UIDocument>();
    }
    private void Start()
    {
        popoverContainer.rootVisualElement.Q<VisualElement>("background").RegisterCallback<PointerDownEvent>((e) =>
        {
            Debug.Log("popover BG clicked");
            ClearPopover();
        });
        popoverContainer.rootVisualElement.Q<VisualElement>("background").pickingMode = PickingMode.Ignore;
        popoverContainer.rootVisualElement.Q<VisualElement>("background").style.display = DisplayStyle.None;
        popoverContainer.rootVisualElement.Q<VisualElement>("container").pickingMode = PickingMode.Ignore;
    }

    public void AddPopover(VisualElement popover, string id, Vector2? position = null)
    {
        var root = popoverContainer.rootVisualElement.Q<VisualElement>("container");
        var dom = popover;
        if (position.HasValue)
        {
            dom.transform.position = position.Value;
        }
        else
        {
            var pos = Input.mousePosition;
            dom.transform.position = new Vector3(pos.x,Screen.height- pos.y);
        }
        popoverMap[id] = dom;
        popoverContainer.rootVisualElement.Q<VisualElement>("background").pickingMode = PickingMode.Position;
        popoverContainer.rootVisualElement.Q<VisualElement>("background").style.display = DisplayStyle.Flex;
        root.Add(dom);
    }

    public void RemovePopover(string id)
    {
        var root = popoverContainer.rootVisualElement.Q<VisualElement>("container");
        root.Remove(popoverMap[id]);
        popoverMap.Remove(id);
        if (root.childCount == 0)
        {
            popoverContainer.rootVisualElement.Q<VisualElement>("background").pickingMode = PickingMode.Ignore;
        popoverContainer.rootVisualElement.Q<VisualElement>("background").style.display = DisplayStyle.None;
        }
    }
    void ClearPopover()
    {
        popoverContainer.rootVisualElement.Q<VisualElement>("container").Clear();
        popoverContainer.rootVisualElement.Q<VisualElement>("background").pickingMode = PickingMode.Ignore;
        popoverContainer.rootVisualElement.Q<VisualElement>("background").style.display = DisplayStyle.None;
    }
}
