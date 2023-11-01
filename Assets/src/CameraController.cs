using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float scrollSensitivity = 1f;
    [SerializeField] float zoomSensitivity = 1f;
    [SerializeField] float zoomMin = 0;
    [SerializeField] float zoomMax = 2000;

    Vector3 origin;
    Vector3 transformOrigin;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            origin = Input.mousePosition;
            transformOrigin = transform.position;
        }
        else if (Input.GetMouseButton(0))
        {
            transform.position = transformOrigin + (Input.mousePosition - origin) * scrollSensitivity;
        }

        transform.position += new Vector3(0, 0, Input.mouseScrollDelta.y * zoomSensitivity);
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, zoomMin, zoomMax));
    }
}
