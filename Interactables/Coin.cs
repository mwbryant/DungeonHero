using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Coin : MonoBehaviour
{
    public int Value;
    public float SucRange = 1;
    public float SucForce = 1;

    public float Lifetime = 3;
    public float BlinkTime = 1;
    public float BlinkRate = .1f;

    public ParticleSystem PickupParticles;
    private float life = 0;

    private Transform player_ref;
    //If I failed to find a player dont just keep retrying, probably dead
    private bool given_up = false;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private AudioSource audioClip;

    IEnumerator Shrink()
    {
        while (true)
        {
            body.velocity = Vector2.zero;
            transform.localScale *= .6f;
            yield return new WaitForSeconds(.1f);
        }
    }

    public void Pickup()
    {
        body.velocity = Vector2.zero;
        PickupParticles.Play();
        audioClip.Play();
        StartCoroutine(Shrink());

        Destroy(gameObject, PickupParticles.main.duration + PickupParticles.main.startLifetime.constantMax);
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        audioClip = GetComponent<AudioSource>();
    }

    void Update()
    {
        life += Time.deltaTime;
        if(life > Lifetime) Destroy(gameObject);
        if (Lifetime - life < BlinkTime)
        {
            float fade = (Mathf.Cos(Lifetime - (life+2.2f) * 2 * Mathf.PI * BlinkRate) + 1.2f) / 2.2f;
            Color color = sprite.color;
            color.a = fade;
            sprite.color = color;
        }
        if(given_up) return;
        MoveTowardPlayer();
    }

    void MoveTowardPlayer()
    {
        if (player_ref == null)
        {
            //try to find player
            GameObject player_obj = GameObject.FindGameObjectWithTag("Player");
            if (player_obj == null)
            {
                // :(
                given_up = true;
                return;
            }
            player_ref = player_obj.transform;
        }
        //Get dist from player then suc
        //Use vector2 because z
        //TODO if this is slow we can do dist squared, this is on coins which there may be many of
        //Profile
        float dist = Vector2.Distance(new Vector2(player_ref.position.x, player_ref.position.y), new Vector2(transform.position.x, transform.position.y));
        if (dist < SucRange)
        {
            Vector2 direction = new Vector2(player_ref.position.x, player_ref.position.y)- new Vector2(transform.position.x, transform.position.y);
            //Suc
            body.AddForce(direction.normalized * (1/direction.magnitude) * SucForce, ForceMode2D.Force);
        }
    }
}
