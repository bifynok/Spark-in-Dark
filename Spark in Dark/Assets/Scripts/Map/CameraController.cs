using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float clickThreshold = 10f;

    private Vector2 pressMouseScreenPos;
    private Vector2 prevMouseScreenPos;
    private bool dragging;

    public bool BlocksClick { get; private set; }

    private void Update()
    {
        HandleLeftMouse();
    }

    private void HandleLeftMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
        {
            pressMouseScreenPos = Mouse.current.position.ReadValue();
            prevMouseScreenPos = pressMouseScreenPos;
            dragging = true;
            BlocksClick = false;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame)
        {
            dragging = false;
        }

        if (dragging)
        {
            Vector2 cur = Mouse.current.position.ReadValue();

            Vector2 deltaScreen = cur - prevMouseScreenPos;

            Vector3 worldDelta = cam.ScreenToWorldPoint(new Vector3(deltaScreen.x, deltaScreen.y, 0)) - cam.ScreenToWorldPoint(Vector3.zero);

            transform.position -= worldDelta;

            prevMouseScreenPos = cur;

            if (!BlocksClick)
            {
                float distFromPress = (cur - pressMouseScreenPos).magnitude;
                if (distFromPress > clickThreshold)
                    BlocksClick = true;
            }
        }
    }
}
