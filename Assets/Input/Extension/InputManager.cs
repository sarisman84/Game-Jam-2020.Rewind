using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlSchema { InGame, Menu }
public class InputManager : InputSchema.IIngameActions
{
    static InputManager ins;
    public static InputManager Singleton
    {
        get
        {
            ins = ins ?? new InputManager();
            return ins;
        }
    }
    InputSchema controls;
    public InputManager()
    {
        controls = new InputSchema();
        controls.Ingame.SetCallbacks(this);
        SetControlsState(ControlSchema.InGame, true);
        SetControlsState(ControlSchema.Menu, true);
    }

    public void SetControlsState(ControlSchema type, bool value)
    {

        switch (type)
        {
            case ControlSchema.InGame:
                if (value)
                {
                    controls.Ingame.Enable();
                    break;
                }
                controls.Ingame.Disable();
                break;

            case ControlSchema.Menu:
                if (value)
                {
                    controls.Menu.Enable();
                    break;
                }
                controls.Menu.Disable();
                break;
        }
    }
    public void OnAiming(InputAction.CallbackContext context)
    {
        
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
       
    }
}
