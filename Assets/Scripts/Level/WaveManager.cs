using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using TMPro;

public class WaveManager {

    public struct EntityType {
        public IEntity type;
        public int waveSpawnLimit;

        public EntityType(IEntity item, int maxWaveSpawn)
        {
            type = item;
            waveSpawnLimit = maxWaveSpawn;
        }
    }


    public int currentWave { get; set; } = 0;
    public List<IEntityBehaviour> allSpawnedEnemies = new List<IEntityBehaviour>();
    public List<EntityType> allEntityTypes = new List<EntityType>();
    public List<IEntityBehaviour> summonedEnemies = new List<IEntityBehaviour>();

    private const int BOSSWAVE = 9;

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
    Coroutine nextWave;
    public void AttemptToGoToNextWave()
    {
        if (!areAllEnemiesDead) return;
        if (nextWave != null)
            levelManagerRef.StopCoroutine(nextWave);
        levelManagerRef.hasAlreadyRewinded = false;
        nextWave = levelManagerRef.StartCoroutine(levelManagerRef.waveManager.DeployNextWave());
        TimeHandler.GetInstance.isRewinding = false;
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
        summonedEnemies.Clear();
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

            allEntityTypes.Add(new EntityType(item, 40));
        }

        currentWave++;
        Debug.Log($"Current Wave (at DeployFirstWave):{currentWave}");
        waveCounter.text = $"Wave:{currentWave}";
    }


    public IEnumerator DeployNextWave()
    {

        if (currentWave % 4 == 0)
        {
            yield return TimeHandler.GetInstance.DelayedClearRecordings();
        }

        if (currentWave % 20 == 0)
        {
            IEnumerator ClearEntities(IEntityBehaviour behaviour)
            {
                if (behaviour.gameObject.activeSelf)
                {
                    behaviour.gameObject.SetActive(false);
                    yield return new WaitForSeconds(0.1f);
                }
                levelManagerRef.PlayArea[behaviour.parentClass.spawnIndex.x, behaviour.parentClass.spawnIndex.y].RemoveEntity();
                yield return null;
            }

            yield return allSpawnedEnemies.ExecuteAction(ClearEntities);
            allSpawnedEnemies.Clear();
        }

        TimeHandler.GetInstance.latestList++;
        summonedEnemies.ExecuteAction(s => s.gameObject.SetActive(false));
        summonedEnemies.Clear();


        yield return new WaitForSeconds(1.35f);



        Annoy_O_Tron tron = new Annoy_O_Tron();
        CarpetBomber bomber = new CarpetBomber();

        AddEntityType(new BouncyWall(), 4, int.MaxValue);
        AddEntityType(tron, 2, int.MaxValue);
        AddEntityType(new Slime(), 4, int.MaxValue);
        AddEntityType(bomber, 6, int.MaxValue);
        AddEntityType(new Turret(), 8, int.MaxValue);
        AddEntityType(new Boss(), 20, int.MaxValue);



        IEnumerator RevivePreviousEnemies(IEntityBehaviour e)
        {
            (e as EnemyBehaviour)?.ResetPositionToSpawn();
            e.AssignEvents(e.parentClass);
            e.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            DisplayAllLivingEnemies();

        }
        yield return allSpawnedEnemies.ExecuteAction(RevivePreviousEnemies);

        IEnumerator AddExtraEnemies(EntityType e)
        {
            if (e.waveSpawnLimit <= currentWave && e.waveSpawnLimit != 0) goto exit;
            int amm = Random.Range(0, 1);
            if (currentWave % 20 == 0)
                amm = Random.Range(4, 7);
            if (currentWave < 5)
            {
                amm = Random.Range(1, 3);
            }
            for (int i = 0; i < amm; i++)
            {
                yield return CreateEnemyAtRandomPosition(e.type, 0.05f, levelManagerRef);
                DisplayAllLivingEnemies();

            }
        exit: yield return null;

        }
        yield return allEntityTypes.ExecuteAction(AddExtraEnemies);

        if (currentWave % BOSSWAVE == 0)
        {
            addBosses();
        }

        currentWave++;
        Debug.Log($"Current Wave (at DeployNextWave):{currentWave}");
        waveCounter.text = $"Wave:{currentWave}";


    }

    private void AddEntityType<E>(E entity, int minWaveSpawn, int maxWaveSpawn) where E : IEntity
    {
        if (!allEntityTypes.Contains(entity))
            if (currentWave >= minWaveSpawn)
            {
                allEntityTypes.Add(new EntityType(entity, maxWaveSpawn));
            }
    }

    public void addBosses()
    {
        for (int i = 0; i < currentWave / BOSSWAVE; i++)
        {
            CreateBossAtRandomPosition(new Boss(), levelManagerRef);
        }
    }


    void CreateBossAtRandomPosition<E>(E boss, LevelManager levelManager) where E : IEntity
    {
        Vector2Int index = levelManagerRef.GetGetRandomGridPosition();
        Vector3 spawnPos = levelManagerRef.PlayArea[index.x, index.y].GetWorldPosition();
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemySpawn", spawnPos);
        summonedEnemies.Add(boss.SpawnEntity(spawnPos, index, levelManager));
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
