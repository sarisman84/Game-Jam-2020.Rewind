using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public interface IEntityBehaviour : IDamageable {



    GameObject gameObject { get; }
    Transform transform { get; }



    IEntity parentClass { set; get; }
    Vector3 spawnPos { set; }
    string onDeathParticle { set; }
    bool activeSelf { get; }
    bool overrideUpdate { set; }
    float enemySpeed { set; }
    float turningSpeed { set; }
    float accelerationRate { set; }
    float attackRange { set; }
    NavMeshAgent agent { get; }
    PlayerController foundPlayer { get; }

    Action<IEntityBehaviour, BulletBehaivour> onDamageEvent { get; set; }
    Action<IEntityBehaviour> onStartEvent { get; set; }
    Action<IEntityBehaviour> onUpdateEvent { get; set; }

    void AssignEvents(IEntity entity);


}

