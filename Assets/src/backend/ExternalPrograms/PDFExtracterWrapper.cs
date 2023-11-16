using System.Diagnostics;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PDFExtracterWrapper : Singleton<PDFExtracterWrapper>
{
    private static readonly string FolderPath = Application.streamingAssetsPath;
    private static readonly string FilePath = FolderPath + "/pdf2txt/pdf2txt.exe";

    private Process _process;

    public class OnStandardOutEvent : UnityEvent<string> { }
    public OnStandardOutEvent OnStandardOut { get; } = new OnStandardOutEvent();

    public async UniTask Extract(string filePath, string outPutPath)
    {
        if (!File.Exists(FilePath))
        {
            await UniTask.DelayFrame(1);
            Akak.Debug.PrintError("pdf2txt not found");
            return;
        }
        if (!File.Exists(filePath))
        {
            await UniTask.DelayFrame(1);
            Akak.Debug.PrintError($"target file:{filePath} not found");
            return;
        }
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
            CreateNoWindow = true,                      // プロセス用の新しいウィンドウを作成せずにプロセスを起動するかどうか(既定値：false)
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
                //DialogPopupManager.Instance.Print($"pdf extracter 異常終了 {_process == null} {_process.HasExited} {_process.ExitCode}");
                tcs.TrySetResult();
                return;
            }
            _process.StandardInput.Close();
            _process.CloseMainWindow();
            _process.Dispose();
            _process = null;
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
