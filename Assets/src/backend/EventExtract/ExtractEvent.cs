using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ExtractEvent
{
    readonly t00_01_conbine_bunsetsu t0001 = new();
    readonly t00_02_combine_tango t0002 = new();
    readonly t00_04_replace_serif t0004 = new();
    readonly t01_01_mark_time t0101 = new();
    readonly t01_02_mark_rentaishi t0102 = new();
    readonly t01_03_mark_person t0103 = new();
    readonly t01_04_time_group t0104 = new();
    readonly t02_01_extract_time t0201 = new();
    readonly t02_02_extract_people t0202 = new();
    readonly t02_03_mark_kakari t0203 = new();
    readonly t02_04_extract_act t0204 = new();

    public async UniTask<t02_04_extract_act.OutputData> Extract(string filePath, CancellationToken token)
    {
        var dat = Resources.Load<ProgramConfig>("ConfigData");
        if (dat == null)
        {
            Akak.Debug.LogError("Program Config Data not found");
            return null;
        }
        string timeRulePath = dat.pdfOutputPath;
        string personRulePath = dat.pdfOutputPath;
        Resources.UnloadAsset(dat);
        var inputFileName = Path.GetFileNameWithoutExtension(filePath);

        var res = await HanreiTokenizer.Instance.Tokenize(filePath);
        Debug.Log("extract finished");
        await UniTask.DelayFrame(1);
        Akak.Debug.Log("pdf2txt : phase 1");
        var res1 = t0001.Convert(res);
        //Export(JsonUtility.ToJson(res1, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase1.json");
        Akak.Debug.Log("pdf2txt : phase 2");
        var res2 = t0002.Convert(res1);
        //Export(JsonUtility.ToJson(res2, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase2.json");
        Akak.Debug.Log("pdf2txt : phase 3");
        //var res3 = t0004.Convert(res2);
        //Export(JsonUtility.ToJson(res3, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase3.json");
        Akak.Debug.Log("pdf2txt : phase 4");
        var res4 = t0101.Convert(res2, timeRulePath);
        //Export(JsonUtility.ToJson(res4, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase4.json");
        Akak.Debug.Log("pdf2txt : phase 5");
        var res5 = t0102.Convert(res4);
        //Export(JsonUtility.ToJson(res5, true), Application.dataPath + "/" + josnOutputPath + "/" + inputFileName + "__phase5.json");
        Akak.Debug.Log("pdf2txt : phase 6");
        var res6 = await t0103.Convert(res5, personRulePath, token);
        var res7 = t0104.Convert(res6);
        var res8 = t0201.Convert(res7);
        var res9 = t0202.Convert(res8);
        var res10 = t0203.Convert(res9);
        var res11 = t0204.Convert(res10);
        return res11;
    }
}