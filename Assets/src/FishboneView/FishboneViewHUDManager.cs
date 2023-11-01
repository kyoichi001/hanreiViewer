using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishboneViewHUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button backButton;
    [SerializeField] GameObject hanreiSelectorButtonPrefab;
    [SerializeField] RectTransform hanreiSelector;
    [SerializeField] GameObject fishboneHUD;

    UIHanreiSelectorButton currentActiveButton = null;
    // Start is called before the first frame update
    void Awake()
    {
        FishboneViewManager.Instance.OnFilenamesLoaded.AddListener((filenames) =>
        {
            foreach (var (hanreiTitle, path) in filenames)
            {
                var obj = Instantiate(hanreiSelectorButtonPrefab, hanreiSelector).GetComponent<UIHanreiSelectorButton>();
                obj.Init(hanreiTitle);
                obj.OnShowFishbone.AddListener(() =>
                {
                    currentActiveButton?.Deactivate();
                    FishboneViewManager.Instance.ShowFishboneUI(path);
                    obj.Activate();
                    currentActiveButton = obj;
                });
                obj.OnShowTextview.AddListener(() =>
                {

                });
            }
        });
        fishboneHUD.SetActive(false);
        FishboneViewManager.Instance.OnShowData.AddListener((path, data) =>
        {
            fishboneHUD.SetActive(true);
        });
        EventDataLoader.Instance.OnDataLoaded.AddListener((path, data_) =>
        {
            Debug.Log($"path : {path}");
            var arr = path.Split(new char[] { '/', '\\' });
            var filename = arr[^1];
        });

        backButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Title");
        });
    }
}
