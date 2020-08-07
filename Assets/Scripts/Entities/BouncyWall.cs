using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BouncyWall : Entity {

    public BouncyWall(string name = "Bouncy Wall") : base(name)
    {

    }

    public override IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {

        return base.SpawnEntity(spawnPos, index, levelManagerRef);
    }
    public override void DamageEvent(IEntityBehaviour obj, BulletBehaivour incomingBullet)
    {


        Collision contactPoint = incomingBullet.ContactPoint;

        if (contactPoint.gameObject.GetComponent<EntityBehaviour>() != obj) return;

        var direction = Vector3.Reflect(incomingBullet.lastKnownVelocity.normalized, contactPoint.contacts[0].normal);

        incomingBullet.currentVelocity = direction.normalized;







    }




    public override void StartEvent(IEntityBehaviour obj)
    {
        base.StartEvent(obj);
    }


    public override void UpdateEvent(IEntityBehaviour obj)
    {
        base.UpdateEvent(obj);
    }
}

