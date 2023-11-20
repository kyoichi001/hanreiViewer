using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroll : MonoBehaviour
{
    [SerializeField] Transform origin;
    [SerializeField] float scaleFactor = 1;
    [SerializeField] float scrollSensitivity = 1f;
    SpriteRenderer sprite;
    private const string k_propName = "_MainTex";
    private Material m_material;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Shader sh = sprite.material.shader;
        m_material = new Material(sh);
        sprite.material = m_material;
    }
    // Update is called once per frame
    void Update()
    {
        var scale = origin.lossyScale * scaleFactor;
        m_material.SetTextureScale(k_propName, new Vector2(1 / scale.x, 1 / scale.y));
        m_material.SetTextureOffset(k_propName, origin.localPosition * scrollSensitivity);
    }
}
