
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour, IDamageable, IEntityBehaviour {
    // Start is called before the first frame update
    event Action<IEntityBehaviour, BulletBehaivour> _onDamageEvent;
    event Action<IEntityBehaviour> _onStartEvent;
    event Action<IEntityBehaviour> _onUpdateEvent;

    public IEntity _parentClass;



    public Vector3 spawnPos { private get; set; }

    public string onDeathParticle { private get; set; } = "EnemyDeath";
    public bool activeSelf => gameObject.activeSelf;
    public bool overrideUpdate { private get; set; }
    public float enemySpeed { private get; set; } = 3.5f;
    public float turningSpeed { private get; set; } = 120f;
    public float accelerationRate { private get; set; } = 8f;

    public float attackRange { private get; set; } = 3f;

    public NavMeshAgent agent { get; private set; }
    public PlayerController foundPlayer { get; private set; }
    public IEntity parentClass { set => _parentClass = value; get => _parentClass; }
    public Action<IEntityBehaviour, BulletBehaivour> onDamageEvent { get => _onDamageEvent; set => _onDamageEvent = value; }
    public Action<IEntityBehaviour> onStartEvent { get => _onStartEvent; set => _onStartEvent = value; }
    public Action<IEntityBehaviour> onUpdateEvent { get => _onUpdateEvent; set => _onUpdateEvent = value; }

    public void TakeDamage(BulletBehaivour bullet)
    {
        onDamageEvent?.Invoke(this, bullet);
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt(onDeathParticle, transform.position);
    }


    private void Start()
    {
        agent = agent ?? GetComponent<NavMeshAgent>();
        foundPlayer = foundPlayer ?? FindObjectOfType<PlayerController>();


        onStartEvent?.Invoke(this);
        agent.speed = enemySpeed;
        agent.angularSpeed = turningSpeed;
    }

    private void OnEnable()
    {
        onStartEvent?.Invoke(this);
        if (agent == null) return;
        agent.speed = enemySpeed;
        agent.angularSpeed = turningSpeed;
    }
    float attackDelay = 1.5f, localTimer;
    private void Update()
    {
        localTimer += Time.deltaTime;
        localTimer = Mathf.Clamp(localTimer, 0, attackDelay);
        onUpdateEvent?.Invoke(this);
        if (!overrideUpdate)
            SetNavMeshTarget();



    }


    public void SetNavMeshTarget()
    {


        if ((foundPlayer.transform.position - transform.position).magnitude > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(foundPlayer.transform.position);
        }

        else
        {
            agent.isStopped = true;
            if (localTimer == attackDelay)
            {
                foundPlayer.TakeDamage(null);
                localTimer = 0;
            }

        }
    }

    public void AssignEvents(IEntity enemy)
    {
        onDamageEvent += enemy.DamageEvent;
        onStartEvent += enemy.StartEvent;
        onUpdateEvent += enemy.UpdateEvent;
    }

    public void ResetPositionToSpawn()
    {
        transform.position = spawnPos;
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("EnemySpawn", spawnPos);
    }
}
