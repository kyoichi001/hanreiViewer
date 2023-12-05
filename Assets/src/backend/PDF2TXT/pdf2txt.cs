using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class pdf2txt
{
    static public async UniTask ConvertPDF2Txt(string filePath, string outputPath = null)
    {
        var programConfig = Resources.Load<ProgramConfig>("ConfigData");
        if (programConfig == null)
        {
            Akak.Debug.LogError("Program Config Data not found");
            return;
        }
        string pdfOutputPath = programConfig.pdfOutputPath;
        string josnOutputPath = programConfig.jsonOutputPath;
        string headerRulePath = programConfig.headerRulePath;
        Resources.UnloadAsset(programConfig);

        t01_JustifySentence t01 = new();
        t02_DetectHeader t02 = new();
        t03_SplitSection t03 = new();
        t04_IgnoreHeaderText t04 = new();
        t05_SplitSentence t05 = new();
        t06_AddExtention t06 = new();

        var inputFileName = Path.GetFileNameWithoutExtension(filePath);

        await PDFExtracterWrapper.Instance.Extract(filePath, Application.dataPath + "/" + pdfOutputPath + "/" + inputFileName + "__pdf.json");

        var datastr = "";
        using (var reader = new StreamReader(Application.dataPath + "/" + pdfOutputPath + "/" + inputFileName + "__pdf.json"))
        {
            datastr = await reader.ReadToEndAsync();
        }
        var inputDat = JsonUtility.FromJson<t01_JustifySentence.InputData>(datastr);
        await UniTask.DelayFrame(1);
        var res1 = t01.Convert(inputDat);
        var res2 = t02.Convert(res1, Application.streamingAssetsPath + "/" + headerRulePath);
        var res3 = t03.Convert(res2, Application.streamingAssetsPath + "/" + headerRulePath);
        var res4 = t04.Convert(res3);
        var res5 = t05.Convert(res4);
        var res6 = t06.Convert(res5);

        string json = JsonUtility.ToJson(res6, true);                 // jsonとして変換
        var o = outputPath ?? Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__text.json";
        using (var wr = new StreamWriter(o, false))// ファイル書き込み指定
        {
            await wr.WriteLineAsync(json);// json変換した情報を書き込み
        }
    }
}