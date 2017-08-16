using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    [SerializeField] float pitch = 2f;
    [SerializeField] float zoomSpeed = 4f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 15f;
    [SerializeField] float yawSpeed = 100f;

    private float yawInput = 0f;
    private float currentZoom = 10f;


    void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (Input.GetMouseButton(2))
        {
            yawInput -= Input.GetAxis("Mouse X") * (-1) * yawSpeed * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position - offset * currentZoom;
        transform.LookAt(target.position + Vector3.up * 2f * pitch);
        transform.RotateAround(target.position, Vector3.up, yawInput);
    }
}
