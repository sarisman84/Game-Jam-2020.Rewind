using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Enemy
{
    public class Enemy
    {
        public Enemy(Vector3 pos)
        {
            GameObject newPill = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            EnemyBehaviour enemy = newPill.AddComponent<EnemyBehaviour>();
            enemy.onDamageEvent += BasicEffect;
            newPill.transform.position = pos;
            obj = newPill;
        }

        private void BasicEffect(EnemyBehaviour obj)
        {
            obj.gameObject.SetActive(false);
            obj.onDamageEvent -= BasicEffect;
        }



        public GameObject obj;
    }
}
