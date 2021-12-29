using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{

    public float revSpeed;
    public float runSpeed;
    private Rigidbody2D body;

    void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var impulse = revSpeed * body.inertia;
        body.AddTorque(impulse * Time.fixedDeltaTime);
    }
}
