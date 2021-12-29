using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInUI : MonoBehaviour
{
    public Image image;
    public float fadetime;
    private float timer=0;
    public bool FadingIn = false;

    void Update()
    {
        if(!FadingIn) return;
        if(timer > fadetime) return;
        timer += Time.deltaTime;
        Color c = image.color;
        c.a = timer/fadetime;
        image.color = c;
    }
}
