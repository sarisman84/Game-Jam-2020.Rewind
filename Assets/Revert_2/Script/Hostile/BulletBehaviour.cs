using System;
using UnityEngine;

namespace Revert_2.Script.Hostile
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulletBehaviour : MonoBehaviour
    {
        public struct BulletInfo
        {
            public float Lifespan;
            public float Velocity;
            public float Damage;
            public int OwnerID;


            public BulletInfo(float lifespan, float velocity, float damage, int ownerID)
            {
                Lifespan = lifespan;
                Velocity = velocity;
                Damage = damage;
                OwnerID = ownerID;
            }

            public void Reset()
            {
                Lifespan = 0;
                Velocity = 0;
                Damage = 0;
                OwnerID = -1;
            }

            public void UpdateInfo(BulletInfo info)
            {
                Lifespan = info.Lifespan;
                Velocity = info.Velocity;
                Damage = info.Damage;
                OwnerID = info.OwnerID;
            }
        }

        private Rigidbody _rigidbody;

        private void OnEnable()
        {
            _rigidbody = _rigidbody ? _rigidbody : GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        private void FixedUpdate()
        {
            _rigidbody.velocity += transform.forward.normalized * (BulletInformation.Velocity * Time.fixedDeltaTime);
        }

        public BulletInfo BulletInformation;

        public void Setup(BulletInfo information, Vector3 firePos, Quaternion fireRot)
        {
            BulletInformation.UpdateInfo(information);

            _rigidbody.velocity = Vector3.zero;
            transform.position = firePos;
            transform.rotation = fireRot;
        }
    }
}