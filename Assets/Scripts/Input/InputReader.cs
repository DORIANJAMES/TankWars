using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader", order = 0)]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<bool> PrimaryFireEvent = delegate { }; 
    public event Action<Vector2> MovementEvent = delegate { };
    public event Action<float> AccelerateEvent = delegate { };
    
    public Vector2 AimPosition { get; private set; }
    
    private Controls controls;
    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }
    

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MovementEvent?.Invoke(context.ReadValue<Vector2>());
        } else if (context.canceled)
        {
            MovementEvent?.Invoke(Vector2.zero);
        }
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        } else if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
        
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log(context.ReadValue<float>());
            AccelerateEvent?.Invoke(1.5f);
        }
        else if (context.canceled)
        {
            Debug.Log(context.ReadValue<float>());
            AccelerateEvent?.Invoke(1f);
        }
    }
}
