using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FileFishboneExtracter : MonoBehaviour
{
    FileDragAndDrop dragAndDrop;
    PDFExtracterWrapper pdfExtracterWrapper;
    void Awake()
    {
        dragAndDrop = GetComponent<FileDragAndDrop>();
        pdfExtracterWrapper = GetComponent<PDFExtracterWrapper>();
        dragAndDrop.OnFileDropped.AddListener(async (path) =>
        {
            DialogPopupManager.Instance.Print($"file dropped {path}");
            await ExtractTextFromPDF(path);
        });
        pdfExtracterWrapper.OnStandardOut.AddListener((output) =>
        {
            UnityEngine.Debug.Log(output);
            DialogPopupManager.Instance.Print(output);
        });
    }

    async UniTask ExtractTextFromPDF(string filePath)
    {
        if (Path.GetExtension(filePath) != ".pdf")
        {
            DialogPopupManager.Instance.Print("not pdf");
            return;
        }
        DialogPopupManager.Instance.Print("pdf extracting...");
        var outputPath = Application.dataPath + "\\out.json";
        try
        {
            await pdfExtracterWrapper.Extract(filePath, outputPath);
            await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);//これ入れないと非同期処理からmainthreadに帰ってくる前に下の文を実行するためエラーになる
        }
        catch (System.Exception e)
        {
            DialogPopupManager.Instance.Print(e.Message);
            throw;
        }
        DialogPopupManager.Instance.Print("pdf extracted");

    }

    async void Update()
    {

        if (Input.GetKeyDown(KeyCode.D))
        {
            UnityEngine.Debug.Log("sample pdf extracting");
            await ExtractTextFromPDF(@"C:\Users\tanso\Downloads\2023ETver8_田中.pdf");
            UnityEngine.Debug.Log("sample pdf extracted");
        }
    }

}
