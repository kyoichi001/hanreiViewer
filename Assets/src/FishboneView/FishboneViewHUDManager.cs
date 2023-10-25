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

    // Start is called before the first frame update
    void Awake()
    {
        FishboneViewManager.Instance.OnFilenamesLoaded.AddListener((filenames) =>
        {
            foreach (var (hanreiTitle, path) in filenames)
            {
                var obj = Instantiate(hanreiSelectorButtonPrefab, hanreiSelector);
                var buttons = obj.GetComponentsInChildren<Button>();
                buttons[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = hanreiTitle;

                //fishboneの表示
                buttons[0].onClick.AddListener(() =>
                {
                    FishboneViewManager.Instance.ShowFishboneUI(path);
                });
                //textviewの表示
                buttons[1].onClick.AddListener(() =>
                {

                });
            }
        });
        FishboneViewManager.Instance.OnShowData.AddListener((path, data) =>
        {
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
