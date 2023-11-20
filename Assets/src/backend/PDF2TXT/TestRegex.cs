using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TestRegex : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = Regex.Match("(原告の主張)", "^(\\(|【|〔)(被|原)告.*の主張(\\)|】|〕)$");
        Debug.Log($"test regex matches: {a.Success}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
