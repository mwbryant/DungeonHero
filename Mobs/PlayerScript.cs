using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

//Old janky player controller, now split into multiple seperate systems
[Obsolete("Use new dual stick movement and damage components")]
public class PlayerScript : MonoBehaviour
{
    public bool MattsWay =false;
    [Tooltip("Game will allow spin in both directions, holding both buttons moves forward")]
    public bool DualSpin = true;

    public float revSpeed;
    public float runSpeed;
    [Tooltip("Increases angular drag during forward movement")]
    public float DragIncreaseOnForward;

    public float Health;
    public float MaxHealth;
    public GameObject gibs;
    public Color Normal;
    public Color Damaged;
    public float DamageColorTime;
    public float InvincibilityInterval;
    public float MinAngularDrag;
    public float SpeedRampFactor;
    private float TimeFromLastHit;

    public float Gold;
    public AudioClip DamageClip;
    public AudioClip HealthPotClip;
    public UnityEvent Death;


    public SpriteRenderer BodySprite;

    //TODO it would be great to seperate UI from player
    //Maybe through an event system
    //Actually a lot of things need a callback on player death....
    public FadeInUI DeathScreenUI;

    private Rigidbody2D body;

    private AudioSource audioSource;
    private float start_angular_drag;
    
    void Start() {
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        start_angular_drag = body.angularDrag;
    }

    void OnValidate()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void DualSpinMovement() {
        if (Input.GetButton("Special2") && Input.GetButton("Special")) {
            //Increase drag if trying to move forward
            body.AddForce(-transform.up * runSpeed * Time.fixedDeltaTime, ForceMode2D.Force);
            body.angularDrag = start_angular_drag * DragIncreaseOnForward;
        }
        else if (Input.GetButton("Special"))
        {
            body.angularDrag *= SpeedRampFactor;
            if (body.angularDrag < MinAngularDrag)
            {
                body.angularDrag = MinAngularDrag;
            }
            var impulse = revSpeed * body.inertia;
            body.AddTorque(-impulse * Time.fixedDeltaTime, ForceMode2D.Force);
            if(body.angularDrag > start_angular_drag) body.angularDrag = start_angular_drag;
        } else if (Input.GetButton("Special2"))
        {
            body.angularDrag *= SpeedRampFactor;
            if (body.angularDrag < MinAngularDrag)
            {
                body.angularDrag = MinAngularDrag;
            }
            var impulse = revSpeed * body.inertia;
            body.AddTorque(impulse * Time.fixedDeltaTime, ForceMode2D.Force);
            if(body.angularDrag >start_angular_drag) body.angularDrag = start_angular_drag;
        } else {
            body.angularDrag = start_angular_drag;
        }
    }

    void SingleSpinMovement()
    {
            if (Input.GetButton("Move"))
            {
                //Increase drag if trying to move forward
                body.angularDrag = start_angular_drag * DragIncreaseOnForward;
                body.AddForce(-transform.up * runSpeed * Time.fixedDeltaTime);
            }
            if (Input.GetButton("Special"))
            {
                body.angularDrag = start_angular_drag;
                if(body.inertia == 0) Debug.Log("Matts way requires dynamic rigidbody!");
                var impulse = revSpeed * body.inertia;
                body.AddTorque(impulse * Time.fixedDeltaTime);
            }
    }

    void Movement()
    {
        if (DualSpin)
        {
            DualSpinMovement();
        } else if (MattsWay)
        {
            SingleSpinMovement();
        } else
        {
            //SAMS CODE
            if (Input.GetButton("Special"))
            {
                body.MoveRotation(body.rotation + revSpeed * Time.fixedDeltaTime);
            }
            if (Input.GetButton("Move"))
            {
                Vector3 delta = runSpeed * -transform.up * Time.fixedDeltaTime;
                body.MovePosition(body.position + new Vector2(delta.x, delta.y));
            }
        }
    }

    void FixedUpdate () {
        Movement();

        if (Health<=0) {
            //TODO fire a death event here
            GameObject new_gibs = Instantiate(gibs, transform.position, Quaternion.identity);
            Transform[] children =new_gibs.GetComponentsInChildren<Transform>();

            EnemyChase[] enemies = FindObjectsOfType<EnemyChase>();
            for (int i = 0; i < enemies.Length; i= i+1) {
                enemies[i].target = children[1];
            }

            Death.Invoke();

            Destroy(gameObject);
        }


        TimeFromLastHit = TimeFromLastHit + Time.deltaTime;    
        //flash red
        if (TimeFromLastHit >= DamageColorTime){
            BodySprite.color = Normal;
        }
    }   

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "enemy")
        {
            DamagePlayer();
        }
    }

    public void DamagePlayer()
    {
        if (TimeFromLastHit >= InvincibilityInterval)
        {
            Health--;
            audioSource.PlayOneShot(DamageClip);
            Debug.Log("ouch");
            TimeFromLastHit = 0;
            BodySprite.color = Damaged;
        }
    }
    public void PlayDamageSound()
    {
    }
    //Isaac's Code Please feel free to corect/Clean up!!!!
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("coins"))
        {
            Coin coin = other.gameObject.GetComponent<Coin>();
            coin.Pickup();
            Gold += coin.Value;
            other.enabled = false;
        }else if(other.gameObject.CompareTag("health")){
            if(Health < MaxHealth){
                Health++;
                audioSource.PlayOneShot(HealthPotClip);
            }
            Debug.Log("Picked up health");
            Destroy(other.gameObject);
        }
    }
}