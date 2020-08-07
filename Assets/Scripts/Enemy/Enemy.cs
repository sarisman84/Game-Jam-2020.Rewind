using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LevelManager;
using Object = UnityEngine;
using Westwind.Scripting;


public class Enemy : Entity {
    public Enemy(string modelPath) : base(modelPath)
    {
    }

    LevelManager levelManagerRef;
    public EnemyBehaviour SpawnEnemy(Vector3 spawnPos, Vector2Int index, LevelManager levelManager)
    {
        levelManagerRef = levelManager;
        EnemyBehaviour enemy = ObjectPooler.GetPooledObject(Resources.Load<EnemyBehaviour>($"Enemies/{modelPath}"));

        enemy.parentClass = this;
        enemy.AssignEvents(this);
        enemy.spawnPos = spawnPos;
        enemy.transform.position = spawnPos;
        enemy.gameObject.SetActive(true);
        levelManager.PlayArea[index.x, index.y].entity = enemy.gameObject;
        spawnIndex = index;
        return enemy;
    }



    public virtual void UpdateEvent(EnemyBehaviour obj)
    {

    }

    public virtual void StartEvent(EnemyBehaviour obj)
    {


    }

    public virtual void DamageEvent(EnemyBehaviour obj, BulletBehaivour bullet)
    {
        obj.gameObject.SetActive(false);
        TimeHandler.GetInstance.ConfirmAllEnemyDeaths(levelManagerRef);

        bullet.physics.velocity = Vector3.zero;
        bullet.gameObject.SetActive(false);

        obj.onDamageEvent -= DamageEvent;
        obj.onStartEvent -= StartEvent;
        obj.onUpdateEvent -= UpdateEvent;



    }





}

