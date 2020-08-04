using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour, IDamageable {
    // Start is called before the first frame update
    public event Action<EnemyBehaviour> onDamageEvent;
    public event Action<EnemyBehaviour> onStartEvent;
    public event Action<EnemyBehaviour> onUpdateEvent;


    public bool overrideUpdate { private get; set; }
    public float enemySpeed { private get; set; } = 3.5f;
    public float turningSpeed { private get; set; } = 120f;

    public float accelerationRate { private get; set; } = 8f;

    public void TakeDamage()
    {
        onDamageEvent?.Invoke(this);
    }


    private void Start()
    {
        onStartEvent?.Invoke(this);
    }

    private void Update()
    {
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
    private void SetNavMeshTarget()
    {
        agent = agent ?? GetComponent<NavMeshAgent>();
        player = player ?? FindObjectOfType<PlayerController>();

        agent.speed = enemySpeed;
        agent.angularSpeed = turningSpeed;

        agent.SetDestination(player.transform.position);
    }
}
