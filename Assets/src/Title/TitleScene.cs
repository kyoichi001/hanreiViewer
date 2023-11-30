using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEditor;

public class TitleScene : SingletonMonoBehaviour<TitleScene>
{
    [SerializeField] string hanreiViewerName;
    [SerializeField] string fishboneViewerName;

    public void ToHanreiViewer()
    {
        SceneManager.LoadScene(hanreiViewerName);

    }
    public void ToFishboneViewer()
    {
        SceneManager.LoadScene(fishboneViewerName);

    }

    [System.Serializable]
    public class OnSaveDataLoadedEvent : UnityEvent<List<string>> { }
    public OnSaveDataLoadedEvent OnSaveDataLoaded { get; } = new OnSaveDataLoadedEvent();
    [System.Serializable]
    public class OnFileLoadedEvent : UnityEvent<string> { }
    public OnFileLoadedEvent OnFileLoaded { get; } = new OnFileLoadedEvent();
    SaveDataIO saveDataIO;
    SaveData saveFile;
    void Awake()
    {
        // saveDataIO = GetComponent<SaveDataIO>();
    }

    void Start()
    {
        //saveFile = saveDataIO.Load();
        //OnSaveDataLoaded.Invoke(saveFile.recentFiles);
    }
    private void OnApplicationQuit()
    {
        //saveDataIO.Save(saveFile);
    }
    public void AddFile(string path)
    {
        saveFile.recentFiles.Add(path);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
