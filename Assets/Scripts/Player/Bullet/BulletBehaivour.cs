using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaivour : MonoBehaviour
{
    new Rigidbody rigidbody;

    float bulletVelocity { get; set; }
    RaycastHit collisionInfo;


    void FixedUpdate()
    {
        CollisionCheck();
        rigidbody.velocity = transform.forward * bulletVelocity;
    }

    private void CollisionCheck(float distanceCheck = 2f)
    {
        //If we have collided with something, affect the bullet
        if (Physics.Raycast(transform.position, transform.forward.normalized * distanceCheck, out collisionInfo))
        {
            //Check if the collider we hit contains a BounceTile component. If it has, affect the bullet's velocity so that it bounces of the wall.
            //collisionInfo.collider.GetComponent<BounceTile>()

           

            //If none of the above conditions are met, disable the bullet.
            rigidbody.velocity = Vector3.zero;
            gameObject.SetActive(false);


            //Check if the collider we hit contains a EnemyBehaivour component. If it has, affect the EnemyBehaivour's logic.
            //collisionInfo.collider.GetComponent<EnemyBehaivour>();

        }
    }

    public void Setup(GameObject aimGameObject)
    {
        rigidbody = rigidbody ?? GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        transform.position = aimGameObject.transform.GetChild(1).position;
        transform.forward = aimGameObject.transform.forward;
        bulletVelocity = 10f;

    }
}
