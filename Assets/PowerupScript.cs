using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupScript : MonoBehaviour {
    public PowerupEnum powerupType;
    public ParticleSystem powerupParticlePrefab;
    private ParticleSystem powerupParticle;
    public int GridX { get; set; }
    public int GridY { get; set; }
    private int turn = 0;

	// Use this for initialization
	void Start () {
        powerupParticle = Object.Instantiate<ParticleSystem>(powerupParticlePrefab);
        powerupParticle.transform.position = transform.position;
        powerupParticle.startColor = new Color(1, 0, 1);
        ParticleSystem.ColorOverLifetimeModule col = powerupParticle.colorOverLifetime;
        col.enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void nextTurn()
    {
        turn += 1;
        //deprecated, but awesome if it works instead of the weird current way
        powerupParticle.emissionRate += 30;
        powerupParticle.startSpeed += 2;
        if (turn == 3)
        {
            ParticleSystem.ColorOverLifetimeModule col = powerupParticle.colorOverLifetime;
            col.enabled = true;
            powerupParticle.startColor = new Color(1, 1, 1);
        }
        if (turn == 4)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            powerupParticle.enableEmission = false;
            
        }
    }

    public void gotten()
    {
        gameObject.SetActive(false);
        SoundEffects.sf.playPowerup();
    }

    public bool getable()
    {
        return turn >= 4;
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.RotateAround(Vector3.up, 2 * Time.deltaTime);
	}
}
