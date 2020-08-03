using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LevelManager;

namespace Assets.Enemy {
    public class Enemy {
        Vector2Int currentPos;
        public Enemy(Vector2Int index)
        {
            GameObject newPill = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            EnemyBehaviour enemy = newPill.AddComponent<EnemyBehaviour>();
            enemy.onDamageEvent += BasicEffect;
            newPill.transform.position = LevelManager.GetInstance.PlayArea[index.x, index.y].GetWorldPosition(newPill);
            obj = newPill;
            LevelManager.GetInstance.PlayArea[index.x, index.y].entity = newPill;

            currentPos = index;
        }

        private void BasicEffect(EnemyBehaviour obj)
        {

            TimeHandler.GetInstance.ConfirmAllEnemyDeaths();
            LevelManager.GetInstance.PlayArea[currentPos.x, currentPos.y].RemoveEntity();
            
            obj.gameObject.SetActive(false);
            obj.onDamageEvent -= BasicEffect;


        }





        public GameObject obj;
    }
}
