using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Enemy
{
    class Enemy
    {
        public Enemy(Vector3 pos)
        {
            GameObject newBox = GameObject.CreatePrimitive(PrimitiveType.Capsule);//UnityEngine.Object.Instantiate(UnityEngine.GameObject.);
            EnemyBehaviour enemy = newBox.AddComponent<EnemyBehaviour>();
            enemy.onDamageEvent += BasicEffect;
            newBox.transform.position = pos;

        }

        private void BasicEffect(EnemyBehaviour obj)
        {
            obj.gameObject.SetActive(false);
            obj.onDamageEvent -= BasicEffect;
        }
    }
}
