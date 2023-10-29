using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPopupManager : SingletonMonoBehaviour<DialogPopupManager>
{
    public GameObject DialogPrefab;
    public void Print(string text, Color color)
    {
        var s = Instantiate(DialogPrefab, transform).GetComponent<DialogPopup>();
        s.Init(text, color);
    }
}
