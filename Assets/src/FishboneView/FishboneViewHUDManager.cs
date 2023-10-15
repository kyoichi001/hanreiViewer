using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishboneViewHUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMPro.TextMeshProUGUI dataName;
    [SerializeField] Button backButton;
    [SerializeField] GameObject hanreiSelectorButtonPrefab;
    [SerializeField] RectTransform hanreiSelector;

    // Start is called before the first frame update
    void Awake()
    {
        FishboneViewManager.Instance.OnFilenamesLoaded.AddListener((filenames) =>
        {
            foreach (var (hanreiTitle, path) in filenames)
            {
                var obj = Instantiate(hanreiSelectorButtonPrefab, hanreiSelector);
                obj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = hanreiTitle;
                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    FishboneViewManager.Instance.ShowFishboneUI(path);
                });
            }
        });
        FishboneViewManager.Instance.OnShowData.AddListener((path, data) =>
        {
            dataName.text = path;
        });
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data_) =>
        {
            Debug.Log($"path : {path}");
            var arr = path.Split(new char[] { '/', '\\' });
            var filename = arr[arr.Length - 1];
        });

        backButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Title");
        });
    }
}
