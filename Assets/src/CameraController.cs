using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : SingletonMonoBehaviour<CameraController>
{
    [SerializeField] float scrollSensitivity = 1f;
    [SerializeField] float zoomSensitivity = 1f;
    [SerializeField] float zoomMin = 0;
    [SerializeField] float zoomMax = 2000;

    Vector3 origin;
    Vector3 transformOrigin;
    bool isEnable = false;
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                origin = Input.mousePosition;
                transformOrigin = transform.position;
                isEnable = true;
            }
            else if (isEnable && Input.GetMouseButton(0))
            {
                transform.position = transformOrigin + (Input.mousePosition - origin) * scrollSensitivity;
            }
            else if (isEnable && Input.GetMouseButtonUp(0))
            {
                isEnable = false;
            }
            transform.position += new Vector3(0, 0, Input.mouseScrollDelta.y * zoomSensitivity);
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, zoomMin, zoomMax));
        }
    }
    public async UniTask SetCenter(Vector3 target, float duration = 0.5f)
    {
        var targetPos = new Vector3(target.x, target.y, transform.position.z);
        float t = 0;
        var start = transform.position;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, targetPos, ease(t / duration));
            t += Time.deltaTime;
            await UniTask.DelayFrame(1);
        }
        transform.position = targetPos;
    }

    float ease(float t)
    {
        return Mathf.Pow(t - 1, 3) + 1;
    }
}