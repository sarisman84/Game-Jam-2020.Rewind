using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CarpetBomber : Enemy {

    public CarpetBomber() : base("Carpet Bomber")
    {

    }



    public override IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {

        return base.SpawnEntity(spawnPos, index, levelManagerRef);
    }

    public override void DamageEvent(IEntityBehaviour obj, BulletBehaivour bullet)
    {

        base.DamageEvent(obj, bullet);
        bullet.physics.velocity = Vector3.zero;
        bullet.gameObject.SetActive(false);

    }

    Vector3 previousPosition;

    public override void StartEvent(IEntityBehaviour obj)
    {

        obj.enemySpeed = 5f;
        obj.turningSpeed = 10f;
        if (obj != null && obj.foundPlayer != null)
            obj.transform.rotation = Quaternion.LookRotation((obj.transform.position - obj.foundPlayer.transform.position).normalized, Vector3.up);
        previousPosition = Vector3.zero;


    }

    float localTimer = 0;
    float bombingRate = 1f;



    public override void UpdateEvent(IEntityBehaviour obj)
    {
        localTimer = localTimer.CountTime(bombingRate);
        if (localTimer == bombingRate)
        {
            if (previousPosition != Vector3.zero)
            {
                BulletBehaivour bullet = ObjectPooler.GetPooledObject<BulletBehaivour>();
                bullet.Setup(previousPosition, obj.transform.rotation, 0);
                bullet.gameObject.SetActive(true);
                EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("BulletSpawn", bullet.transform.position);
            }
            previousPosition = obj.transform.position;
            localTimer = 0;

        }

    }


}

