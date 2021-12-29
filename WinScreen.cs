using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public FadeInUI overlay;
    public Transform buttons;
    public Transform winText;

    public void OnPlayerWin()
    {
        overlay.FadingIn = true;
        buttons.gameObject.SetActive(true);
        winText.gameObject.SetActive(true);
    }
}
