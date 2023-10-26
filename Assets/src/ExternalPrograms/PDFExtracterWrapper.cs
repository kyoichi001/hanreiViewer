using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class PDFExtracterWrapper : MonoBehaviour
{
    private static readonly string FolderPath = Application.streamingAssetsPath;
    private static readonly string FilePath = FolderPath + "/pdf_extracter/pdf_extracter.exe";

    private Process _process;

    public class OnStandardOutEvent : UnityEvent<string> { }
    public OnStandardOutEvent OnStandardOut { get; } = new OnStandardOutEvent();


    public async UniTask Extract(string filePath, string outPutPath)
    {
        var tcs = new UniTaskCompletionSource();
        _process = new Process();
        // プロセスを起動するときに使用する値のセットを指定
        _process.StartInfo = new ProcessStartInfo
        {
            FileName = FilePath,                        // 起動するファイルのパスを指定する
            UseShellExecute = false,                    // プロセスの起動にオペレーティング システムのシェルを使用するかどうか(既定値:true)
            WorkingDirectory = FolderPath,              // 開始するプロセスの作業ディレクトリを取得または設定する(既定値:"")
            RedirectStandardInput = false,               // StandardInput から入力を読み取る(既定値：false)
            RedirectStandardOutput = true,              // 出力を StandardOutput に書き込むかどうか(既定値：false)
            RedirectStandardError = true,
            CreateNoWindow = false,                      // プロセス用の新しいウィンドウを作成せずにプロセスを起動するかどうか(既定値：false)
            Arguments = $"\"{filePath}\" \"{outPutPath}\""
        };
        // 外部プロセスのStandardOutput ストリームに行を書き込む度に発火されるイベント
        _process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        {
            OnStandardOut.Invoke(e.Data);
        };
        _process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
        {
            UnityEngine.Debug.Log(sender);
            OnStandardOut.Invoke(e.Data);
        };
        //外部プロセスの終了を検知する
        _process.EnableRaisingEvents = true;
        _process.Exited += (object sender, System.EventArgs e) =>
        {
            if (_process == null || _process.HasExited)
            {
                //var msg = _process.StandardOutput.ReadToEnd();
                UnityEngine.Debug.Log($"pdf extracter 異常終了 {_process == null} {_process.HasExited} {_process.ExitCode}");
                tcs.TrySetResult();
                return;
            }
            _process.StandardInput.Close();
            _process.CloseMainWindow();
            _process.Dispose();
            _process = null;
            UnityEngine.Debug.Log("pdf extracter ended");
            tcs.TrySetResult();
        };
        // プロセスを起動する
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        await tcs.Task;
        return;
    }


}
