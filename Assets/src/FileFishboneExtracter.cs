using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FileFishboneExtracter : MonoBehaviour
{
    FileDragAndDrop dragAndDrop;
    void Awake()
    {
        dragAndDrop = GetComponent<FileDragAndDrop>();
        dragAndDrop.OnFileDropped.AddListener(async (path) =>
        {
            Akak.Debug.Print($"file dropped {path}");
            await ExtractTextFromPDF(path);
        });
        PDFExtracterWrapper.Instance.OnStandardOut.AddListener((output) =>
        {
            Akak.Debug.Log(output);
        });
    }

    async UniTask ExtractTextFromPDF(string filePath)
    {
        if (Path.GetExtension(filePath) != ".pdf")
        {
            Akak.Debug.PrintWarn("not pdf");
            return;
        }
        Akak.Debug.Log("pdf extracting...");
        var outputPath = Application.dataPath + "\\out.json";
        try
        {
            await PDFExtracterWrapper.Instance.Extract(filePath, outputPath);
            await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);//これ入れないと非同期処理からmainthreadに帰ってくる前に下の文を実行するためエラーになる
        }
        catch (System.Exception e)
        {
            Akak.Debug.PrintError(e.Message);
            throw;
        }
        Akak.Debug.Log("pdf extracted");

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
