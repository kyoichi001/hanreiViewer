using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.Events;
using System.Linq;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class CaboChaRes
{
    [System.Serializable]
    public class CaboChaToken
    {
        public int id;
        public string text;
        public string tag;
    }
    [System.Serializable]
    public class CaboChaBunsetsu
    {
        public List<CaboChaToken> tokens = new List<CaboChaToken>();
        public string text;
        public int id;
        public int to;
    }
    public List<CaboChaBunsetsu> bunsetsus = new List<CaboChaBunsetsu>();
}

public class CabochaWrapper : SingletonMonoBehaviour<CabochaWrapper>
{
    private static readonly string FolderPath = Application.streamingAssetsPath;
    private static readonly string FilePath = FolderPath + "/CaboCha/bin/cabocha.exe";

    private Process _process;

    List<string> buffer = new List<string>();
    public class OnTextTokenizedEvent : UnityEvent<CaboChaRes> { }
    OnTextTokenizedEvent OnTextTokenized = new OnTextTokenizedEvent();

    /*private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UnityEngine.Debug.Log("sample text analyzing");
            var res = await CaboCha("今日は良い天気だ");
            await UniTask.DelayFrame(1);//これ入れないと非同期処理からmainthreadに帰ってくる前に下の文を実行するためエラーになる
            foreach (var i in res.bunsetsus)
            {
                Akak.Debug.Print($"{i.text} id:{i.id} to:{i.to}");
                foreach (var j in i.tokens)
                {
                    Akak.Debug.Print($"{j.text} tag:{j.tag}");
                }
            }
        }
    }*/

    private void OnDestroy()
        => DisposeProcess();

    private void OnStandardOut(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != "EOS")
        {
            buffer.Add(e.Data);
        }
        else
        {
            var res = ConvertRes(buffer);
            OnTextTokenized.Invoke(res);
            buffer.Clear();
        }
    }

    private void DisposeProcess(object sender, EventArgs e)
        => DisposeProcess();

    private void DisposeProcess()
    {
        if (_process == null || _process.HasExited)
        {
            UnityEngine.Debug.Log("cabocha wrapper 異常終了");
            OnTextTokenized.Invoke(null);
            return;
        }

        _process.StandardInput.Close();
        _process.CloseMainWindow();
        _process.Dispose();
        _process = null;
    }
    public void StartProcess()
    {
        _process = new Process();

        // プロセスを起動するときに使用する値のセットを指定
        _process.StartInfo = new ProcessStartInfo
        {
            FileName = FilePath,                        // 起動するファイルのパスを指定する
            UseShellExecute = false,                    // プロセスの起動にオペレーティング システムのシェルを使用するかどうか(既定値:true)
            WorkingDirectory = FolderPath,              // 開始するプロセスの作業ディレクトリを取得または設定する(既定値:"")
            RedirectStandardInput = true,               // StandardInput から入力を読み取る(既定値：false)
            RedirectStandardOutput = true,              // 出力を StandardOutput に書き込むかどうか(既定値：false)
            CreateNoWindow = false,                      // プロセス用の新しいウィンドウを作成せずにプロセスを起動するかどうか(既定値：false)
            Arguments = "-f1"
        };
        // 外部プロセスのStandardOutput ストリームに行を書き込む度に発火されるイベント
        _process.OutputDataReceived += OnStandardOut;

        //外部プロセスの終了を検知する
        _process.EnableRaisingEvents = true;
        _process.Exited += DisposeProcess;

        // プロセスを起動する
        _process.Start();
        _process.BeginOutputReadLine();
    }
    public void EndProcess()
    {
        if (_process == null) return;
        _process.Close();
        _process = null;
    }
    public async UniTask<CaboChaRes> CaboCha(string text)
    {
        _process?.StandardInput.WriteLine(text);
        var tcs = new UniTaskCompletionSource<CaboChaRes>();
        OnTextTokenized.RemoveAllListeners();
        OnTextTokenized.AddListener((res) =>
        {
            tcs.TrySetResult(res);
        });
        var result = await tcs.Task;
        UnityEngine.Debug.Log("converted");
        return result;
    }
    CaboChaRes ConvertRes(List<string> buffer_)
    {
        var res = new CaboChaRes();
        var id = 0;
        //UnityEngine.Debug.Log($"converting {string.Join(',', buffer_)}");
        foreach (var i in buffer_)
        {
            if (i[0] == '*')
            {
                res.bunsetsus.Add(new CaboChaRes.CaboChaBunsetsu());
                var a1 = i.Split(" ");
                res.bunsetsus[^1].id = int.Parse(a1[1]);
                res.bunsetsus[^1].to = int.Parse(a1[2].Replace("D", ""));
            }
            else
            {
                var a = i.Split("\t");
                res.bunsetsus[^1].tokens.Add(new CaboChaRes.CaboChaToken());
                res.bunsetsus[^1].tokens[^1].id = id;
                res.bunsetsus[^1].tokens[^1].text = a[0];
                res.bunsetsus[^1].text += a[0];
                var b = a[1].Split(",")[0..4];
                res.bunsetsus[^1].tokens[^1].tag = string.Join("-", b.Where((v) => v != "*"));
                id++;
            }
        }
        return res;
    }

}
