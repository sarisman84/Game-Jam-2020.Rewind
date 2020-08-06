
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaivour : MonoBehaviour {
    new Rigidbody rigidbody;

    public float bulletVelocity { get; set; }

    public Rigidbody physics => rigidbody;
    Vector3 collisionPosition => transform.position + transform.forward.normalized * (transform.localScale.z + 0.25f);
    Vector3 originalSize = Vector3.zero;

    public Collision ContactPoint { private set; get; }

    public event Action<BulletBehaivour> onCollisionEvent;

    void FixedUpdate()
    {
        if (rigidbody != null)
        {

            rigidbody.velocity = transform.forward * bulletVelocity;
        }

    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null || rigidbody == null) return;
        ContactPoint = collision;
        if (onCollisionEvent == null)
        {
            rigidbody.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
        else
        {
            onCollisionEvent.Invoke(this);
        }

        IDamageable element = collision.gameObject.GetComponent<IDamageable>();
        if (element != null)
        {
            element.TakeDamage(this);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(collisionPosition, 0.30f);
    }

    private void OnDisable()
    {
        
    }


    public void Setup(Vector3 position, Quaternion rotation, float bulletVelocity = 25f)
    {
        rigidbody = rigidbody ?? GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        transform.position = position;
        transform.rotation = rotation;
        this.bulletVelocity = bulletVelocity;
    }
}
