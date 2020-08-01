using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float Speed = 1;

    CharacterController controller;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = InputManager.Singleton.GetMovement(Speed);
        
        controller.Move(movement);
    }
}