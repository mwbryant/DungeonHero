using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DualStickMovement : MonoBehaviour
{
    public float baseRotationForce;
    public float baseRunForce;
    [Space()]
    public float speedRampFactor;
    public float minAngularDrag;
    [Tooltip("The force that the player tries to face the movement direction with when not spinning")]
    public float facingForce;
    public float forwardSpeedRampFactor;
    public float speedMultCap;

    private Rigidbody2D body;
    private float startAngularDrag;
    private float currentSpeedUp = 1;
    private Vector2 lastDirection;
    private PlayerInput input;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        startAngularDrag = body.angularDrag;
        input = GetComponent<PlayerInput>();
    }

    void FixedUpdate() {
        Spin();
        Move();
    }

    void Spin() {
        float spin_input = input.actions["Spin"].ReadValue<float>();

        if (spin_input == 0) {
            body.angularDrag = startAngularDrag;
        } else {
            body.angularDrag *= speedRampFactor;
            if (body.angularDrag < minAngularDrag) {
                body.angularDrag = minAngularDrag;
            }
        }

        var impulse = baseRotationForce * body.inertia;
        body.AddTorque(spin_input * impulse * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    void Move() {
        float spin_input = input.actions["Spin"].ReadValue<float>();
        Vector2 direction = input.actions["Move"].ReadValue<Vector2>();

        if (direction.magnitude > 1.0f) {
            direction.Normalize();
        }

        //TODO test with joystick
        if (Vector2.Dot(direction, lastDirection) > .99) {
            currentSpeedUp += forwardSpeedRampFactor * Time.fixedDeltaTime;
        } else {
            currentSpeedUp = 1;
        }
        if (currentSpeedUp > speedMultCap) {
            currentSpeedUp = speedMultCap;
        }

        body.AddForce(direction * baseRunForce * currentSpeedUp * Time.fixedDeltaTime, ForceMode2D.Force);
        lastDirection = direction;

        //Not spinning then turn toward movement
        //Safe to compare to 0 because deadzone filtering
        if (spin_input == 0) {
            Vector2 desired_direction = direction.normalized;
            Vector2 facing = transform.right;

            float dot = Vector2.Dot(facing, desired_direction);

            var impulse = baseRotationForce * body.inertia;
            body.AddTorque(dot * facingForce * impulse * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

}
