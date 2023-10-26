using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;
using UnityEngine.Events;
//https://github.com/Bunny83/UnityWindowsFileDrag-Drop

public class FileDragAndDrop : MonoBehaviour
{
    public class OnFileDroppedEvent : UnityEvent<string> { }
    public OnFileDroppedEvent OnFileDropped { get; } = new OnFileDroppedEvent();

    void OnEnable()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();

        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        foreach (var f in aFiles)
            OnFileDropped.Invoke(f);
    }
}
