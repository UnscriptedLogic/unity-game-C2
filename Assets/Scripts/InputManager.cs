using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private InputControls controls;
    public event Action<bool> isDragging;
    public event Action<Vector2> mousePos;

    private void Awake()
    {
        instance = this;

        controls = new InputControls();

        controls.Enable();

        controls.DefaultMap.IsDragging.started += IsDragging_started;
        controls.DefaultMap.IsDragging.canceled += IsDragging_canceled;

        controls.DefaultMap.MousePosition.performed += MousePosition_performed;
    }

    private void IsDragging_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) => isDragging?.Invoke(true);
    private void IsDragging_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) => isDragging?.Invoke(false);
    
    private void MousePosition_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) => mousePos?.Invoke(obj.ReadValue<Vector2>());
}
