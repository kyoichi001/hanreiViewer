using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPopup : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public float lifeTime = 10;
    float time = 0;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(string text_, float lifeTime_ = 10f)
    {
        var text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = text_;
        lifeTime = lifeTime_;
    }
    void Update()
    {
        if (time > lifeTime)
        {
            Destroy(gameObject);
        }
        if (time > lifeTime * 0.8f)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, (time - lifeTime * 0.8f) / (lifeTime - lifeTime * 0.8f));
        }
        time += Time.deltaTime;
    }
}
