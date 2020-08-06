using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using System.Linq;

public class LevelManager : MonoBehaviour {


    public PlayerController player;


    public GameObject floor;

    public GameObject wallPrefab;
    public Tile[,] playArea = new Tile[71,51];

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
    public Vector3 GetGetRandomGridPosition<A>() where A : IEquatable<A>
    {
        if (playArea == null) return Vector3.zero;

        A a = default(A);
        if (a is Vector3 pos)
        {
            pos = playArea[Random.Range(0, playArea.GetLength(0) - 1), Random.Range(0, playArea.GetLength(1) - 1)].GetWorldPosition();

            return pos;
        }
        else if (a is Vector2Int vector)
        {
            int attemps = 0;
            while (vector == Vector2Int.zero)
            {
                vector = new Vector2Int(Random.Range(0, playArea.GetLength(0) - 1), Random.Range(0, playArea.GetLength(1) - 1));
                Vector2Int playerPos = new Vector2Int(player.PositionX, player.PositionZ);
                float distanceToPlayer = (playerPos - vector).magnitude;

                if (playArea[vector.x, vector.y].ExistsEntity()) vector = Vector2Int.zero;
                attemps++;
                if (attemps == 30)
                {
                    break;
                }
            }
            return new Vector3(vector.x, vector.y);
        }

        return Vector3.zero;
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
    void Awake()
    {
        player = player ?? FindObjectOfType<PlayerController>();
        if(player == null)
        player.gameObject.SetActive(false);
        waveManager = new WaveManager(this, waveScreen, debugScreen);
        ObjectPooler.PoolGameObject(wallPrefab, 300);
        playArea = new Tile[FieldAreaX / TileSize, FieldAreaZ / TileSize];
    }

    Vector3 GetPositionCenteredInArr(int x, int z)
    {
        return new Vector3(CenterOffsetPositionX(x), floor.transform.position.y + 1f, CenterOffsetPositionY(z));
    }

    Vector2Int GetLocationInGrid(int x, int z)
    {
        return new Vector2Int((x - FieldAreaX / 2), (z - FieldAreaZ / 2));
    }

    // Start is called before the first frame update
    void Start()
    {






    }

    //By calling this method, the game starts.
    public void CreateLevel()
    {
        //Transform floorParent = CreateParent("Floor");
        StartCoroutine(_CreateLevel());        
    }

    private IEnumerator _CreateLevel()
    {
        Transform enemyParent = CreateParent("Enemies");
        if (waveManager.allSpawnedEnemies.Count != 0)
        {
            waveManager.allSpawnedEnemies.ExecuteAction(e => e.gameObject.SetActive(false)).ToList().Clear();
            waveManager.allCustomTiles.ExecuteAction(t => t.gameObject.SetActive(false)).ToList().Clear();
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

                //CreateFloorTile(x, z, floorParent);

                bool edgeTiles = x == 0 || z == 0 || x == playArea.GetLength(0) - 1 || z == playArea.GetLength(1) - 1;
                //Enemies
                //if (x % (5) == 1 && z % (5) == 1)
                //{
                //    SpawnEnemy(x, z).model.transform.SetParent(enemyParent);
                //}


                if (!PlayArea[x, z].ExistsEntity())
                {
                    SpawnEntity(PlayArea[x, z]);
                }

            }
        }
        yield return new WaitForSeconds(1.5f);
        yield return waveManager.DeployFirstWave(2, 0.05f, new Enemy("Enemy"));
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

    // Update is called once per frame
    void Update()
    {

    }

    //public Enemy SpawnEnemy(int x, int z)
    //{
    //    Enemy enemy = new Enemy(new Vector2Int(x, z), "Enemy");
    //    allKnownEnemies.Add(enemy);
    //    return enemy;
    //}

    public void SpawnEntity(Tile tile)
    {

    }


    public void OnValidate()
    {

        //Vector3 size = floor.transform.localScale;
        //size.x = FieldAreaX;
        //size.z = FieldAreaZ;
        //floor.transform.localScale = size;


        floor.transform.position = new Vector3(0, -1f, 0);


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
