using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Entity {
    public Vector2Int spawnIndex;

    string modelPath;
    public Entity(string modelPath)
    {
        this.modelPath = modelPath;

    }

    public EntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index)
    {
        EntityBehaviour entity = ObjectPooler.GetPooledObject(Resources.Load<EntityBehaviour>($"Enemies/{modelPath}"));

        entity.parentClass = this;
        entity.AssignEvents(this);
        entity.spawnPos = spawnPos;
        entity.transform.position = spawnPos;
        entity.gameObject.SetActive(true);
        LevelManager.GetInstance.PlayArea[index.x, index.y].entity = entity.gameObject;
        spawnIndex = index;
        return entity;
    }



    public virtual void UpdateEvent(EntityBehaviour obj)
    {

    }

    public virtual void StartEvent(EntityBehaviour obj)
    {


    }

    public virtual void DamageEvent(EntityBehaviour obj)
    {




        obj.onDamageEvent -= DamageEvent;
        obj.onStartEvent -= StartEvent;
        obj.onUpdateEvent -= UpdateEvent;



    }

}

