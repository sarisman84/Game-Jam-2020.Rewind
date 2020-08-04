using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour, IDamageable {
    // Start is called before the first frame update
    public event Action<EnemyBehaviour> onDamageEvent;
    public event Action<EnemyBehaviour> onStartEvent;
    public event Action<EnemyBehaviour> onUpdateEvent;

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
        onUpdateEvent?.Invoke(this);
    }
}
