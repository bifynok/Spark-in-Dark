using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIInputController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private bool isKeyboard = false;

    private Vector2 lastMousePos;
    
    private GameObject lastSelected;
    
    private PointerEventData pointerEventData;
    
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>(16);

    private const float MOUSE_MOVE_THRESHOLD = 4f;
    private const float RAYCAST_INTERVAL = 0.1f;
    private float nextRaycastTime = 0f;


    void Start()
    {
        pointerEventData = new PointerEventData(EventSystem.current);

        EventSystem.current.SetSelectedGameObject(null);

        if (Mouse.current != null)
            lastMousePos = Mouse.current.position.ReadValue();

        Cursor.visible = true;
    }

    void Update()
    {
        CheckMouseMovement();
        CheckKeyboardNavigation();
    }

    private void CheckMouseMovement()
    {
        if (Mouse.current == null) return;

        Vector2 now = Mouse.current.position.ReadValue();

        bool isMoved = (now - lastMousePos).sqrMagnitude > MOUSE_MOVE_THRESHOLD * MOUSE_MOVE_THRESHOLD;
        bool isRaycastTime = Time.unscaledTime > nextRaycastTime;

        if (isKeyboard && !isMoved)
        {
            lastMousePos = now;
            return;
        }

        if (isMoved)
        {
            isKeyboard = false;
            Cursor.visible = true;
        }

        if (!isKeyboard && (isMoved || isRaycastTime))
        {
            nextRaycastTime = Time.unscaledTime + RAYCAST_INTERVAL;

            GameObject hovered = GetUIUnderMouse();

            if (hovered == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            else if (EventSystem.current.currentSelectedGameObject != hovered)
            {
                lastSelected = hovered;
                EventSystem.current.SetSelectedGameObject(hovered);
            }
        }

        lastMousePos = now;
    }

    private void CheckKeyboardNavigation()
    {
        if (Keyboard.current == null) return;

        bool pressed =
            Keyboard.current.upArrowKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame ||
            Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Keyboard.current.rightArrowKey.wasPressedThisFrame ||
            Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.aKey.wasPressedThisFrame ||
            Keyboard.current.dKey.wasPressedThisFrame;

        if (!pressed) return;

        isKeyboard = true;
        Cursor.visible = false;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != null)
        {
            lastSelected = selected;
            return;
        }

        if (lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
            lastSelected = firstSelected;
        }
    }

    private GameObject GetUIUnderMouse()
    {
        pointerEventData.position = Mouse.current.position.ReadValue();

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (var r in raycastResults)
        {
            var selectable = r.gameObject.GetComponentInParent<Selectable>();
            if (selectable != null)
                return selectable.gameObject;
        }

        return null;
    }

    public bool IsUsingKeyboard() => isKeyboard;
}
