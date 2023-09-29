using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishboneViewHUDManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI dataName;
    [SerializeField] Button backButton;

    // Start is called before the first frame update
    void Awake()
    {
        FishboneViewManager.Instance.OnDataLoaded.AddListener((data) =>
        {
            dataName.text = data.filename;
        });
        backButton.onClick.AddListener(() =>
        {

        });
    }
}
