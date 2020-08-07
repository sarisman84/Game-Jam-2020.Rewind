using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SummonedSlime : Enemy {

    public SummonedSlime() : base("Summoned Slime")
    {

    }


   
    public override IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {
        

        return base.SpawnEntity(spawnPos, index, levelManagerRef);
    }

    public override void StartEvent(IEntityBehaviour obj)
    {
        obj.enemySpeed = 30;
        obj.accelerationRate = 20;
    }

    

    public override void DamageEvent(IEntityBehaviour obj, BulletBehaivour bullet)
    {


        base.DamageEvent(obj, bullet);




    }
}

