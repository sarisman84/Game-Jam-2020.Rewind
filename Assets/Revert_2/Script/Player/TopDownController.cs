using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Revert_2.Script.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class TopDownController : MonoBehaviour
    {
        public InputActionReference movementInput, aimInput, fireInput;
        public Transform aimBarrel;

        public float accelerationRate, deccelerationRate, maxMovementSpeed;


        private Rigidbody physics;
        private Camera _camera;

        private WeaponHandler handler;

        private void Awake()
        {
            _camera = Camera.main;
            physics = GetComponent<Rigidbody>();
            handler = new WeaponHandler("Entities/Bullet", gameObject);
        }

        private void Update()
        {
            AimPlayerWeaponTowards(MouseDirection(_camera, aimInput), 0.5f);

            if (fireInput.action.ReadValue<float>().Equals(1) && aimBarrel != null) handler.FireWeapon(0.3f, aimBarrel);
        }

        private Vector3 MouseDirection(Camera camera, InputActionReference mousePos)
        {
            var groundPlane = new Plane(Vector3.up, -transform.position.y);
            var mouseRay = camera.ScreenPointToRay(mousePos.action.ReadValue<Vector2>());
            float hitDistance;
            Color rayColor = Color.red;
            if (groundPlane.Raycast(mouseRay, out hitDistance))
            {
                rayColor = Color.green;

                Debug.DrawRay(mouseRay.origin, mouseRay.direction, rayColor);
                return (mouseRay.GetPoint(hitDistance) - transform.position).normalized;
            }

            Debug.DrawRay(mouseRay.origin, mouseRay.direction, rayColor);
            return Vector3.zero;
        }

        public void AimPlayerWeaponTowards(Vector3 direction, float rotationSpeed)
        {
            if (aimBarrel == null) return;
            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            if (Double.IsNaN(targetRotation.x)) return;
            var finalRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed + 1f);
            aimBarrel.localRotation = finalRotation;

            Debug.DrawRay(aimBarrel.transform.position, aimBarrel.transform.forward.normalized * 100f, Color.cyan);
        }

        private void FixedUpdate()
        {
            MovePlayer(movementInput.action.ReadValue<Vector2>().ToVector3(CustomVector3.Axis.ZX), accelerationRate,
                deccelerationRate
                , maxMovementSpeed);
        }

        public void MovePlayer(Vector3 direction, float accelerationRate, float deccelerationRate,
            float maxMovementSpeed)
        {
            accelerationRate *= 10f;
            maxMovementSpeed *= 10f;
            if (direction == Vector3.zero)
            {
                physics.velocity = Vector3.Lerp(physics.velocity, Vector3.zero, deccelerationRate);
            }
            else
                physics.velocity = CustomVector3.AddWithClampedMagnitude(physics.velocity,
                    direction * accelerationRate * Time.fixedDeltaTime,
                    maxMovementSpeed);
        }


        private void OnEnable()
        {
            movementInput.action.Enable();
            fireInput.action.Enable();
            aimInput.action.Enable();
        }

        private void OnDisable()
        {
            movementInput.action.Disable();
            fireInput.action.Disable();
            aimInput.action.Disable();
        }
    }
}