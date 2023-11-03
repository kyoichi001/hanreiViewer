using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: 整備
public class ModalManager : MonoBehaviour
{
    [SerializeField] Button bgButton;
    List<Modal> modals = new List<Modal>();//表示するmodalのキュー
    void Awake()
    {
        bgButton.onClick.AddListener(() =>
        {
            CloseModal();
        });
    }
    public void AddModal(Modal m)
    {
        modals.Add(m);
        m.gameObject.SetActive(false);
    }
    public void CloseModal()
    {
        Destroy(modals[0]);
        modals.RemoveAt(0);
        bgButton.gameObject.SetActive(false);
    }
    public void OpenModal()
    {
        bgButton.gameObject.SetActive(true);
        modals[0].gameObject.SetActive(true);
    }
}
