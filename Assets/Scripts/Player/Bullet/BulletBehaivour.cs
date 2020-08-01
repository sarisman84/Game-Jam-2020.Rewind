using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaivour : MonoBehaviour
{
    Rigidbody rigidbody;

    public float bulletVelocity { private get; set; }
    void OnEnable()
    {
        rigidbody = rigidbody ?? GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }


    void FixedUpdate()
    {
        rigidbody.MovePosition(transform.forward * Time.fixedDeltaTime * bulletVelocity);
    }



}
