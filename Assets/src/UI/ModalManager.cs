using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: 整備
public class ModalManager : SingletonMonoBehaviour<ModalManager>
{
    [SerializeField] RectTransform modalContainer;
    [SerializeField] Button bgButton;
    List<Modal> modals = new List<Modal>();//表示するmodalのキュー
    void Awake()
    {
        bgButton.onClick.AddListener(() =>
        {
            CloseModal();
        });
    }
    public GameObject AddModal(GameObject m)
    {
        var obj = Instantiate(m, modalContainer);
        modals.Add(obj.GetComponent<Modal>());
        modals[^1].OnClose.AddListener(() =>
        {
            CloseModal();
        });
        obj.gameObject.SetActive(false);
        OpenModal();
        return obj;
    }
    public void CloseModal()
    {
        Destroy(modals[0].gameObject);
        modals.RemoveAt(0);
        bgButton.gameObject.SetActive(false);
    }
    public void OpenModal()
    {
        bgButton.gameObject.SetActive(true);
        modals[0].gameObject.SetActive(true);
    }
}
