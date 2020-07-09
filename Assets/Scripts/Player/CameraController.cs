using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public float offsetSensitivity = 0.2f;
    public float followSpeed = 0.2f;

    [HideInInspector] public Camera cam;
    Transform target;
    Vector3 startingCameraOffset;

    private void Awake()
    {
        instance = this;
        cam = GetComponentInChildren<Camera>();
        startingCameraOffset = cam.transform.localPosition;
    }

    public void SetOffset(Vector2 offset)
    {
        cam.transform.localPosition = (Vector3)offset * offsetSensitivity + startingCameraOffset;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed*Time.deltaTime);
    }

    private void Update()
    {
        int selected = GetPressedNumber();
        PlayerController.selectedIndex = selected >= 0 ? selected : PlayerController.selectedIndex ;
    }

    int GetPressedNumber()
    {
        for (int number = 0; number <= 9; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
                return number;
        }
        return -1;
    }
}
