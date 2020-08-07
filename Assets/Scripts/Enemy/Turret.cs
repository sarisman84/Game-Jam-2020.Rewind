using UnityEngine;
using Object = UnityEngine;
using System;

public class Turret : Enemy
{
    private float fireInterval = 5;
    private float timeSinceLastShot = 0;

    public Turret() : base("Turret")
    {
        timeSinceLastShot = new System.Random().Next((int)fireInterval);       
    }

    public override IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {
        return base.SpawnEntity(spawnPos, index, levelManagerRef);
    }

    public override void StartEvent(IEntityBehaviour obj)
    {
        obj.overrideUpdate = true;
        obj.enemySpeed = 0;
        obj.accelerationRate = 0;        
    }

    public override void UpdateEvent(IEntityBehaviour obj)
    {        
        timeSinceLastShot = timeSinceLastShot.CountTime(fireInterval);
        if (timeSinceLastShot >= fireInterval)
        {
            Vector3 player = obj.foundPlayer.transform.position;
            Vector3 turret = obj.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation((player - turret).normalized, Vector3.up);

            Transform aim = obj.transform.GetChild(0);
            aim.localRotation = targetRotation;
            Transform barrel = aim.GetChild(0);

            BulletBehaivour.InitializeBullet(barrel.position, targetRotation);
            timeSinceLastShot = 0;
        }
    }


}

