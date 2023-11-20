using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FileTextExtracter : SingletonMonoBehaviour<FileTextExtracter>
{
    public class OnDataLoadedEvent : UnityEvent<HanreiData> { }
    public List<HanreiData> hanreiDatas = new List<HanreiData>();
    public OnDataLoadedEvent OnDataLoaded { get; } = new OnDataLoadedEvent();

    FileDragAndDrop dragAndDrop;
    // Start is called before the first frame update
    void Start()
    {
        dragAndDrop = GetComponent<FileDragAndDrop>();
        dragAndDrop.OnFileDropped.AddListener(async (path) =>
        {
            Akak.Debug.Log($"file dropped {path}");
            try
            {
                await ExtractTextFromPDF(path);
                await TokenizeText();
                var data = Convert();
                Akak.Debug.Log("text converted");
                OnDataLoaded.Invoke(data);
            }
            catch (Exception e)
            {
                await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);//これ入れないと非同期処理からmainthreadに帰ってくる前に下の文を実行するためエラーになる
                Akak.Debug.LogError(e.Message);
            }
        });
    }
    async UniTask ExtractTextFromPDF(string filePath)
    {
        if (Path.GetExtension(filePath) != ".pdf")
        {
            Akak.Debug.LogError("not pdf");
            return;
        }
        Akak.Debug.Log("pdf extracting...");
        var outputPath = Application.dataPath + "\\out.json";
        try
        {
            var inputFileName = Path.GetFileNameWithoutExtension(filePath);
            await pdf2txt.Instance.ConvertPDF2Txt(filePath, Application.dataPath + "/" + "out2.json");
        }
        catch (System.Exception e)
        {
            await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);//これ入れないと非同期処理からmainthreadに帰ってくる前に下の文を実行するためエラーになる
            Akak.Debug.LogError(e.Message);
            throw;
        }
        await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);//これ入れないと非同期処理からmainthreadに帰ってくる前に下の文を実行するためエラーになる
        Akak.Debug.Log("pdf extracted");
    }
    async UniTask TokenizeText()
    {
        var filePath = Application.dataPath + "\\out2.json";
        Akak.Debug.Log("tokenizing");
        var tokenizedData = await HanreiTokenizer.Instance.Tokenize(filePath);
        await Cysharp.Threading.Tasks.UniTask.DelayFrame(1);//これ入れないと非同期処理からmainthreadに帰ってくる前に下の文を実行するためエラーになる
        Akak.Debug.Log("tokenized");
        string json = JsonUtility.ToJson(tokenizedData, true);                 // jsonとして変換
        var outputPath = Application.dataPath + "/" + "sample_tokenized.json";
        StreamWriter wr = new StreamWriter(outputPath, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Close();
    }

    HanreiData Convert()
    {
        var pdfPath = Application.dataPath + "/" + "out2.json";
        var dataPath = Application.dataPath + "/" + "sample_tokenized.json";
        var data = LoadData(pdfPath, dataPath);
        return data;
    }
    HanreiData LoadData(string extractedPDFPath, string tokenizedfilePath)
    {
        StreamReader reader = new StreamReader(tokenizedfilePath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        StreamReader reader2 = new StreamReader(extractedPDFPath);
        string datastr2 = reader2.ReadToEnd();
        reader2.Close();
        var dat = JsonUtility.FromJson<HanreiTokenizer.OutputData>(datastr);
        var d = JsonUtility.FromJson<t06_AddExtention.OutputData>(datastr2);

        if (dat == null)
        {
            Akak.Debug.LogWarn("tokenized data not loaded");
            return null;
        }
        if (d == null)
        {
            Akak.Debug.LogWarn("pdf data not loaded");
            return null;
        }
        var res = new HanreiData();
        res.filename = "sample";
        res.contents.signature.header_text = d.signature.header_text;
        res.contents.signature.texts = d.signature.contents;
        res.contents.judgement.header_text = d.judgement.header_text;
        res.contents.judgement.texts = d.judgement.contents;
        res.contents.main_text.header_text = d.mainText.header_text;
        res.contents.fact_reason.header_text = d.factReason.header_text;
        foreach (var i in d.factReason.sections)
        {
            var v = new Section
            {
                type = i.type,
                header = i.header,
                //header_text=i.header_text,
                indent = i.indent,
            };
            foreach (var j in i.texts)
            {
                var tokenizedD = dat.datas.Find((d) => d.text_id == j.text_id);
                if (tokenizedD == null)
                {
                    Akak.Debug.LogWarn($"text_id not found {j.text_id}");
                    continue;
                }
                var sectionText = new SectionText
                {
                    text_id = j.text_id,
                    raw_text = j.raw_text,
                };
                foreach (var k in tokenizedD.bunsetsu)
                {
                    var bunset = new Bunsetsu
                    {
                        id = k.id,
                        text = k.text
                    };
                    foreach (var l in k.tokens)
                    {
                        bunset.tokens.Add(new Bunsetsu.Token
                        {
                            text = l.text,
                            tag = l.tag
                        });
                    }
                    sectionText.bunsetu.Add(bunset);
                }
                v.texts.Add(sectionText);
            }
            res.contents.fact_reason.sections.Add(v);
        }
        return res;
    }


}
