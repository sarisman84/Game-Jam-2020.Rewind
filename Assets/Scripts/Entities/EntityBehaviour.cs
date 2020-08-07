using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;

public class EntityBehaviour : MonoBehaviour, IDamageable, IEntityBehaviour {
    internal IEntity _parentClass;
    internal Vector3 spawnPos;

    #region Unused Properties
    Vector3 IEntityBehaviour.spawnPos { set => spawnPos = value; }
    public string onDeathParticle { set => throw new NotImplementedException(); }

    public bool activeSelf => throw new NotImplementedException();

    public bool overrideUpdate { set => throw new NotImplementedException(); }
    public float enemySpeed { set => throw new NotImplementedException(); }
    public float turningSpeed { set => throw new NotImplementedException(); }
    public float accelerationRate { set => throw new NotImplementedException(); }
    public float attackRange { set => throw new NotImplementedException(); }

    public NavMeshAgent agent => throw new NotImplementedException();

    public PlayerController foundPlayer => throw new NotImplementedException();

    public IEntity parentClass { set => _parentClass = value; get => _parentClass; }
    public Action<IEntityBehaviour, BulletBehaivour> onDamageEvent { get; set; }
    public Action<IEntityBehaviour> onStartEvent { get; set; }
    public Action<IEntityBehaviour> onUpdateEvent { get; set; }

    #endregion

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

    public void AssignEvents(IEntity entity)
    {
        onDamageEvent += entity.DamageEvent;
        onUpdateEvent += entity.UpdateEvent;
        onStartEvent += entity.StartEvent;
    }

    public void TakeDamage(BulletBehaivour bullet)
    {
        if (onDamageEvent == null)
        {
            bullet.physics.velocity = Vector3.zero;
            bullet.gameObject.SetActive(false);
            return;
        }
        onDamageEvent.Invoke(this, bullet);
    }
}


