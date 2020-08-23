
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaivour : MonoBehaviour {
    new Rigidbody rigidbody;

    public GameObject owner;
    public float bulletVelocity { get; set; }

    public Rigidbody physics => rigidbody;
    public Vector3 lastKnownVelocity { get; private set; }

    public Vector3 currentVelocity { private get; set; }
    Vector3 collisionPosition => transform.position + transform.forward.normalized * (transform.localScale.z + 0.25f);
    Vector3 originalSize = Vector3.zero;

    public Collision ContactPoint { private set; get; }


    public event Action<BulletBehaivour> onUpdateEvent;

    void FixedUpdate()
    {
        if (rigidbody != null)
        {
            if (currentVelocity != Vector3.zero)
                rigidbody.velocity = currentVelocity * bulletVelocity;
            else
                rigidbody.velocity = transform.forward * bulletVelocity;

            if (rigidbody.velocity != Vector3.zero)
            {
                lastKnownVelocity = rigidbody.velocity;
            }
        }

    }



    private void OnCollisionEnter(Collision collision)
    {

        if (collision == null)
        {

            return;
        }
        ContactPoint = collision;
        IDamageable element = collision.gameObject.GetComponent<IDamageable>();
        if (element != null)
        {
            element.TakeDamage(this);
        }

    }



    private void Update()
    {
        onUpdateEvent?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(collisionPosition, 0.30f);
    }




    public void Setup(Vector3 position, Quaternion rotation, float bulletVelocity = 25f)
    {
        rigidbody = rigidbody ?? GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;


        currentVelocity = Vector3.zero;
        transform.position = position;
        transform.rotation = rotation;
        this.bulletVelocity = bulletVelocity;
    }

    public static BulletBehaivour InitializeBullet(GameObject owner, Vector3 firePosition, Quaternion fireRotation)
    {
        EffectsManager.GetInstance.CurrentAudioFiles.PlayAudioClip("PlayerShoot", c =>
        {
            float result = Random.Range(1f, 2f);
            c.Player.time = 0;
            if (TimeHandler.GetInstance.isRewinding)
            {
                result = Random.Range(-1f, -0.5f);
                c.Player.time = c.Player.clip.length / 4f;
            }
            c.Player.pitch = result;
        });

        EffectsManager.GetInstance.CurrentParticleEffects.PlayParticleEffectAt("BulletEffect", firePosition, false, c =>
        {
            c.prefab.transform.rotation = fireRotation;
        });
        BulletBehaivour bullet = ObjectPooler.GetPooledObject<BulletBehaivour>();
        bullet.gameObject.SetActive(true);
        bullet.Setup(firePosition, fireRotation);
        bullet.owner = owner;
        return bullet;
    }
}
