using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using TMPro;

public class WaveManager {

    public int currentWave { get; set; } = 0;
    public List<EnemyBehaviour> allSpawnedEnemies = new List<EnemyBehaviour>();
    List<Enemy> allEnemyTypes = new List<Enemy>();

    public List<EntityBehaviour> allCustomTiles = new List<EntityBehaviour>();


    TMP_Text waveCounter;

    public TMP_Text WaveCounter => waveCounter;

    TMP_Text debugScreen;

    public bool areAllEnemiesDead
    {
        get
        {
            int inactiveCount = allSpawnedEnemies.Count(e => !e.activeSelf);
            DisplayAllLivingEnemies();
            return inactiveCount == allSpawnedEnemies.Count;
        }
    }

    private void DisplayAllLivingEnemies()
    {
        debugScreen.text = $"Current Enemies Alive: {allSpawnedEnemies.Count(a => a.activeSelf)}";
    }

    public MonoBehaviour Behaivour { get; }

    public WaveManager(MonoBehaviour behaivour, TMP_Text screenText, TMP_Text debugText)
    {
        Behaivour = behaivour;
        waveCounter = GameObject.FindObjectOfType<TextMeshProUGUI>();
        debugScreen = debugText;
        waveCounter = screenText;
    }



    public IEnumerator DeployFirstWave<E>(int amountOfEnemies, float spawnDelay, params E[] enemyTypes) where E : Enemy
    {
        currentWave = 0;
        foreach (var item in enemyTypes)
        {
            for (int i = 0; i < amountOfEnemies; i++)
            {
                yield return CreateEnemyAtRandomPosition(item, spawnDelay);

                DisplayAllLivingEnemies();
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
            DisplayAllLivingEnemies();

        }
        yield return allSpawnedEnemies.ExecuteAction(p);



        IEnumerator x(Enemy e)
        {
            int amm = Random.Range(0, 2);
            for (int i = 0; i < amm; i++)
            {
                yield return CreateEnemyAtRandomPosition(e, 0.05f);
                yield return CreateEntityAtRandomPosition(0.05f);
                DisplayAllLivingEnemies();

            }
        }
        yield return allEnemyTypes.ExecuteAction(x);

        currentWave++;
        waveCounter.text = $"Wave:{currentWave}";



    }

    private IEnumerator CreateEntityAtRandomPosition(float v)
    {
        Vector2Int index = LevelManager.GetInstance.GetGetRandomGridPosition<Vector2Int>().ToVector2Int();
        Vector3 spawnPos = LevelManager.GetInstance.PlayArea[index.x, index.y].GetWorldPosition();
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemySpawn", spawnPos);
        yield return new WaitForSeconds(v);
        allCustomTiles.Add(new BouncyWall().SpawnEntity(spawnPos, index));
    }

    IEnumerator CreateEnemyAtRandomPosition(Enemy e, float delay)
    {
        Vector2Int index = LevelManager.GetInstance.GetGetRandomGridPosition<Vector2Int>().ToVector2Int();
        Vector3 spawnPos = LevelManager.GetInstance.PlayArea[index.x, index.y].GetWorldPosition();
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemySpawn", spawnPos);
        yield return new WaitForSeconds(delay);
        allSpawnedEnemies.Add(e.SpawnEnemy(spawnPos, index));

    }


}
