using UnityEngine;
using System.Collections;
using Assets.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using TMPro;

public class WaveManager {

    public int currentWave { get; set; } = 0;
    public List<EnemyBehaviour> allSpawnedEnemies = new List<EnemyBehaviour>();
    List<Enemy> allEnemyTypes = new List<Enemy>();


    TextMeshProUGUI waveCounter;

    public bool areAllEnemiesDead
    {
        get
        {
            int inactiveCount = allSpawnedEnemies.Count(e => !e.activeSelf);
            
            return inactiveCount == allSpawnedEnemies.Count - 1;
        }
    }

    public MonoBehaviour Behaivour { get; }

    public WaveManager(MonoBehaviour behaivour)
    {
        Behaivour = behaivour;
        waveCounter = GameObject.FindObjectOfType<TextMeshProUGUI>();
    }



    public IEnumerator DeployFirstWave<E>(int amountOfEnemies, float spawnDelay, params E[] enemyTypes) where E : Enemy
    {

        foreach (var item in enemyTypes)
        {
            for (int i = 0; i < amountOfEnemies; i++)
            {
                Vector2Int index = LevelManager.GetInstance.GetGetRandomGridPosition<Vector2Int>().ToVector2Int();
                Vector3 spawnPos = LevelManager.GetInstance.PlayArea[index.x, index.y].GetWorldPosition();
                allSpawnedEnemies.Add(item.SpawnEnemy(spawnPos, index));
                yield return new WaitForSeconds(spawnDelay);
            }

            allEnemyTypes.Add(item);
        }

        currentWave++;
        waveCounter.text = $"Wave:{currentWave}";
    }


    public IEnumerator DeployNextWave()
    {
        TimeHandler.GetInstance.latestList++;

        yield return new WaitForSeconds(1.35f);


        IEnumerator p(EnemyBehaviour e)
        {
            e.ResetPositionToSpawn();
            e.AssignEvents(e.parentClass);
            e.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);

        }
        yield return allSpawnedEnemies.ExecuteAction(p);



        IEnumerator x(Enemy e)
        {
            int amm = Random.Range(2, 4);
            for (int i = 0; i < amm; i++)
            {
                Vector2Int index = LevelManager.GetInstance.GetGetRandomGridPosition<Vector2Int>().ToVector2Int();
                Vector3 spawnPos = LevelManager.GetInstance.PlayArea[index.x, index.y].GetWorldPosition();
                allSpawnedEnemies.Add(e.SpawnEnemy(spawnPos, index));
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield return allEnemyTypes.ExecuteAction(x);

        currentWave++;
        waveCounter.text = $"Wave:{currentWave}";
    }


}
