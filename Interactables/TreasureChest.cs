using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CoinSpewer))]
public class TreasureChest : MonoBehaviour
{
    public Sprite Opened;
    public Sprite Damage;
    public int Value;
    public GameObject HealthPot;

    public float GoldOdds;
    public float HealthOdds;
    public float TrapOdds;

    // Possibility for weapon chest down the road
    public enum ChestType {
        Gold = 0,
        Health,
        Trap,
    }

    private SpriteRenderer sprite_renderer;
    private bool opened;
    private ChestType Type;

    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        opened = false;
        
        float total = HealthOdds + GoldOdds + TrapOdds;

        float health_chance = HealthOdds / total;
        float gold_chance = GoldOdds / total + health_chance;
        float trap_chance = TrapOdds / total + gold_chance;

        //assert trap chance == 1
        if (trap_chance < .999 || trap_chance > 1.001)
        {
            Debug.LogError("Matt doesn't know math :(");
        }

        float chance = Random.value;

        if(chance < health_chance){
            Type = ChestType.Health;
            Debug.Log("Health");
        } else if(chance < gold_chance){
            Type = ChestType.Gold;
            Debug.Log("Gold");
        } else {
            Type = ChestType.Trap;
            Debug.Log("It's a trap!");
        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(opened) return;
        if (collider.gameObject.tag == "Player")
        {
            //Open the chest
            //TODO add particles and cool stuff
            sprite_renderer.sprite = Opened;
            opened = true;

            switch(Type){
                case ChestType.Gold:
                    Debug.Log("Opening Gold");
                    GetComponent<CoinSpewer>().Spew();
                    // Instanciate ${Value} gold
                    break;
                case ChestType.Health:
                    Debug.Log("Opening Health");
                    Instantiate(HealthPot, GetComponent<Transform>());
                    // Instanciate health pot
                    break;
                case ChestType.Trap:
                    Debug.Log("Opening Trap");
                    // Damage Player
                    sprite_renderer.sprite = Damage;
                    PlayerScript player = collider.gameObject.GetComponent<PlayerScript>();
                    if(player==null) return;
                    player.DamagePlayer();
                    break;
            }
        }
    }

}
