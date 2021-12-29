using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private float health;
    public float maxHealth;
    [Space()]
    public Color normal;
    public Color damaged;
    public float damageColorTime;
    public float invincibilityInterval;
    public bool isInvincible;
    [Space()]
    public SpriteRenderer bodySprite;
    public GameObject gibs;
    public AudioClip damageClip;
    public ParticleSystem damageEffect;
    public UnityEvent deathEvent;



    private float timeFromLastHit;
    private AudioSource source;

    void Start() {
        source = GetComponent<AudioSource>();
    }

    //Returns if damage was dealt (ie not invincible)
    public bool TryDamage(int damage) {
        if(damage <= 0 ) return false;
        if (timeFromLastHit >= invincibilityInterval && !isInvincible) {
            health--;
            timeFromLastHit = 0;

            if(damageClip != null) source.PlayOneShot(damageClip);
            else Debug.Log("Missing damage sound on " + name);

            if(damageEffect != null) damageEffect.Play();
            else Debug.Log("Missing damage particle system on " + name);

            bodySprite.color = damaged;

            if (health <= 0) {
                deathEvent.Invoke();
            }
            return true;
        }
        return false;
    }

    void Update() {
        timeFromLastHit = timeFromLastHit + Time.deltaTime;    
        //flash red
        if (timeFromLastHit >= damageColorTime){
            bodySprite.color = normal;
        }
    }
}
