using Assets.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Vector2Int[,] PlayArea;
    [SerializeField]
    private int TileSize = 1;

    [SerializeField]
    private int FieldAreaX = 20;
        
    [SerializeField]
    private int FieldAreaZ = 20;

    // Start is called before the first frame update
    void Start()
    {
        PlayArea = new Vector2Int[FieldAreaX,FieldAreaZ];

        for (int i = 0; i < FieldAreaX; i++) {
            for (int ii = 0; ii < FieldAreaZ; ii++) {

                SpawnEnemy(new Vector2Int(i-FieldAreaX/2, ii - FieldAreaZ / 2));
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemy(Vector2Int location)
    {
        new Enemy(location.x * TileSize, location.y * TileSize);
    }
}
