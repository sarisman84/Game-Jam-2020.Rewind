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

    public EnemyBehaviour SpawnEnemy(Vector3 spawnPos, Vector2Int index)
    {
        EnemyBehaviour enemy = ObjectPooler.GetPooledObject(Resources.Load<EnemyBehaviour>($"Enemies/{modelPath}"));

        enemy.parentClass = this;
        enemy.AssignEvents(this);
        enemy.spawnPos = spawnPos;
        enemy.transform.position = spawnPos;
        enemy.gameObject.SetActive(true);
        GetInstance.PlayArea[index.x, index.y].entity = enemy.gameObject;
        spawnIndex = index;
        return enemy;
    }



    public virtual void UpdateEvent(EnemyBehaviour obj)
    {

    }

    public virtual void StartEvent(EnemyBehaviour obj)
    {


    }

    public virtual void DamageEvent(EnemyBehaviour obj)
    {
        obj.gameObject.SetActive(false);
        TimeHandler.GetInstance.ConfirmAllEnemyDeaths();



        obj.onDamageEvent -= DamageEvent;
        obj.onStartEvent -= StartEvent;
        obj.onUpdateEvent -= UpdateEvent;



    }





}

