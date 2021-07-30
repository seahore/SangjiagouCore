using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, DefaultInputActions.IGameplayActions
{
    public event UnityAction<Vector2> Point = delegate { };
    public event UnityAction<Vector2> Zoom = delegate { };
    public event UnityAction Select = delegate { };
    public event UnityAction NextTurn = delegate { };
    public event UnityAction ShowMenu = delegate { };
    DefaultInputActions inputActions;

    void OnEnable()
    {
        inputActions = new DefaultInputActions();
        inputActions.Gameplay.SetCallbacks(this);
        inputActions.Gameplay.Enable();
    }
    void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        //if (context.phase == InputActionPhase.Performed) {
            Point(context.ReadValue<Vector2>());
        //}
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Canceled) {
            Zoom(context.ReadValue<Vector2>());
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Canceled) {
            Select();
        }
    }

    public void OnNextTurn(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled) {
            NextTurn();
        }
    }

    public void OnShowMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled) {
            ShowMenu();
        }
    }
}
