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
    bool showCursor;
    [SerializeField]
    private float Speed = 1;


    [SerializeField]
    BulletBehaivour bulletPrefab;

    CharacterController controller;


    public GameObject aimGameObject;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        ObjectPooler.PoolGameObject(bulletPrefab, 300);


    }

    // Update is called once per frame
    void Update()
    {
        MovementInput();
        AimIndicator();
        ShootBullet();










    }

    private void ShootBullet()
    {
        if (InputManager.Singleton.IsShooting)
        {
            BulletBehaivour bullet = ObjectPooler.GetPoolObject<BulletBehaivour>();
            bullet.gameObject.SetActive(true);
            bullet.Setup(aimGameObject);

            InputManager.Singleton.IsShooting = false;
        }
    }

    private void MovementInput()
    {
        Vector3 movement = InputManager.Singleton.GetMovement(Speed);
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
        if (showCursor && Application.isPlaying)
            Gizmos.DrawSphere(lookAtPos, 1);
    }
}