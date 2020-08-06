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

    public override EntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index)
    {

        return base.SpawnEntity(spawnPos, index);
    }
    public override void DamageEvent(EntityBehaviour obj, BulletBehaivour incomingBullet)
    {
        void p(BulletBehaivour bullet)
        {
            Collision contactPoint = incomingBullet.ContactPoint;
            if (contactPoint == null) return;
            var speed = 10f;
            Debug.Log(contactPoint.contacts[0]);
            var direction = Vector3.Reflect(incomingBullet.physics.velocity, contactPoint.contacts[0].normal);

            incomingBullet.physics.velocity = direction * speed;
            incomingBullet.onCollisionEvent -= p;
        }

        incomingBullet.onCollisionEvent += p;


    }




    public override void StartEvent(EntityBehaviour obj)
    {
        base.StartEvent(obj);
    }


    public override void UpdateEvent(EntityBehaviour obj)
    {
        base.UpdateEvent(obj);
    }
}

