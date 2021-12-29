using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{

    public FadeInUI overlay;
    public Transform buttons;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerScript>().Death.AddListener(OnPlayerDeath);
    }

    void OnPlayerDeath()
    {
        overlay.FadingIn = true;
        buttons.gameObject.SetActive(true);
    }
}
