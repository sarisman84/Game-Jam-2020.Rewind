using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlSchema { InGame, Menu }
public class InputManager : InputSchema.IIngameActions
{
    static InputManager ins;
    public static InputManager GetInstance
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
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementDirection = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {

        IsShooting = context.performed;

        //Debug.Log($"Has Performed: {context.performed}");
        //Debug.Log($"Has Started: {context.started}");


        //Debug.Log($"Result: {IsShooting}");

    }


    Vector3 MovementDirection { set; get; }

    public bool IsShooting { get; set; }
    public Vector3 mousePosition { get; internal set; }





    public Vector3 GetMovement(float speed)
    {
        return new Vector3(MovementDirection.x, 0, MovementDirection.y) * Time.deltaTime * speed;
    }



}
