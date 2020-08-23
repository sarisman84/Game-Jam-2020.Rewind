using UnityEngine;
using Object = UnityEngine;
using System;

public class Boss : Enemy {
    private const float FIREINTERVAL = 0.2f;
    private float timeSinceLastShot = 0;
    private const float INITIALDELAY = 2f;
    private const float ROTATIONSPEED = 0.01f;
    private bool spawnDelay = true;

    public int currentHealth;
    public int maxHealth;

    public Boss() : base("Boss")
    {
        maxHealth = 30;
    }

    public override IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {
        currentHealth = maxHealth;
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
        if (spawnDelay)
        {
            timeSinceLastShot = timeSinceLastShot.CountTime(INITIALDELAY);
            if (timeSinceLastShot >= INITIALDELAY)
                spawnDelay = false;
        }
        else
        {
            timeSinceLastShot = timeSinceLastShot.CountTime(FIREINTERVAL);

            if (timeSinceLastShot >= FIREINTERVAL)
            {
                Vector3 player = obj.foundPlayer.transform.position;
                Vector3 turret = obj.transform.position;

                //rotate
                Quaternion targetRotation = Quaternion.LookRotation((player - turret).normalized, Vector3.up);
                Transform aim = obj.transform.GetChild(0);
                aim.localRotation = targetRotation;
                targetRotation = Quaternion.Lerp(aim.localRotation, targetRotation, ROTATIONSPEED);

                //shoot
                Transform barrel = aim.GetChild(0);
                BulletBehaivour.InitializeBullet(obj.gameObject, barrel.position, targetRotation);

                timeSinceLastShot = 0;
            }
        }
    }

    public override void DamageEvent(IEntityBehaviour obj, BulletBehaivour bullet)
    {
        if (currentHealth <= 0)
            base.DamageEvent(obj, bullet);
        currentHealth -= 1;
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("PlayerHit", obj.transform.position);

    }


}

