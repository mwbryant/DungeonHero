using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyChase : MonoBehaviour
{
    public Transform target;
    //public float MoveCell;
    public float MoveTime;
    private float Timer;
    public float Health;
    public ParticleSystem DamageEffect;
    public LayerMask mask;

    private AudioSource damageClip;

    private Vector2 wanderoffset;
    private float wanderTimer;

    public Collider2D coll2D;
    public SpriteRenderer Sprite;
    public Color Normal;
    public Color Damaged;
    public float InvincibilityInterval;
    public float DamageColorTime;
    private float TimeFromLastHit;

    [Range(0.2f, 200f)]
    public float minRunSpeed;
    [Range(0.2f, 200f)]
    public float maxRunSpeed;
    private float runSpeed;

    private Rigidbody2D body;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        runSpeed = Random.Range(minRunSpeed, maxRunSpeed);
        damageClip = GetComponent<AudioSource>();
    }

    void OnValidate()
     {
        runSpeed = Random.Range(minRunSpeed, maxRunSpeed);
     }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            if(playerHealth == null) Debug.Log(name + " hit " + collision.gameObject.name + " which had tag Player but no health system...");
            playerHealth.TryDamage(1);
        }
    }

    //DezNuts
    void Update()
    {
        Timer=Timer+Time.deltaTime;
        TimeFromLastHit = TimeFromLastHit + Time.deltaTime;    

        if (Timer > MoveTime)
        {
            if(target != null){
                Vector3 direction = target.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), direction, Mathf.Infinity, mask.value);
                if (hit.collider.tag == "Player" || hit.collider.tag == "weapon")
                {
                    direction.Normalize();
                    body.AddForce(direction * runSpeed);
                } else
                {
                    Debug.Log("Cant see player");
                    wanderTimer -= Time.deltaTime;
                    if (wanderTimer <= 0)
                    {
                        wanderTimer += Random.Range(.01f, 0.2f);
                        wanderoffset = Random.insideUnitCircle * .5f;
                    }
                    direction = new Vector3(wanderoffset.x, wanderoffset.y, 0);
                    direction.Normalize();
                    body.AddForce(direction * runSpeed * .25f);
                }
            } else
            {
                Debug.Log("No target");
            }

            Timer=0;
        }

        //flash red
        if (TimeFromLastHit >= DamageColorTime){
            Sprite.color = Normal;
        }

        if (Health<=0) {
            //if i need to drop coins do it
            if (GetComponent<CoinSpewer>() != null)
            {
                Sprite.enabled = false;
                GetComponent<CoinSpewer>().Spew();
                coll2D.enabled = false;
                body.velocity = Vector2.zero;
                Destroy(this);
            } else
            {
                Destroy(gameObject);
            }
        }
        
    }
}
