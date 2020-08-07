using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Entity {
    public Vector2Int spawnIndex;

    protected string modelPath;
    public Entity(string modelPath)
    {
        this.modelPath = modelPath;

    }


    LevelManager levelManagerRef;

    public virtual EntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {
        EntityBehaviour entity = ObjectPooler.GetPooledObject<EntityBehaviour>(Resources.Load<GameObject>($"Entity/{modelPath}"));

        entity.parentClass = this;
        entity.AssignEvents(this);
        entity.spawnPos = spawnPos;
        entity.transform.position = spawnPos;
        entity.gameObject.SetActive(true);
        levelManagerRef.PlayArea[index.x, index.y].entity = entity.gameObject;
        spawnIndex = index;
        return entity;
    }



    public virtual void UpdateEvent(EntityBehaviour obj)
    {

    }

    public virtual void StartEvent(EntityBehaviour obj)
    {


    }

    public virtual void DamageEvent(EntityBehaviour obj, BulletBehaivour incomingBullet)
    {




        obj.onDamageEvent -= DamageEvent;
        obj.onStartEvent -= StartEvent;
        obj.onUpdateEvent -= UpdateEvent;



    }

}

