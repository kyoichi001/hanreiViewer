using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PopoverManager : MonoBehaviour
{
    UIDocument popoverContainer;
    Dictionary<string, VisualElement> popoverMap = new Dictionary<string, VisualElement>();

    private void Awake()
    {
        popoverContainer = GetComponent<UIDocument>();
    }
    private void Start()
    {
        popoverContainer.rootVisualElement.Q<Button>("background").clicked+=(() =>
        {
            Debug.Log("popover BG clicked");
            ClearPopover();
        });
        popoverContainer.rootVisualElement.Q<Button>("background").pickingMode = PickingMode.Ignore;
        popoverContainer.rootVisualElement.Q<Button>("background").style.display = DisplayStyle.None;
        popoverContainer.rootVisualElement.Q<VisualElement>("container").pickingMode = PickingMode.Ignore;
    }

    public void AddPopover(VisualTreeAsset popover, string id, Vector2? position = null)
    {
        var root = popoverContainer.rootVisualElement.Q<VisualElement>("container");
        var dom = popover.CloneTree();
        if (position.HasValue)
        {
            dom.transform.position = position.Value;
        }
        else
        {
            dom.transform.position = new Vector3(0, 0, 0);
        }
        popoverMap[id] = dom;
        popoverContainer.rootVisualElement.Q<Button>("background").pickingMode = PickingMode.Position;
        popoverContainer.rootVisualElement.Q<Button>("background").style.display = DisplayStyle.Flex;
        root.Add(dom);
    }

    public void RemovePopover(string id)
    {
        var root = popoverContainer.rootVisualElement.Q<VisualElement>("container");
        root.Remove(popoverMap[id]);
        popoverMap.Remove(id);
        if (root.childCount == 0)
        {
            popoverContainer.rootVisualElement.Q<Button>("background").pickingMode = PickingMode.Ignore;
        popoverContainer.rootVisualElement.Q<Button>("background").style.display = DisplayStyle.None;
        }
    }
    void ClearPopover()
    {
        popoverContainer.rootVisualElement.Q<VisualElement>("container").Clear();
        popoverContainer.rootVisualElement.Q<Button>("background").pickingMode = PickingMode.Ignore;
        popoverContainer.rootVisualElement.Q<Button>("background").style.display = DisplayStyle.None;
    }
}
