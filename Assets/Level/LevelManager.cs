using System;
using Assets.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    GameObject floor, wallPrefab;
    private Vector2Int[,] PlayArea;
    [SerializeField]
    [Range(Int32.MaxValue, 1)]
    private int TileSize = 1;

    [SerializeField]
    private int FieldAreaX = 20;

    [SerializeField]
    private int FieldAreaZ = 20;

    // Start is called before the first frame update
    void Start()
    {
        ObjectPooler.PoolGameObject(wallPrefab, 300);
        PlayArea = new Vector2Int[FieldAreaX, FieldAreaZ];

        for (int i = 0; i < FieldAreaX; i++)
        {
            for (int ii = 0; ii < FieldAreaZ; ii++)
            {
                //walls
                if (i == 0 || ii == 0 || i == FieldAreaX - 1 || ii == FieldAreaZ - 1)
                {
                    GameObject obj = ObjectPooler.GetPooledObject(wallPrefab.GetInstanceID());
                    obj.SetActive(true);
                    obj.transform.position = new Vector3(CenterOffsetPositionX(i), floor.transform.position.y + 1f, CenterOffsetPositionY(ii));
                }
                else if (i % 5 == 0 && ii % 5 == 0)
                {
                    SpawnEnemy(new Vector2Int(i - FieldAreaX / 2, ii - FieldAreaZ / 2));
                }
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


    public void OnValidate()
    {
        FieldAreaX = (int)floor.transform.localScale.x;
        FieldAreaZ = (int)floor.transform.localScale.z;
    }
}
