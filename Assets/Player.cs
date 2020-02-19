using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private int gridX = -1;
    private int gridY = -1;
    public int GridX { get { return gridX; } }
    public int GridY { get { return gridY; } }
    public Manager manager;
    private int playerNumber = -1; //0 or 1

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

    //I don't like using "setPosition" since it does a little more
    public void changeToPosition(int gridX, int gridY, Vector3 realPosition)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        transform.position = realPosition;
    }
}
