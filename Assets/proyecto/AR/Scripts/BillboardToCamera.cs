using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        // Hace que el objeto mire a la cámara
        transform.forward = mainCamera.transform.forward;
    }
}