using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonTile : MonoBehaviour
{

    public int gridX = -1;
    public int gridY = -1;
    public Manager manager;
    private Color initialColor;
    public int Health { get; set; } // changes on one tile type
    public TileTypeEnum terrainType;
    private ParticleSystem validMovParticle;
    private ParticleSystem invalidMovParticle;
    public Material desertBad;

    public int consumedMovement()
    {
        switch(terrainType)
        {
            case TileTypeEnum.PLAINS:
                return 0;
            case TileTypeEnum.SWAMP:
                return 2;
            default:
                return 1;
        }
    }

    public void setParticles(ParticleSystem validMovParticle, ParticleSystem invalidMovParticle)
    {
        this.validMovParticle = validMovParticle;
        this.invalidMovParticle = invalidMovParticle;
    }

    // Use this for initialization
    void Start()
    {
        initialColor = gameObject.GetComponentInChildren<Renderer>().material.color;
        Health = terrainType == TileTypeEnum.DESERT ? 2 : 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeToValidColor()
    {
        float inc = terrainType == TileTypeEnum.MOUNTAIN ? 1.5f : 0.5f;
        //gameObject.GetComponentInChildren<Renderer>().material.color = new Color(1f, 0f, 1f);
        validMovParticle.transform.position = transform.position + new Vector3(0, inc, 0);
        validMovParticle.gameObject.SetActive(true);
    }

    public void changeToInvalidColor()
    {
        float inc = terrainType == TileTypeEnum.MOUNTAIN ? 1.5f : 0.5f;
        //gameObject.GetComponentInChildren<Renderer>().material.color = new Color(1f, 1f, 0f);
        invalidMovParticle.transform.position = transform.position + new Vector3(0, inc, 0);
        invalidMovParticle.gameObject.SetActive(true);
    }

    void OnMouseOver()
    {
        
        manager.tileOver(gridX, gridY);
    }

    void OnMouseExit()
    {
        //gameObject.GetComponentInChildren<Renderer>().material.color = initialColor;
        validMovParticle.gameObject.SetActive(false);
        invalidMovParticle.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        manager.tilePressed(gridX, gridY);
    }

    public void degrade()
    {
        Health--;
        if(Health <= 0)
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }
        if (terrainType == TileTypeEnum.DESERT)
        {
            //gameObject.GetComponent<Renderer>().materials = new Material[] { gameObject.GetComponent<Renderer>().materials[0], desertBad};
            gameObject.GetComponent<MeshFilter>().mesh = manager.desertPrefabBad.GetComponent<MeshFilter>().sharedMesh;
        }
    }

    public bool isDestroyed()
    {
        return Health <= 0;
    }

    public bool isWalkable()
    {
        return Health > 0 && terrainType != TileTypeEnum.MOUNTAIN;
    }

    public void DestroyTile()
    {
        Health = 0;
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void RecreateTile()
    {
        Health = 1;
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    public Vector3 getPlayerPosition()
    {
        if(terrainType == TileTypeEnum.MOUNTAIN)
        {
            return transform.position + new Vector3(0, 1f, 0);
        }
        return transform.position + new Vector3(0, 0.1f, 0); ;
    }

    public Vector3 getPowerupPosition(bool isTp)
    {
        if (terrainType == TileTypeEnum.MOUNTAIN)
        {
            return transform.position + new Vector3(0, 2f, 0);
        }
        return transform.position + new Vector3(0, 0.6f + (isTp ? 0.3f : 0f), 0); ;
    }
}
