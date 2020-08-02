using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour, IDamageable
{
    // Start is called before the first frame update
    public event Action<EnemyBehaviour> onDamageEvent;

    public void TakeDamage()
    {
        onDamageEvent?.Invoke(this);
    }
}
