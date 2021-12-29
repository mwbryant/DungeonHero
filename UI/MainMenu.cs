using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour
{
    public string MainScene;
    public GameObject Main;
    public GameObject Credits;
    private AudioSource button;

    void Start()
    {
        button = GetComponent<AudioSource>();
    }

    public void StartButton()
    {
        Debug.Log("Button PresseD");
        button.Play();
        SceneManager.LoadScene(MainScene);
    }

    public void BackButton()
    {
        button.Play();
        Main.SetActive(true);
        Credits.SetActive(false);
    }

    public void CreditsButton()
    {
        button.Play();
        Main.SetActive(false);
        Credits.SetActive(true);
    }
}
