using UnityEngine;
using System.Collections;
using System;

public class EntityBehaviour : MonoBehaviour, IDamageable {
    internal Entity parentClass;
    internal Vector3 spawnPos;
    internal Action<EntityBehaviour, BulletBehaivour> onDamageEvent;
    internal Action<EntityBehaviour> onStartEvent;
    internal Action<EntityBehaviour> onUpdateEvent;

    // Use this for initialization
    void Start()
    {
        onStartEvent?.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        onUpdateEvent?.Invoke(this);
    }

    internal void AssignEvents(Entity entity)
    {
        onDamageEvent += entity.DamageEvent;
        onUpdateEvent += entity.UpdateEvent;
        onStartEvent += entity.StartEvent;
    }

    public void TakeDamage(BulletBehaivour bullet)
    {
        if(onDamageEvent == null)
        {
            bullet.physics.velocity = Vector3.zero;
            bullet.gameObject.SetActive(false);
            return;
        }
        onDamageEvent.Invoke(this, bullet);
    }
}
