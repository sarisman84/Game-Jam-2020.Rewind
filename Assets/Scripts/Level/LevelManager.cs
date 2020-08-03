﻿using System;
using Assets.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {


    public PlayerController player;

    [SerializeField]
    GameObject floor, wallPrefab;
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

    private static LevelManager Instance;
    public static LevelManager GetInstance
    {
        get
        {
            Instance = Instance ?? GameObject.FindObjectOfType<LevelManager>() ?? new GameObject("LevelManager").AddComponent<LevelManager>();
            return Instance;
        }
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
        ObjectPooler.PoolGameObject(wallPrefab, 300);
        playArea = new Tile[FieldAreaX / TileSize, FieldAreaZ / TileSize];

        CreateLevel();

        

    }

    private void CreateLevel()
    {
        Transform floorParent = CreateParent("Floor");
        Transform enemyParent = CreateParent("Enemies");




        for (int x = 0; x < playArea.GetLength(0); x++)
        {
            for (int z = 0; z < playArea.GetLength(1); z++)
            {
                PlayArea[x, z].position = GetLocationInGrid(x * TileSize, z * TileSize);


                //walls
                //if (x == 0 || z == 0 || x == playArea.GetLength(0) - 1 || z == playArea.GetLength(1) - 1)
                //{
                //    GameObject obj = ObjectPooler.GetPooledObject(wallPrefab.GetInstanceID());
                //    obj.SetActive(true);
                //    obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
                //    obj.transform.position = playArea[x, z].GetWorldPosition(obj);
                //    playArea[x, z].entity = obj;
                //}


                CreateFloorTile(x, z, floorParent);

                bool edgeTiles = x == 0 || z == 0 || x == playArea.GetLength(0) - 1 || z == playArea.GetLength(1) - 1;
                //Enemies
                if (x % (5) == 1 && z % (5) == 1)
                {
                    SpawnEnemy(x, z).obj.transform.SetParent(enemyParent);
                }


                if (!PlayArea[x, z].ExistsEntity())
                {
                    SpawnEntity(PlayArea[x, z]);
                }

            }
        }
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

    public Enemy SpawnEnemy(int x, int z)
    {
        return new Enemy(new Vector2Int(x, z));
    }

    public void SpawnEntity(Tile tile)
    {

    }


    public void OnValidate()
    {

        Vector3 size = floor.transform.localScale;
        size.x = FieldAreaX;
        size.z = FieldAreaZ;
        floor.transform.localScale = size;


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


        public Vector3 GetWorldPosition(GameObject obj)
        {
            if (entity == null)
            {
                //                entity = obj;
                return new Vector3(position.x, obj.transform.position.y, position.y);
            }
            return obj.transform.position;

        }
    }
}