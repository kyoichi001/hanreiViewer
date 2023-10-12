using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScroll : MonoBehaviour
{
    //https://note.com/what_is_picky/n/nf9b5dca6e5b6

    [SerializeField] Transform origin;
    [SerializeField] float scaleFactor = 1;
    [SerializeField] float scrollSensitivity = 1f;
    private const string k_propName = "_MainTex";
    private Material m_material;
    void Awake()
    {
        if (GetComponent<Image>() is Image i)
        {
            Shader sh = i.material.shader;
            m_material = new Material(sh);
            i.material = m_material;
            //m_material = i.material;
        }
    }
    void Update()
    {
        var scale = origin.lossyScale * scaleFactor;
        m_material.SetTextureScale(k_propName, new Vector2(1 / scale.x, 1 / scale.y));
        m_material.SetTextureOffset(k_propName, origin.localPosition * scrollSensitivity);
    }
    public void SetOffset(Vector2 offset)
    {
        m_material.SetTextureOffset(k_propName, offset);
    }
    private void OnDestroy()
    {
        // ゲームをやめた後にマテリアルのOffsetを戻しておく
        if (m_material)
        {
            m_material.SetTextureOffset(k_propName, Vector2.zero);
        }
    }
}
