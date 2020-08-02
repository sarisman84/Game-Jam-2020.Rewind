using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour, IDamageable
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
        if (InputManager.GetInstance.IsShooting) {
            BulletBehaivour bullet = ObjectPooler.GetPooledObject<BulletBehaivour>();
            bullet.gameObject.SetActive(true);
            bullet.Setup(aimGameObject);

            InputManager.GetInstance.IsShooting = false;
        }

    }

    private int _PositionX;
    public int PositionX
    {
        get { return _PositionX; }
        set { _PositionX = Mathf.Clamp(value, 0, LevelManager.GetInstance.FieldAreaX); }
    }

    private int _PositionZ;
    public int PositionZ
    {
        get { return _PositionZ; }
        set { _PositionZ = Mathf.Clamp(value, 0, LevelManager.GetInstance.FieldAreaZ); }
    }

    private void MovementInput()
    {
        Vector3 direction = InputManager.GetInstance.MovementDirection;
        PositionX += (int)direction.x;
        PositionZ += (int)direction.y;
        Debug.Log(new Vector2Int(PositionX, PositionZ));
        Vector3 movement = InputManager.GetInstance.GetMovement(Speed);
        controller.Move(movement);
    }

    Vector3 lookAtPos;
    Quaternion targetRotation;

    private void AimIndicator()
    {
        //Taken by Can Baycay; https://stackoverflow.com/questions/29457819/how-to-make-a-game-object-point-towards-the-mouse-in-unity-c
        var groundPlane = new Plane(Vector3.up, -transform.position.y);
        var mouseRay = Camera.main.ScreenPointToRay(InputManager.GetInstance.mousePosition);
        float hitDistance;


        if (groundPlane.Raycast(mouseRay, out hitDistance)) {
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

    public void TakeDamage()
    {
        //PlayerManager.GetInstance.RemoveOneLife();
    }
}