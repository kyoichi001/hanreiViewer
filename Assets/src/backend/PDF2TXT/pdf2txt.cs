using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class pdf2txt : Singleton<pdf2txt>
{
    readonly t01_JustifySentence t01 = new();
    readonly t02_DetectHeader t02 = new();
    readonly t03_SplitSection t03 = new();
    readonly t04_IgnoreHeaderText t04 = new();
    readonly t05_SplitSentence t05 = new();
    readonly t06_AddExtention t06 = new();
    void Export(string jsonRes, string filename)
    {
        StreamWriter wr = new StreamWriter(filename + "__phase1.json", false);
        wr.WriteLine(jsonRes);
        wr.Close();
    }

    public async UniTask ConvertPDF2Txt(string filePath, string outputPath = null)
    {
        var dat = Resources.FindObjectsOfTypeAll<ProgramConfig>();
        if (dat == null || dat.Length == 0)
        {
            Akak.Debug.LogError("Program Config Data not found");
            return;
        }
        string pdfOutputPath = dat[0].pdfOutputPath;
        string josnOutputPath = dat[0].jsonOutputPath;
        string headerRulePath = dat[0].headerRulePath;

        var inputFileName = Path.GetFileNameWithoutExtension(filePath);

        await PDFExtracterWrapper.Instance.Extract(filePath, Application.dataPath + "/" + pdfOutputPath + "/" + inputFileName + "__pdf.json");
        Debug.Log("extract finished");

        StreamReader reader = new StreamReader(Application.dataPath + "/" + pdfOutputPath + "/" + inputFileName + "__pdf.json");
        string datastr = reader.ReadToEnd();
        reader.Close();
        var inputDat = JsonUtility.FromJson<t01_JustifySentence.InputData>(datastr);
        await UniTask.DelayFrame(1);
        Akak.Debug.Log("pdf2txt : phase 1");
        var res1 = t01.Convert(inputDat);
        //Export(JsonUtility.ToJson(res1, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase1.json");
        Akak.Debug.Log("pdf2txt : phase 2");
        var res2 = t02.Convert(res1, Application.streamingAssetsPath + "/" + headerRulePath);
        //Export(JsonUtility.ToJson(res2, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase2.json");
        Akak.Debug.Log("pdf2txt : phase 3");
        var res3 = t03.Convert(res2, Application.streamingAssetsPath + "/" + headerRulePath);
        //Export(JsonUtility.ToJson(res3, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase3.json");
        Akak.Debug.Log("pdf2txt : phase 4");
        var res4 = t04.Convert(res3);
        //Export(JsonUtility.ToJson(res4, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase4.json");
        Akak.Debug.Log("pdf2txt : phase 5");
        var res5 = t05.Convert(res4);
        //Export(JsonUtility.ToJson(res5, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase5.json");
        Akak.Debug.Log("pdf2txt : phase 6");
        var res6 = t06.Convert(res5);

        string json = JsonUtility.ToJson(res6, true);                 // jsonとして変換
        var o = outputPath ?? Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__text.json";
        var wr = new StreamWriter(o, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Close();
        Akak.Debug.Log("pdf2txt : task finished");
    }
}