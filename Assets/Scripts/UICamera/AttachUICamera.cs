using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class AttachUICamera : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [Inject] private UICamera uiCamera;

    private void Start()
    {
        TryGetComponent<Canvas>(out canvas);

        SmartDebug.Log($"UI Camera: {uiCamera}");

        if(canvas != null && uiCamera != null)
            canvas.worldCamera = uiCamera.GetComponent<Camera>();
    }
}