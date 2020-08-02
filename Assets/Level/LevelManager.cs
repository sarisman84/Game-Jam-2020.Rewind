using System;
using Assets.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    GameObject floor, wallPrefab;
    private Tile[,] playArea;

    public Tile[,] PlayArea => playArea;
    //private Tile[,] PlayArea;
    [SerializeField]
    [Range(Int32.MaxValue, 1)]
    private int TileSize = 1;

    [SerializeField]
    public int FieldAreaX = 20;

    [SerializeField]
    public int FieldAreaZ = 20;

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
        return new Vector2Int(x - FieldAreaX / 2, z - FieldAreaZ / 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        ObjectPooler.PoolGameObject(wallPrefab, 300);
        playArea = new Tile[FieldAreaX, FieldAreaZ];

        for (int x = 0; x < FieldAreaX; x++) {
            for (int z = 0; z < FieldAreaZ; z++) {
                PlayArea[x, z].position = GetLocationInGrid(x, z);
                //walls
                if (x == 0 || z == 0 || x == FieldAreaX - 1 || z == FieldAreaZ - 1) {
                    GameObject obj = ObjectPooler.GetPooledObject(wallPrefab.GetInstanceID());
                    obj.SetActive(true);
                    obj.transform.position = GetPositionCenteredInArr(x, z);
                }
                else if (x % 5 == 0 && z % 5 == 0) {
                    SpawnEnemy(PlayArea[x, z].position);
                }


                if (!PlayArea[x, z].ExistsEntity())
                    SpawnEntity(PlayArea[x, z]);
            }
        }

    }

    private int CenterOffsetPositionY(int ii)
    {
        return ii - FieldAreaZ / 2;
    }

    private int CenterOffsetPositionX(int i)
    {
        return i - FieldAreaX / 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemy(Vector2Int location)
    {
        new Enemy(new Vector3(location.x * TileSize, floor.transform.position.y + 1.5f, location.y * TileSize));
    }

    public void SpawnEntity(Tile tile)
    {

    }


    public void OnValidate()
    {
        FieldAreaX = (int)floor.transform.localScale.x;
        FieldAreaZ = (int)floor.transform.localScale.z;
    }

    public struct Tile
    {
        public Vector2Int position;
        public GameObject entity;


        public bool ExistsEntity()
        {
            return entity != null;
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
                entity = obj;
                return new Vector3(position.x, 0, position.y);
            }
            return Vector3.zero;

        }
    }
}
