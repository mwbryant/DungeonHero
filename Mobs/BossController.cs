using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dear god this is jank but the whole script was wrote and tested in the last hour of the jam
//Please be gentle
[RequireComponent(typeof(AudioSource))]
public class BossController : MonoBehaviour
{
    public Transform target;
    //public float MoveCell;
    public float MoveTime;
    private float Timer;
    public float Health;
    public ParticleSystem DamageEffect;
    public LayerMask mask;
    public WinScreen win;

    private AudioSource damageClip;

    private Vector2 wanderoffset;
    private float wanderTimer;

    public Collider2D coll2D;
    public SpriteRenderer Sprite;
    public Color Normal;
    public Color Damaged;
    public Color ChargeColor;
    public float InvincibilityInterval;
    public float DamageColorTime;
    private float TimeFromLastHit;
    public float IdleTime = 2;
    public float ChargeTime = 1;
    public float movementTimer;

    [Range(0.2f, 200f)]
    public float minRunSpeed;
    [Range(0.2f, 200f)]
    public float maxRunSpeed;
    private float runSpeed;

    private int bossState = 0;
    public float chargeMultiplier = 3;

    private Rigidbody2D body;
    public Animator animator;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        runSpeed = Random.Range(minRunSpeed, maxRunSpeed);
        damageClip = GetComponent<AudioSource>();
    }

    void OnValidate()
     {
        runSpeed = Random.Range(minRunSpeed, maxRunSpeed);
     }

    private bool startedReady = false;

    //DezNuts
    void Update()
    {
        if (Health<=0) {
            //if i need to drop coins do it
            animator.SetTrigger("Death");
            win.OnPlayerWin();
            return;
        }
        if((target.position - transform.position).magnitude > 20) return;
        Timer=Timer+Time.deltaTime;
        movementTimer+=Time.deltaTime;
        TimeFromLastHit = TimeFromLastHit + Time.deltaTime;

    // ready to charge animation
        if (bossState == 0 && movementTimer > (IdleTime) - 2 && !startedReady)
        {
            animator.SetTrigger("Start");
            startedReady = true;
        }

        if (bossState == 0 && movementTimer > IdleTime)
        {
            startedReady = false;
            bossState = 1;
            movementTimer = 0;
        }
        if (bossState == 1 && movementTimer > ChargeTime)
        {
            animator.SetTrigger("Idle");
            bossState = 0;
            movementTimer = 0;
        }

        if (Timer > MoveTime)
        {
            if(target != null){
                Vector3 direction = target.position - transform.position;
                if(direction.x < 0 ) Sprite.flipX = true;
                else Sprite.flipX = false;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), direction, Mathf.Infinity, mask.value);
                if (hit.collider.tag == "Player" || hit.collider.tag == "weapon")
                {
                    direction.Normalize();
                    if (bossState == 0)
                    {
                        if(!startedReady)
                        body.AddForce(direction * runSpeed);
                        else
                            body.velocity = Vector2.zero;
                    } else
                    {
                        body.AddForce(direction * runSpeed * chargeMultiplier);
                    }
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
        if (TimeFromLastHit >= DamageColorTime && bossState == 0){
            Sprite.color = Normal;
        }
        if (bossState == 1)
        {
            Sprite.color = ChargeColor;
        }

        
    }
}
