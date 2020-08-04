using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.PlayerLoop;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour, IDamageable {
    // Start is called before the first frame update
    [SerializeField]
    bool showCursor;
    [SerializeField]
    private float Speed = 1;

    float localTimer;
    [SerializeField]
    float movementRate;

    [SerializeField]
    CinemachineVirtualCamera playerCamera;




    [SerializeField]
    BulletBehaivour bulletPrefab;

    CharacterController controller;


    public GameObject aimGameObject;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        ObjectPooler.PoolGameObject(bulletPrefab, 300);
        TimeHandler.GetInstance.PlayerReference = this;

        //Move the player to the middle of the field.
        

    }

    void Start()
    {
        _PositionX = (LevelManager.GetInstance.PlayArea.GetLength(0) - 1) / 2;
        _PositionZ = (LevelManager.GetInstance.PlayArea.GetLength(1) - 1) / 2;

        //Used to center the player on the grid at the start of the game.
        MoveWithinGrid();
    }
    // Update is called once per frame
    void Update()
    {

        NormalMovement();
       // GridMovement();

        AimIndicator();
        ShootBullet();


    }

    private void NormalMovement()
    {
        Vector3 movementDirection = InputManager.GetInstance.movementInput * Time.deltaTime * Speed;
        controller.Move(movementDirection);

    }

    private void GridMovement()
    {
        localTimer += Time.deltaTime;
        localTimer = Mathf.Clamp(localTimer, 0, movementRate);
        if (localTimer == movementRate)
        {
            MoveWithinGrid();
            localTimer = 0;
        }
    }

    private void ShootBullet()
    {
        if (InputManager.GetInstance.IsShooting)
        {
            InitializeBullet(aimGameObject.transform.GetChild(1).position, aimGameObject.transform.rotation);
            TimeHandler.GetInstance.RecordAction(this);
            InputManager.GetInstance.IsShooting = false;
        }

    }

    public void InitializeBullet(Vector3 firePosition, Quaternion fireRotation)
    {
        BulletBehaivour bullet = ObjectPooler.GetPooledObject<BulletBehaivour>();
        bullet.gameObject.SetActive(true);
        bullet.Setup(firePosition, fireRotation);
    }

    private int _PositionX;
    public int PositionX
    {
        get { return _PositionX; }
        set { _PositionX = Mathf.Clamp(value, 0, LevelManager.GetInstance.PlayArea.GetLength(0) - 1); }
    }

    private int _PositionZ;
    public int PositionZ
    {
        get { return _PositionZ; }
        set { _PositionZ = Mathf.Clamp(value, 0, LevelManager.GetInstance.PlayArea.GetLength(1) - 1); }
    }

    /// <summary>
    /// Moves the player within a grid by incrementing 2 indexes that are used for a 2D array.
    /// </summary>
    private void MoveWithinGrid()
    {
        Vector3 direction = InputManager.GetInstance.movementInput;
        int newX = Mathf.Clamp(PositionX + IncrementIndexBy(direction.x), 0, LevelManager.GetInstance.PlayArea.GetLength(0) - 1);
        int newZ = Mathf.Clamp(PositionZ + IncrementIndexBy(direction.z), 0, LevelManager.GetInstance.PlayArea.GetLength(1) - 1);
        if (!LevelManager.GetInstance.PlayArea[newX, newZ].ExistsEntity())
        {
            PositionX += IncrementIndexBy(direction.x);
            PositionZ += IncrementIndexBy(direction.z);
            transform.position = LevelManager.GetInstance.PlayArea[PositionX, PositionZ].GetWorldPosition(gameObject);
        }



    }

    int IncrementIndexBy(float value)
    {
        return Mathf.Sign(value) == 1 ? (int)Math.Ceiling(value) : (int)Math.Floor(value);
    }

    Vector3 lookAtPos = Vector3.zero;
    Quaternion targetRotation = Quaternion.identity;

    private void AimIndicator()
    {
        //Taken by Can Baycay; https://stackoverflow.com/questions/29457819/how-to-make-a-game-object-point-towards-the-mouse-in-unity-c
        var groundPlane = new Plane(Vector3.up, -transform.position.y);
        var mouseRay = Camera.main.ScreenPointToRay(InputManager.GetInstance.mousePosition);
        float hitDistance;


        if (groundPlane.Raycast(mouseRay, out hitDistance))
        {
            lookAtPos = mouseRay.GetPoint(hitDistance);
            targetRotation = Quaternion.LookRotation((lookAtPos - transform.position).normalized, Vector3.up);

        }
    }

    void FixedUpdate()
    {
        if (targetRotation == null || double.IsNaN(targetRotation.w)) return;
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