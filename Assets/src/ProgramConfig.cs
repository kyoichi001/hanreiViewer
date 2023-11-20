using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigData", menuName = "ScriptableObjects/ConfigData")]
public class ProgramConfig : ScriptableObject
{
    [Header("pdf2txt")]
    public string pdfOutputPath;
    public string jsonOutputPath;
    public string headerRulePath;
}
