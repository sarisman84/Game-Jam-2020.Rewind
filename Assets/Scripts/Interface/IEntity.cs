using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    IEntityBehaviour SpawnEntity(Vector3 spawnPos, Vector2Int index, LevelManager levelManagerRef);

    void DamageEvent(IEntityBehaviour obj, BulletBehaivour bullet);

    void StartEvent(IEntityBehaviour obj);

    void UpdateEvent(IEntityBehaviour obj);
}
