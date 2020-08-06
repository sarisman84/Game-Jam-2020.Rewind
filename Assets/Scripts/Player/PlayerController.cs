using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.PlayerLoop;
using Cinemachine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour, IDamageable {

    public LevelManager levelManager;
    // Start is called before the first frame update
    [SerializeField]
    bool showCursor;
    [SerializeField]
    private float Speed = 1;

    float localTimer;
    [SerializeField]
    float movementRate;

    public bool godMode = false;

    [SerializeField]
    CinemachineVirtualCamera playerCamera;

    public PlayerManager manager;


    [SerializeField]
    BulletBehaivour bulletPrefab;

    Rigidbody controller;


    public GameObject aimGameObject;
    private void Awake()
    {
        controller = GetComponent<Rigidbody>();
        ObjectPooler.PoolGameObject(bulletPrefab, 300);
        TimeHandler.GetInstance.PlayerReference = this;
        manager = new PlayerManager(this);


        //Move the player to the middle of the field.


    }

    public void ResetPositionToSpawn()
    {

        TimeHandler.GetInstance.isRewinding = false;
        //Used to center the player on the grid at the start of the game.
        StartCoroutine(MoveWithinGrid());
    }

    void Start()
    {
        _PositionX = (levelManager.PlayArea.GetLength(0) - 1) / 2;
        _PositionZ = (levelManager.PlayArea.GetLength(1) - 1) / 2;

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
        if (movementDirection == Vector3.zero) controller.velocity = Vector3.Lerp(controller.velocity, Vector3.zero, 0.1f);
        controller.velocity += movementDirection;

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
        if (InputManager.GetInstance.IsShooting && !TimeHandler.GetInstance.isRewinding)
        {

            InitializeBullet(aimGameObject.transform.GetChild(1).position, aimGameObject.transform.rotation);
            TimeHandler.GetInstance.RecordAction(this);
            InputManager.GetInstance.IsShooting = false;
        }

    }

    public void InitializeBullet(Vector3 firePosition, Quaternion fireRotation)
    {
        EffectsManager.GetInstance.CurrentAudioFiles.PlayAudioClip("PlayerShoot", c =>
        {
            float result = Random.Range(1f, 2f);
            c.Player.time = 0;
            if (TimeHandler.GetInstance.isRewinding)
            {
                result = Random.Range(-1f, -0.5f);
                c.Player.time = c.Player.clip.length / 4f;
            }
            c.Player.pitch = result;
        });

        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("BulletEffect", firePosition, c =>
        {
            c.prefab.transform.rotation = fireRotation;
        });
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

    bool isResetting = false;

    /// <summary>
    /// Moves the player within a grid by incrementing 2 indexes that are used for a 2D array.
    /// </summary>
    /// 
    private IEnumerator MoveWithinGrid()
    {
        controller.velocity = Vector3.zero;
        Vector3 newPos = LevelManager.GetInstance.PlayArea[PositionX, PositionZ].GetWorldPosition(gameObject);
        while (!newPos.IsWithinRadiusOf(transform.position, 3f))
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(transform.position, newPos, 4f * Time.deltaTime);
            isResetting = true;
        }
        transform.position = newPos;
        if (isResetting)
            isResetting = false;
        yield return null;
        EffectsManager.GetInstance.CurrentBackgroundMusic.time = 0;
        EffectsManager.GetInstance.CurrentBackgroundMusic.pitch = 1;
        FindObjectsOfType<BulletBehaivour>().ExecuteAction(b =>
        {

            b.physics.velocity = Vector3.zero;
            b.gameObject.SetActive(false);
            EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemyDeath", b.transform.position);
        }
        );

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
        if (!isResetting)
            NormalMovement();
        if (targetRotation == null || double.IsNaN(targetRotation.w)) return;
        var rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f);
        aimGameObject.transform.localRotation = rotation;
    }


    private void OnDrawGizmos()
    {
        if (showCursor && Application.isPlaying)
            Gizmos.DrawSphere(lookAtPos, 1);
    }

    public void TakeDamage(BulletBehaivour bullet)
    {
        if (!isResetting && !godMode)
            manager.LooseOneLife();
        if (bullet == null) return;
        bullet.physics.velocity = Vector3.zero;
        bullet.gameObject.SetActive(false);
    }
}