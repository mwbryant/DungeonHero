using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image[] Hearts;
    public Sprite FilledHeart;
    public Sprite EmptyHeart;
    private PlayerScript player;

    void Start()
    {
       player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    void Update()
    {
        //TODO Player health UI should be done on an event callback
        //But this is easier and faster and I doubt UI calls will be
        //The preformance road block
        if (player != null)
        {
            for (int i = 0; i < Hearts.Length; i++)
            {
                if (player.Health > i) Hearts[i].sprite = FilledHeart;
                else Hearts[i].sprite = EmptyHeart;
            }
        }
    }
}
