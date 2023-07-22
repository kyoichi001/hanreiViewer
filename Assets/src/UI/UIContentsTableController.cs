using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIContentsTableController : MonoBehaviour
{
    [Header("DOM")]
    [SerializeField] VisualTreeAsset foldableSectionUXML;
    [SerializeField] VisualTreeAsset sectionUXML;

    bool hasChild(Section target, Section next)
    {
        return next.indent == target.indent + 1;
    }
   public void generateTableOfContentDOM(VisualElement root, List<Section> sections)
    {
        var indentContainer = new List<VisualElement> { root };
        for (var i = 0; i < sections.Count - 1; i++)
        {
            var sectionData = sections[i];
            if (hasChild(sectionData, sections[i + 1]))
            {
                //セクションが子要素を含むならfoldoutにする
                var section = foldableSectionUXML.CloneTree();
                section.Q<Foldout>("header").text = sectionData.header + "  " + sectionData?.header_text;
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
            else if (sectionData.header_text?.Length > 0)
            {
                var section = sectionUXML.CloneTree();
                section.Q<Label>("header").text = sectionData.header + sectionData.header_text;
                section.Q<VisualElement>("sectionContainer").Remove(section.Q<VisualElement>("rightContainer"));
                indentContainer[sectionData.indent - 1].Add(section);
            }
        }
        var sectionDat = sections[sections.Count - 1];
        var sectionDOM = sectionUXML.CloneTree();
        sectionDOM.Q<Label>("header").text = sectionDat.header + sectionDat.header_text;
        sectionDOM.Q<VisualElement>("sectionContainer").Remove(sectionDOM.Q<VisualElement>("rightContainer"));
        indentContainer[sectionDat.indent - 1].Add(sectionDOM);
    }

}
