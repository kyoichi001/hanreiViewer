using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataIO : MonoBehaviour
{
    [SerializeField] string fileName = "Save/saveData.json";
    [Header("Debug")]
    [SerializeField, ReadOnly] string filePath_;
    void Awake()
    {
        filePath_ = Application.dataPath + "/" + fileName;
    }
    public SaveData Load()
    {
        var path = Application.dataPath + "/" + fileName;
        if (!File.Exists(path))
        {
            return new SaveData();
        }
        using (StreamReader reader = new StreamReader(path, System.Text.Encoding.GetEncoding("utf-8")))
        {
            string datastr = reader.ReadToEnd();
            return JsonUtility.FromJson<SaveData>(datastr);
        }
    }
    public void Save(SaveData data)
    {
        Debug.Log("Write SaveData...", gameObject);
        if (!Directory.Exists(Application.dataPath + "/" + "Save"))
        {
            Directory.CreateDirectory(Application.dataPath + "/" + "Save");
        }
        var path = Application.dataPath + "/" + fileName;
        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.GetEncoding("utf-8")))
        {
            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
            writer.Write(json);
        }
    }
}
