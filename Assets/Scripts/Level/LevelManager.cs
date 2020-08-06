using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class LevelManager : MonoBehaviour {


    public PlayerController player;

    [SerializeField]
    GameObject floor;
    [SerializeField]
    GameObject wallPrefab;
    private Tile[,] playArea;

    public Tile[,] PlayArea => playArea;
    //private Tile[,] PlayArea;
    [SerializeField]
    [Range(1, 30)]
    private int TileSize = 1;

    [SerializeField]
    int FieldAreaX = 20;

    [SerializeField]
    int FieldAreaZ = 20;


    public TMP_Text debugScreen;
    public TMP_Text waveScreen;

    public WaveManager waveManager;



    //new Vector2Int(Random.Range(0, playArea.GetLength(0) - 1), Random.Range(0, playArea.GetLength(1) - 1))
    public Vector3 GetGetRandomGridPosition<A>() where A : IEquatable<A>
    {
        player = player ?? FindObjectOfType<PlayerController>();

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

    public static LevelManager GetInstance { get; private set; }

    void Awake() => GetInstance = GetInstance ?? FindObjectOfType<LevelManager>() ?? new GameObject("LevelManager").AddComponent<LevelManager>();

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
        waveManager = new WaveManager(this, waveScreen, debugScreen);
        ObjectPooler.PoolGameObject(wallPrefab, 300);
        playArea = new Tile[FieldAreaX / TileSize, FieldAreaZ / TileSize];

        CreateLevel();



       
    }

    //By calling this method, the game starts.
    public void CreateLevel()
    {
        //Transform floorParent = CreateParent("Floor");
        Transform enemyParent = CreateParent("Enemies");

        CreateWalls();



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

        StartCoroutine(waveManager.DeployFirstWave(1, 0.05f, new Enemy("Enemy"), new Enemy("Enemy"), new Enemy("Enemy")));

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

    private void CreateFloorTile(int x, int z, Transform floorParent)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Destroy(tile.GetComponent<Collider>());
        tile.transform.position = playArea[x, z].GetWorldPosition(tile) - new Vector3(0, 0.5f, 0);
        tile.transform.localScale = new Vector3(0.3f, 1, 0.3f);
        tile.transform.SetParent(floorParent);
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
