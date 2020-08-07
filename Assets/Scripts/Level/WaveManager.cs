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
    public List<IEntity> allEntityTypes = new List<IEntity>();
    public List<IEntityBehaviour> summonedEnemies = new List<IEntityBehaviour>();



    TMP_Text waveCounter;

    public TMP_Text WaveCounter => waveCounter;

    TMP_Text debugScreen;

    public bool areAllEnemiesDead
    {
        get
        {
            List<IEntityBehaviour> enemyList = allSpawnedEnemies.FindAll(e => e.GetType() == typeof(EnemyBehaviour));
            int inactiveCount = enemyList.Count(e => !e.activeSelf) + summonedEnemies.Count(e => !e.activeSelf);
            DisplayAllLivingEnemies();

            return inactiveCount == enemyList.Count + summonedEnemies.Count;
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
        allEntityTypes.Clear();

        levelManagerRef = levelManager;
        currentWave = 0;
        foreach (var item in enemyTypes)
        {
            for (int i = 0; i < amountOfEnemies; i++)
            {
                yield return CreateEnemyAtRandomPosition(item, spawnDelay, levelManager);

                DisplayAllLivingEnemies();
            }

            allEntityTypes.Add(item);
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
        AddEntityType(new Slime(), 4);



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
            int amm = Random.Range(1, 2);
            for (int i = 0; i < amm; i++)
            {
                yield return CreateEnemyAtRandomPosition(e, 0.05f, levelManagerRef);
                DisplayAllLivingEnemies();

            }
        }
        yield return allEntityTypes.ExecuteAction(AddExtraEnemies);

        currentWave++;
        Debug.Log($"Current Wave (at DeployNextWave):{currentWave}");
        waveCounter.text = $"Wave:{currentWave}";



    }

    private void AddEntityType<E>(E entity, int minWaveSpawn) where E : IEntity
    {
        if (!allEntityTypes.Contains(entity))
            if (currentWave >= minWaveSpawn)
            {
                allEntityTypes.Add(entity);
            }
    }



    public IEnumerator CreateEnemyAtRandomPosition<E>(E e, float delay, LevelManager levelManager, bool summoned = false) where E : IEntity
    {
        Vector2Int index = levelManagerRef.GetGetRandomGridPosition();
        Vector3 spawnPos = levelManagerRef.PlayArea[index.x, index.y].GetWorldPosition();
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemySpawn", spawnPos);
        yield return new WaitForSeconds(delay);
        if (summoned)
        {
            summonedEnemies.Add(e.SpawnEntity(spawnPos, index, levelManager));

        }
        else
            allSpawnedEnemies.Add(e.SpawnEntity(spawnPos, index, levelManager));

    }

    public IEnumerator CreateEnemyAtLocalPosition<E>(E e, float delay, LevelManager manager, bool summoned, Vector3 position, params Action<IEntityBehaviour>[] customEffect) where E : IEntity
    {
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemySpawn", position);
        IEntityBehaviour entity = e.SpawnEntity(position, e.spawnIndex, manager);
        foreach (var item in customEffect)
        {
            item?.Invoke(entity);
        }
        if (entity != null)
        {
            yield return new WaitForSeconds(delay);
            if (summoned)
            {
                summonedEnemies.Add(entity);

            }
            else
                allSpawnedEnemies.Add(entity);
        }

    }


}
