using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Entity: IEntity {
    public Vector2Int spawnIndex;

    protected string modelPath;
    public Entity(string modelPath)
    {
        this.modelPath = modelPath;

    }


    LevelManager levelManagerRef;

    public virtual IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef)
    {
        IEntityBehaviour entity = ObjectPooler.GetPooledObject<EntityBehaviour>(Resources.Load<GameObject>($"Entity/{modelPath}"));

        entity.parentClass = this;
        entity.AssignEvents(this);
        entity.spawnPos = spawnPos;
        entity.transform.position = spawnPos;
        entity.gameObject.SetActive(true);
        levelManagerRef.PlayArea[index.x, index.y].entity = entity.gameObject;
        spawnIndex = index;
        return entity;
    }



    public virtual void UpdateEvent(IEntityBehaviour obj)
    {

    }

    public virtual void StartEvent(IEntityBehaviour obj)
    {


    }

    public virtual void DamageEvent(IEntityBehaviour obj, BulletBehaivour incomingBullet)
    {




        obj.onDamageEvent -= DamageEvent;
        obj.onStartEvent -= StartEvent;
        obj.onUpdateEvent -= UpdateEvent;



    }

    
}

