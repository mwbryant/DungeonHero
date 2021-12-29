using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Weapon : MonoBehaviour
{
    public int damage;
    public float knockback;

    private Rigidbody2D rb2d;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    //Only knockback on contact
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "enemy")
        {
            Vector2 direction = collision.GetContact(0).normal;
            Rigidbody2D otherBody = collision.gameObject.GetComponent<Rigidbody2D>();
            if(otherBody != null)
            otherBody.AddForce(-direction* knockback);
        }
    }
    void OnCollisionStay2D(Collision2D collision) {
        HealthSystem otherHealth = collision.gameObject.GetComponent<HealthSystem>();
        if(otherHealth != null) otherHealth.TryDamage(damage);
    }
}
