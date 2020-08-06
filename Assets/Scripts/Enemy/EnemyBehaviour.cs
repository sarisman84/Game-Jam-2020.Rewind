
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour, IDamageable {
    // Start is called before the first frame update
    public event Action<EnemyBehaviour, BulletBehaivour> onDamageEvent;
    public event Action<EnemyBehaviour> onStartEvent;
    public event Action<EnemyBehaviour> onUpdateEvent;

    public Enemy parentClass;



    public Vector3 spawnPos { private get; set; }

    public string onDeathParticle { private get; set; } = "EnemyDeath";
    public bool activeSelf => gameObject.activeSelf;
    public bool overrideUpdate { private get; set; }
    public float enemySpeed { private get; set; } = 3.5f;
    public float turningSpeed { private get; set; } = 120f;
    public float accelerationRate { private get; set; } = 8f;

    public void TakeDamage(BulletBehaivour bullet)
    {
        onDamageEvent?.Invoke(this, bullet);
        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt(onDeathParticle, transform.position);
    }


    private void Start()
    {
        onStartEvent?.Invoke(this);
    }
    float attackDelay = 1.5f, localTimer;
    private void Update()
    {
        localTimer += Time.deltaTime;
        localTimer = Mathf.Clamp(localTimer, 0, attackDelay);

        if (overrideUpdate)
        {
            onUpdateEvent?.Invoke(this);
            return;
        }
        onUpdateEvent?.Invoke(this);
        SetNavMeshTarget();

    }
    NavMeshAgent agent;
    PlayerController player;
    public void SetNavMeshTarget()
    {
        agent = agent ?? GetComponent<NavMeshAgent>();
        player = player ?? FindObjectOfType<PlayerController>();

        agent.speed = enemySpeed;
        agent.angularSpeed = turningSpeed;

        if ((player.transform.position - transform.position).magnitude > 3f)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }

        else
        {
            agent.isStopped = true;
            if (localTimer == attackDelay)
            {
                player.TakeDamage(null);
                localTimer = 0;
            }

        }
    }

    public void AssignEvents(Enemy enemy)
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
