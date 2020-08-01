using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float Speed = 1;

    CharacterController controller;


    public GameObject aimGameObject;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = InputManager.Singleton.GetMovement(Speed);

        //Vector3 aimDirection = InputManager.Singleton.AimDirection(transform.position);
        //if (aimDirection != Vector3.zero)
        //{
        //    Quaternion newRotation = Quaternion.LookRotation(aimDirection, Vector3.up);
        //    newRotation.x = 0;
        //    newRotation.z = 0;
        //    //newRotation.y = -newRotation.y;
        //    aimGameObject.transform.rotation = newRotation;
        //}

        AimIndicator();





        controller.Move(movement);
    }

    Vector3 lookAtPos;
    Quaternion targetRotation;

    private void AimIndicator()
    {
        //Taken by Can Baycay; https://stackoverflow.com/questions/29457819/how-to-make-a-game-object-point-towards-the-mouse-in-unity-c
        var groundPlane = new Plane(Vector3.up, -transform.position.y);
        var mouseRay = Camera.main.ScreenPointToRay(InputManager.Singleton.mousePosition);
        float hitDistance;


        if (groundPlane.Raycast(mouseRay, out hitDistance))
        {
            lookAtPos = mouseRay.GetPoint(hitDistance);
            targetRotation = Quaternion.LookRotation((lookAtPos - transform.position).normalized, Vector3.up);

        }
    }

    void FixedUpdate()
    {
        if (targetRotation == null) return;
        var rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f);
        aimGameObject.transform.localRotation = rotation;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(lookAtPos, 1);
    }
}