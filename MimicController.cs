using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyChase))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Rigidbody2D))]
public class MimicController : MonoBehaviour {

    public float wakeUpStunTime;

    private Animator animator;
    private EnemyChase enemyChase;
    private HealthSystem health;
    private float lifeTime;

    void Start() {
        animator = GetComponent<Animator>();
        animator.Play("Barrel Mimic Wake");

        enemyChase = GetComponent<EnemyChase>();
        enemyChase.enabled = false;

        health = GetComponent<HealthSystem>();
        health.isInvincible = true;
    }

    void Update() {
        lifeTime += Time.deltaTime;
        if (lifeTime > wakeUpStunTime) {
            enemyChase.enabled = true;
            health.isInvincible = false;
        }
    }
}
