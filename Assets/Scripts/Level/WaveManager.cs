using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using TMPro;

public class WaveManager {

    public int currentWave { get; set; } = 0;
    public List<IEntityBehaviour> allSpawnedEnemies = new List<IEntityBehaviour>();
    public List<IEntity> allEnemyTypes = new List<IEntity>();



    TMP_Text waveCounter;

    public TMP_Text WaveCounter => waveCounter;

    TMP_Text debugScreen;

    public bool areAllEnemiesDead
    {
        get
        {
            List<IEntityBehaviour> enemyList = allSpawnedEnemies.FindAll(e => e.GetType() == typeof(EnemyBehaviour));
            int inactiveCount = enemyList.Count(e => !e.activeSelf);
            DisplayAllLivingEnemies();
            return inactiveCount == enemyList.Count;
        }
    }

    private void DisplayAllLivingEnemies()
    {
        //debugScreen.text = $"Current Enemies Alive: {allSpawnedEnemies.Count(a => a.activeSelf)}";
    }

    public MonoBehaviour Behaivour { get; }

    public WaveManager(MonoBehaviour behaivour, TMP_Text screenText, TMP_Text debugText)
    {
        Behaivour = behaivour;
        waveCounter = GameObject.FindObjectOfType<TextMeshProUGUI>();
        debugScreen = debugText;
        waveCounter = screenText;
    }

    LevelManager levelManagerRef;

    public IEnumerator DeployFirstWave<E>(LevelManager levelManager, int amountOfEnemies, float spawnDelay, params E[] enemyTypes) where E : IEntity
    {
        allSpawnedEnemies.Clear();
        allEnemyTypes.Clear();

        levelManagerRef = levelManager;
        currentWave = 0;
        foreach (var item in enemyTypes)
        {
            for (int i = 0; i < amountOfEnemies; i++)
            {
                yield return CreateEnemyAtRandomPosition(item, spawnDelay, levelManager);

                DisplayAllLivingEnemies();
            }

            allEnemyTypes.Add(item);
        }

        currentWave++;
        Debug.Log($"Current Wave (at DeployFirstWave):{currentWave}");
        waveCounter.text = $"Wave:{currentWave}";
    }


    public IEnumerator DeployNextWave()
    {
        TimeHandler.GetInstance.latestList++;

        yield return new WaitForSeconds(1.35f);

        Annoy_O_Tron tron = new Annoy_O_Tron();
        CarpetBomber bomber = new CarpetBomber();

        AddEnemyType(new Turret(), 0);
        AddEnemyType(bomber, 6);
        AddEnemyType(tron, 3);
        AddEnemyType(new BouncyWall(), 1);


        IEnumerator RevivePreviousEnemies(IEntityBehaviour e)
        {

            (e as EnemyBehaviour)?.ResetPositionToSpawn();
            e.AssignEvents(e.parentClass);
            e.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            DisplayAllLivingEnemies();

        }
        yield return allSpawnedEnemies.ExecuteAction(RevivePreviousEnemies);



        IEnumerator AddExtraEnemies(IEntity e)
        {
            int amm = Random.Range(0, 3);
            for (int i = 0; i < amm; i++)
            {
                yield return CreateEnemyAtRandomPosition(e, 0.05f, levelManagerRef);
                DisplayAllLivingEnemies();

            }
        }
        yield return allEnemyTypes.ExecuteAction(AddExtraEnemies);

        currentWave++;
        Debug.Log($"Current Wave (at DeployNextWave):{currentWave}");
        waveCounter.text = $"Wave:{currentWave}";



    }

    private void AddEnemyType<E>(E enemy, int minWaveSpawn) where E : IEntity
    {
        if (!allEnemyTypes.Contains(enemy))
            if (currentWave >= minWaveSpawn)
            {
                allEnemyTypes.Add(enemy);
            }
    }



    IEnumerator CreateEnemyAtRandomPosition<E>(E e, float delay, LevelManager levelManager) where E : IEntity
    {
        Vector2Int index = levelManagerRef.GetGetRandomGridPosition();
        Vector3 spawnPos = levelManagerRef.PlayArea[index.x, index.y].GetWorldPosition();
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemySpawn", spawnPos);
        yield return new WaitForSeconds(delay);
        allSpawnedEnemies.Add(e.SpawnEntity(spawnPos, index, levelManager));

    }


}
