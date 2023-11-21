using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 子・孫UIに対して一括で色を一時的にのせる（エフェクト用）
/// </summary>
public class UIColorSet : MonoBehaviour
{
    Dictionary<Image, Color> images = new();

    public async UniTask SetColorEffect(Color color, float duration = 0.5f)
    {
        var uuu = gameObject.GetComponentsInChildren<Image>();
        foreach (var i in uuu)
        {
            images[i] = i.color;
        }
        float t = 0;
        while (t < duration / 2)
        {
            foreach (var i in images)
            {
                i.Key.color = Color.Lerp(i.Value, color, ease(t / (duration / 2)));
            }
            t += Time.deltaTime;
            await UniTask.DelayFrame(1);
        }
        t = 0;
        while (t < duration / 2)
        {
            foreach (var i in images)
            {
                i.Key.color = Color.Lerp(color, i.Value, ease(t / (duration / 2)));
            }
            t += Time.deltaTime;
            await UniTask.DelayFrame(1);
        }
    }

    float ease(float t)
    {
        return Mathf.Pow(t - 1, 3) + 1;
    }
}
