using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PreviewRotator : MonoBehaviour, IDragHandler
{
    [SerializeField] private PreviewManager previewManager;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float controllerRotationSpeed;
    private GameObject preview;
    private bool rotatingController = false;
    private float controllerRotation;

    private void Awake()
    {
        preview = previewManager.GetCurrentPreview();
        previewManager.OnPreviewChanged += ChangePreview;
    }

    private void Update()
    {
        if (!rotatingController) return;
        preview.transform.Rotate(Vector3.up * controllerRotation * controllerRotationSpeed, Space.World);
    }

    public void OnDrag(PointerEventData eventData)
    {
        preview.transform.Rotate(Vector3.up * -eventData.delta.x * rotationSpeed, Space.World);
    }

    public void ChangePreview()
    {
        preview = previewManager.GetCurrentPreview();
    }

    public void RotateWithController(InputAction.CallbackContext context)
    {
        controllerRotation = -context.ReadValue<Vector2>().x;
        if (controllerRotation != 0) rotatingController = true;
        else rotatingController = false;
    }
}
