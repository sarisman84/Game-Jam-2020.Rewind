using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slime : Enemy {

    public Slime() : base("Slime")
    {

    }


    LevelManager manager;
    public override IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {
        manager = levelManagerRef;

        return base.SpawnEntity(spawnPos, index, levelManagerRef);
    }

    public override void StartEvent(IEntityBehaviour obj)
    {
        obj.enemySpeed = 10;
        obj.accelerationRate = 15;
    }

    public override void UpdateEvent(IEntityBehaviour obj)
    {



    }

    public override void DamageEvent(IEntityBehaviour obj, BulletBehaivour bullet)
    {
        int amm = Random.Range(1, 3);
        for (int i = 0; i < amm; i++)
        {
            manager.StartCoroutine(manager.waveManager.CreateEnemyAtLocalPosition(new SummonedSlime(), 0.05f, manager, true, ExtendedVector3.RandomPositionWithinRange(obj.transform.position, 1), o => o.transform.localScale = Vector3.one * 0.8f));


        }

        base.DamageEvent(obj, bullet);




    }
}

