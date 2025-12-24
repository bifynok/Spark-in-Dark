using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapInputHandler : MonoBehaviour
{
    public event Action<MapNode> NodeClicked;
    public event Action ConfirmPressed;
    public event Action CancelPressed;

    [SerializeField] private Camera cam;
    [SerializeField] private CameraController cameraController;

    private void Update()
    {
        HandleMouseClick();
        HandleKeyboard();
    }

    private void HandleMouseClick()
    {
        if (cameraController != null && cameraController.BlocksClick)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        if (!Mouse.current.leftButton.wasReleasedThisFrame)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
        worldPos.z = 0f;

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider == null)
            return;

        MapNode node = hit.collider.GetComponent<MapNode>();
        if (node != null)
            NodeClicked?.Invoke(node);
    }

    private void HandleKeyboard()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ConfirmPressed?.Invoke();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelPressed?.Invoke();
        }
    }

    public void ConfirmFromUI()
    {
        ConfirmPressed?.Invoke();
    }

    public void CancelFromUI()
    {
        CancelPressed?.Invoke();
    }
}
