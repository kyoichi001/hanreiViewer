using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Windows.Forms; //OpenFileDialog用に使う

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button toHanreiViewerButton;
    [SerializeField] UnityEngine.UI.Button toFishboneViewerButton;

    [Header("Path")]
    [SerializeField] GameObject pathPrefab;
    [SerializeField] RectTransform pathesContainer;
    [SerializeField] UnityEngine.UI.Button pathAddButton;
    void Awake()
    {
        toHanreiViewerButton.onClick.AddListener(() =>
        {
            TitleScene.Instance.ToHanreiViewer();
        });
        toFishboneViewerButton.onClick.AddListener(() =>
        {
            TitleScene.Instance.ToFishboneViewer();
        });
        TitleScene.Instance.OnSaveDataLoaded.AddListener((pathes) =>
        {
            foreach (var path in pathes)
            {
                var obj = Instantiate(pathPrefab, pathesContainer).GetComponent<TitleDataFileUI>();
                obj.Init(path);
            }
        });
        pathAddButton.onClick.AddListener(() =>
        {
            OpenDialog();
        });
    }
    void OpenDialog()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        openFileDialog.Title = "ファイルを選択";
        openFileDialog.Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string selectedFilePath = openFileDialog.FileName;

            // ここでファイルを操作したり、選択されたファイルを開く処理を実行できます
        }
    }


}
