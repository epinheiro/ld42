using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{

    public AudioClip piece;
    public AudioClip powerup;
    public AudioClip button;
    public AudioClip wrongClick;
    private AudioSource audioSource;
    public static SoundEffects sf;

    private void Awake()
    {
        sf = this;
    }

    // Use this for initialization
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void playPiece()
    {
        audioSource.PlayOneShot(piece);
    }
    public void playPowerup()
    {
        audioSource.PlayOneShot(powerup);
    }
    public void playButton()
    {
        audioSource.PlayOneShot(button);
    }

    public void playWrong()
    {
        audioSource.PlayOneShot(wrongClick);
    }
    

    // Update is called once per frame
    void Update()
    {

    }
}