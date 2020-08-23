﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Cinemachine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable
{
    public LevelManager levelManager;

    // Start is called before the first frame update
    [SerializeField] bool showCursor;
    [SerializeField] private float Speed = 1;

    float localTimer;
    [SerializeField] float movementRate;

    public bool godMode = false;

    [SerializeField] CinemachineVirtualCamera playerCamera;

    public PlayerManager manager;

    public float fireRate = 0.3f;
    float timeBetweenFire = 0;


    [SerializeField] BulletBehaivour bulletPrefab;

    Rigidbody _controller;


    public GameObject aimGameObject;

    private void Awake()
    {
        _controller = GetComponent<Rigidbody>();
        ObjectPooler.PoolGameObject(bulletPrefab, 300);
        TimeHandler.GetInstance.PlayerReference = this;
        manager = new PlayerManager(this);

        gameObject.SetActive(false);


        //Move the player to the middle of the field.
    }

    public void ResetPositionToSpawn()
    {
        //Used to center the player on the grid at the start of the game.
        StartCoroutine(MoveWithinGrid());
    }

    void Start()
    {
        Debug.Log(levelManager);
        Debug.Log(levelManager.playArea);
        Debug.Log(levelManager.playArea.GetLength(0));
        _positionX = (levelManager.PlayArea.GetLength(0) - 1) / 2;
        _positionZ = (levelManager.PlayArea.GetLength(1) - 1) / 2;

        ResetPositionToSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        // GridMovement();
        if (!isResetting)
        {
            AimIndicator();
            ShootBullet();
        }
    }


    private void NormalMovement()
    {
        Vector3 movementDirection = InputManager.GetInstance.movementInput * Speed;
        if (movementDirection == Vector3.zero)
            _controller.velocity = Vector3.Lerp(_controller.velocity, Vector3.zero, 0.1f);
        _controller.velocity += movementDirection;

        _controller.velocity = Vector3.ClampMagnitude(_controller.velocity, Speed * 10f);
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
        timeBetweenFire = timeBetweenFire.CountTime(fireRate);
        if (InputManager.GetInstance.IsShooting && timeBetweenFire == fireRate)
        {
            BulletBehaivour bullet = BulletBehaivour.InitializeBullet(gameObject,
                aimGameObject.transform.GetChild(1).position, aimGameObject.transform.rotation);
            bullet.physics.velocity = _controller.velocity;
            if (!TimeHandler.GetInstance.isRewinding)
                TimeHandler.GetInstance.RecordAction(this);
            timeBetweenFire = 0;
        }
    }

    private int _positionX;

    public int PositionX
    {
        get { return _positionX; }
        set { _positionX = Mathf.Clamp(value, 0, levelManager.PlayArea.GetLength(0) - 1); }
    }

    private int _positionZ;

    public int PositionZ
    {
        get { return _positionZ; }
        set { _positionZ = Mathf.Clamp(value, 0, levelManager.PlayArea.GetLength(1) - 1); }
    }

    bool isResetting = false;

    /// <summary>
    /// Moves the player within a grid by incrementing 2 indexes that are used for a 2D array.
    /// </summary>
    /// 
    private IEnumerator MoveWithinGrid()
    {
        Vector3 newPos = levelManager.PlayArea[PositionX, PositionZ].GetWorldPosition(gameObject);
        while (!newPos.IsWithinRadiusOf(transform.position, 3f))
        {
            var position = transform.position;
            EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("Immunity", position);
            _controller.isKinematic = true;
            _controller.velocity = Vector3.zero;
            yield return new WaitForEndOfFrame();
            position = Vector3.Lerp(position, newPos, 4f * Time.deltaTime);
            transform.position = position;
            isResetting = true;
        }

        transform.position = newPos;
        _controller.isKinematic = false;
        if (isResetting)
            isResetting = false;
        yield return null;

        FindObjectsOfType<BulletBehaivour>().ExecuteAction(b =>
            {
                b.physics.velocity = Vector3.zero;
                b.gameObject.SetActive(false);
                EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("BulletDeath",
                    b.transform.position);
            }
        );

        manager.HealPlayerBy(3);
        TimeHandler.GetInstance.countdownUntilRewind = 0;
    }

    int IncrementIndexBy(float value)
    {
        return Mathf.Sign(value) == 1 ? (int) Math.Ceiling(value) : (int) Math.Floor(value);
    }

    Vector3 _lookAtPos = Vector3.zero;
    Quaternion _targetRotation = Quaternion.identity;

    private void AimIndicator()
    {
        //Taken by Can Baycay; https://stackoverflow.com/questions/29457819/how-to-make-a-game-object-point-towards-the-mouse-in-unity-c
        var groundPlane = new Plane(Vector3.up, -transform.position.y);
        var mouseRay = Camera.main.ScreenPointToRay(InputManager.GetInstance.mousePosition);


        if (groundPlane.Raycast(mouseRay, out var hitDistance))
        {
            _lookAtPos = mouseRay.GetPoint(hitDistance);
            _targetRotation = Quaternion.LookRotation((_lookAtPos - transform.position).normalized, Vector3.up);
        }
    }

    void FixedUpdate()
    {
        if (!isResetting)
            NormalMovement();
        if (double.IsNaN(_targetRotation.w)) return;
        var rotation = Quaternion.Lerp(transform.rotation, _targetRotation, 1f);
        aimGameObject.transform.localRotation = rotation;
    }


    private void OnDrawGizmos()
    {
        if (showCursor && Application.isPlaying)
            Gizmos.DrawSphere(_lookAtPos, 1);
    }

    public void TakeDamage(BulletBehaivour bullet)
    {
        if (bullet == null)
        {
            if (!isResetting && !godMode && manager.IsAlive)
            {
                manager.LooseOneLife();
                EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("PlayerHit", transform.position);
            }
        }
        else if (bullet.owner == gameObject)
        {
            Collision contactPoint = bullet.ContactPoint;


            var direction = Vector3.Reflect(bullet.lastKnownVelocity.normalized, contactPoint.contacts[0].normal);

            bullet.currentVelocity = direction.normalized;
        }
        else
        {
            if (!isResetting && !godMode && manager.IsAlive)
            {
                manager.LooseOneLife();
                EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("PlayerHit", transform.position);
            }

            bullet.physics.velocity = Vector3.zero;
            bullet.gameObject.SetActive(false);
        }
    }
}