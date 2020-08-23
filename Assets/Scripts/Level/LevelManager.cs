using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using System.Linq;

public class LevelManager : MonoBehaviour {


    public PlayerController player;



    public GameObject wallPrefab;
    public Tile[,] playArea = new Tile[71, 51];

    public Tile[,] PlayArea => playArea;
    //private Tile[,] PlayArea;

    [Range(1, 30)]
    public int TileSize = 1;


    public int FieldAreaX = 71;

    [SerializeField]
    public int FieldAreaZ = 51;


    public TMP_Text debugScreen;
    public TMP_Text waveScreen;

    public WaveManager waveManager;



    //new Vector2Int(Random.Range(0, playArea.GetLength(0) - 1), Random.Range(0, playArea.GetLength(1) - 1))
    public Vector2Int GetGetRandomGridPosition()
    {
        if (playArea == null) return Vector2Int.zero;
        Vector2Int vector = Vector2Int.zero;
        int attemps = 0;
        while (vector == Vector2Int.zero)
        {
            vector = new Vector2Int(Random.Range(0, playArea.GetLength(0) - 1), Random.Range(0, playArea.GetLength(1) - 1));
            Vector2Int playerPos = new Vector2Int(player.PositionX, player.PositionZ);
            float distanceToPlayer = (playerPos - vector).magnitude;

            if (playArea[vector.x, vector.y].ExistsEntity() || distanceToPlayer <= 8f)
                vector = Vector2Int.zero;
            attemps++;
            if (attemps == 30)
            {
                break;
            }
        }
        return new Vector2Int(vector.x, vector.y);
    }
    static LevelManager ins;
    public static LevelManager GetInstance
    {
        get
        {
            ins = ins == null ? FindObjectOfType<LevelManager>() : new GameObject("LevelManager").AddComponent<LevelManager>();
            return ins;
        }
    }
    void Start()
    {
        waveManager = new WaveManager(this, waveScreen, debugScreen);
        ObjectPooler.PoolGameObject(wallPrefab, 300);
        playArea = new Tile[FieldAreaX / TileSize, FieldAreaZ / TileSize];
    }



    Vector2Int GetLocationInGrid(int x, int z)
    {
        return new Vector2Int((x - FieldAreaX / 2), (z - FieldAreaZ / 2));
    }



    //By calling this method, the game starts.
    public void CreateLevel()
    {
        hasStarted = true;
        //Transform floorParent = CreateParent("Floor");
        StartCoroutine(_CreateLevel());
    }

    bool hasStarted = false;
    private IEnumerator _CreateLevel()
    {
        EffectsManager.GetInstance.CurrentBackgroundMusic.Play();
        Transform enemyParent = CreateParent("Enemies");
        yield return new WaitForEndOfFrame();
        if (waveManager.allSpawnedEnemies.Count != 0)
        {
            waveManager.allSpawnedEnemies.ExecuteAction(e => e.gameObject.SetActive(false)).ToList().Clear();
            waveManager.summonedEnemies.ExecuteAction(e => e.gameObject.SetActive(false)).ToList().Clear();
            TimeHandler.GetInstance.ClearRecordings();

        }
        EffectsManager.GetInstance.CurrentBackgroundMusic.pitch = 1;
        waveManager.WaveCounter.text = "";
        CreateWalls();
        player.manager.RevivePlayer();



        for (int x = 0; x < playArea.GetLength(0); x++)
        {
            for (int z = 0; z < playArea.GetLength(1); z++)
            {
                PlayArea[x, z].position = GetLocationInGrid(x * TileSize, z * TileSize);
            }
        }
        yield return new WaitForSeconds(1.5f);
        yield return waveManager.DeployFirstWave(this, 2, 0.05f, new Enemy("Enemy"));
    }

    private void CreateWalls()
    {
        GameObject wallNorth = CreateNewWall(0, FieldAreaZ / 2, FieldAreaX, 1, "North");
        GameObject wallEast = CreateNewWall(-FieldAreaX / 2, 0, 1, FieldAreaZ, "East");
        GameObject wallSouth = CreateNewWall(0, -FieldAreaZ / 2, FieldAreaX, 1, "South");
        GameObject wallWest = CreateNewWall(FieldAreaX / 2, 0, 1, FieldAreaZ, "West");
    }

    private GameObject CreateNewWall(int x, int z, int length, int width, string name)
    {
        GameObject obj = ObjectPooler.GetPooledObject(wallPrefab.GetInstanceID());
        obj.gameObject.SetActive(true);
        obj.transform.localScale = new Vector3(length, 1, width);
        obj.transform.position = new Vector3(x - TileSize * 0.7f, 0, z - TileSize * 0.7f);
        obj.name = name;
        return obj;
    }


    private Transform CreateParent(string v)
    {
        Transform s = new GameObject(v).transform;
        s.SetParent(transform);
        return s;
    }



    private int CenterOffsetPositionY(int ii)
    {
        return ii - playArea.GetLength(1) / 2;
    }

    private int CenterOffsetPositionX(int i)
    {
        return i - playArea.GetLength(0) / 2;
    }

    TimeHandler handler;

    public bool hasAlreadyRewinded { private get; set; }
    // Update is called once per frame
    void Update()
    {

        if (!TimeHandler.GetInstance.isRewinding && hasStarted)
        {
            handler = handler ?? TimeHandler.GetInstance;

            handler.countdownUntilRewind = handler.countdownUntilRewind.CountTime(TimeHandler.MAXCOUNTDOWN);
            hasAlreadyRewinded = handler.AttemptToRewind(this);
        }

    }






    public struct Tile {
        public Vector2Int position;
        public GameObject entity;


        public bool ExistsEntity()
        {
            return entity != null;
        }

        public void RemoveEntity()
        {
            entity = null;
        }


        public Vector2Int this[int x, int z]
        {
            get => position;
            set => position = value;
        }


        public Vector3 GetWorldPosition(GameObject obj = null)
        {
            if (entity == null)
            {
                if (obj == null)
                    return new Vector3(position.x, 0, position.y);
                return new Vector3(position.x, obj.transform.position.y, position.y);
            }
            if (obj == null) return Vector3.zero;
            return obj.transform.position;

        }
    }
}
