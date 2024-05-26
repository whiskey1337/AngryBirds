using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static PlayerInput PlayerInput;

    private InputAction mousePositionAction;
    private InputAction mouseAction;

    public static Vector2 MousePosition;

    public static bool WasLeftMouseButtonPressed;
    public static bool WasLeftMouseButtonReleased;
    public static bool IsLeftMouseButtonPressed;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        mousePositionAction = PlayerInput.actions["MousePosition"];
        mouseAction = PlayerInput.actions["Mouse"];
    }

    private void Update()
    {
        MousePosition = mousePositionAction.ReadValue<Vector2>();

        WasLeftMouseButtonPressed = mouseAction.WasPressedThisFrame();
        WasLeftMouseButtonReleased = mouseAction.WasReleasedThisFrame();
        IsLeftMouseButtonPressed = mouseAction.IsPressed();
    }
}