using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Modal : MonoBehaviour
{

    public UnityEvent OnClose { get; } = new UnityEvent();
    [SerializeField] Button closeButton;
    void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(() => Close());
    }

    public void Close()
    {
        OnClose.Invoke();
    }
    public void Open()
    {

    }
}
