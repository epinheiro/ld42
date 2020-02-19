using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

    public HexagonTile forestPrefab;
    public HexagonTile plainsPrefab;
    public HexagonTile mountainPrefab;
    public HexagonTile swampPrefab;
    public HexagonTile desertPrefabGood;
    public HexagonTile desertPrefabBad;
    Dictionary<TileTypeEnum, HexagonTile> dictTileToPrefab;
    Dictionary<PowerupEnum, PowerupScript> dictPowerupToPrefab;
    public PowerupScript destroyPowerup, createPowerup, teleportPowerup;
    private List<List<HexagonTile>> hexGrid;
    public Player playerPrefab;
    List<Player> players;
    private int currentTurn = 0;
    public int limitX = 10;
    public int limitY = 11;
    public ParticleSystem validMovParticlePrefab;
    public ParticleSystem invalidMovParticlePrefab;
    private ParticleSystem validMovParticle;
    private ParticleSystem invalidMovParticle;
    private int movementLeft;
    public CameraControl camControl;
    private System.Array enumValuesPowerups;
    int turnCount = 0;
    private List<PowerupScript> powerups;
    private PowerupScript activePowerup;
    public static bool aiActive = true;

    public AudioSource audioSource;
    public AudioClip gameSong;
    public AudioClip victorySong;

    /// UI
    public GameObject InGameUI;
    public GameObject Player1TurnUI;
    public GameObject Player2TurnUI;
    public GameObject PlayerActionUI;
    public GameObject PlayerActionIconUI;
    public Texture[] actions;
    public Texture[] icons;
    public GameObject winnerScreen;
    public GameObject winnerPlayer1;
    public GameObject winnerPlayer2;


    // Use this for initialization
    void Start () {
        audioSource.clip = gameSong;
        audioSource.Play();
        audioSource.loop = true;

        activePowerup = null;
        powerups = new List<PowerupScript>();
        playerPrefab.manager = this;
        movementLeft = 2;
        dictTileToPrefab = new Dictionary<TileTypeEnum, HexagonTile>()
        {
            {TileTypeEnum.FOREST, forestPrefab}, {TileTypeEnum.PLAINS, plainsPrefab},
            {TileTypeEnum.MOUNTAIN, mountainPrefab}, {TileTypeEnum.SWAMP, swampPrefab},  {TileTypeEnum.DESERT, desertPrefabGood}
        };

        dictPowerupToPrefab = new Dictionary<PowerupEnum, PowerupScript>()
        {
            {PowerupEnum.CREATE, createPowerup }, {PowerupEnum.DESTROY, destroyPowerup }, {PowerupEnum.TELEPORT, teleportPowerup }
        };

        enumValuesPowerups = System.Enum.GetValues(typeof(PowerupEnum));

        System.Array enumValues = System.Enum.GetValues(typeof(TileTypeEnum));
        hexGrid = new List<List<HexagonTile>>();
        float sizeX = Mathf.Sqrt(3f);
        float sizeY = 2f;

        validMovParticle = Object.Instantiate<ParticleSystem>(validMovParticlePrefab);
        invalidMovParticle = Object.Instantiate<ParticleSystem>(invalidMovParticlePrefab);
        validMovParticle.gameObject.SetActive(false);
        invalidMovParticle.gameObject.SetActive(false);

        for (int j = 0; j < limitY; j++)
        {
            List<HexagonTile> currRow = new List<HexagonTile>();
            for (int i = 0; i < limitX; i++)
            {
                TileTypeEnum tileType;
                if (j <= limitY / 2 )
                {
                    tileType = (TileTypeEnum) enumValues.GetValue(Random.Range(0, enumValues.Length));
                } else
                {
                    tileType = hexGrid[limitY / 2 - (j - limitY / 2)][i].terrainType;
                }
                HexagonTile newTile = Object.Instantiate<HexagonTile>(dictTileToPrefab[tileType]);
                newTile.manager = this;
                Vector3 newPos = new Vector3(sizeX * i + ((1f / 2) * sizeX) * (j % 2),
                    0, (3f / 4) * sizeY * j);
                newTile.transform.position = newPos;
                newTile.name = "hexagon " + i + " " + j;
                currRow.Add(newTile);
                newTile.gridX = i;
                newTile.gridY = j;
                newTile.setParticles(validMovParticle, invalidMovParticle);
            }
            hexGrid.Add(currRow);
        }

        initializePlayers();
        Camera.main.transform.position = new Vector3(hexGrid[limitY / 2][limitX / 2].transform.position.x,
            Camera.main.transform.position.y,
            hexGrid[limitY / 2][limitX / 2].transform.position.z);
        camControl.changeToCharacterPosition(players[currentTurn].transform.position, players[currentTurn].GridY <= (limitY / 2));
    }

    public List<List<HexagonTile>> getHexGrid()
    {
        return hexGrid;
    }

    public List<Player> getPlayers()
    {
        return players;
    }

    public int getCurrentTurn()
    {
        return currentTurn;
    }

    public PowerupScript getActivePowerup()
    {
        return activePowerup;
    }

    private void initializePlayers()
    {
        players = new List<Player>();
        players.Add(GameObject.Instantiate<Player>(playerPrefab));
        players.Add(GameObject.Instantiate<Player>(playerPrefab));
        players[0].GetComponentInChildren<Renderer>().material.color = new Color(1, 0, 0) ;
        players[1].GetComponentInChildren<Renderer>().material.color = new Color(0, 1, 1); ;
        players[0].setPlayerNumber(0);
        players[1].setPlayerNumber(1);
        int ypos = limitY / 4;
        int xpos = limitX / 2;
        players[0].changeToPosition(xpos, ypos, hexGrid[ypos][xpos].getPlayerPosition());
        players[1].changeToPosition(xpos, limitY - 1 - ypos, hexGrid[limitY - 1 - ypos][xpos].getPlayerPosition());
    }

    private bool checkValidMovement(int gridX, int gridY)
    {
        Player currPlayer = players[currentTurn];
        //If the player is currently at an %2 == 0 row, then he can move to the below and above rows at either
        //gridX or gridX - 1. If it's a %2 == 1, then either gridX or gridX + 1. The modifier is the -1/+1.
        int modModifier = (currPlayer.GridY % 2) == 0 ? -1 : 1;

        //In theory he can't even select those
        /*if (hexGrid[gridX][gridY].Health <= 0)
        {
            return false;
        }*/

        if (hexGrid[gridY][gridX].terrainType == TileTypeEnum.MOUNTAIN 
            || hexGrid[gridY][gridX].isDestroyed()
            || isPlayerThisPos((currentTurn + 1) % 2, gridX, gridY)) return false;

        if ((gridY == currPlayer.GridY && (Mathf.Abs(gridX - currPlayer.GridX) == 1)) //same Y
            || ((gridY == (currPlayer.GridY + 1) || gridY == (currPlayer.GridY - 1)) //Different Y
            && (gridX == currPlayer.GridX || (gridX == (currPlayer.GridX + modModifier)))))
        {            
            return true;
        }
        return false;
    }

    public bool isPowerupHere(int gridX, int gridY)
    {
        foreach (PowerupScript p in powerups)
        {
            if (p.GridX == gridX && p.GridY == gridY)
            {
                return true;
            }
        }
        return false;
    }

    void nextTurn()
    {
        foreach(PowerupScript p in powerups) {
            p.nextTurn();
        }
        turnCount++;
        movementLeft = 2;
        currentTurn = (currentTurn + 1) % 2;
        camControl.changeToCharacterPosition(players[currentTurn].transform.position, players[currentTurn].GridY <= (limitY / 2));
        if (turnCount >= 1 && Random.Range(0, 10) < 3)
        {
            PowerupEnum powerup = (PowerupEnum)enumValuesPowerups.GetValue(Random.Range(0, enumValuesPowerups.Length));
            //PowerupEnum powerup = PowerupEnum.TELEPORT;
            int rx = Random.Range(0, limitX);
            int ry = Random.Range(0, limitY);
            int tries = 0; //Just to be safe yknow

            while ((!hexGrid[ry][rx].isWalkable() || isPowerupHere(rx, ry)) && tries < 20)
            {
                tries++;
                rx = Random.Range(0, limitX);
                ry = Random.Range(0, limitY);
            }

            if (tries < 20)
            {
                PowerupScript currPowerup = Object.Instantiate<PowerupScript>(dictPowerupToPrefab[powerup]);
                currPowerup.transform.position = hexGrid[ry][rx].getPowerupPosition(currPowerup.powerupType == PowerupEnum.TELEPORT);
                currPowerup.GridX = rx;
                currPowerup.GridY = ry;
                powerups.Add(currPowerup);
            }            
        }
        checkGetPowerup();
    }

    /*
     * How is the movement/click on tile going to work?
     * What I thought is that each tile send a message to the manager when a mouseover/click occurs
     * that then decides what is going to happen (since the manager has the information available to do that)
     * and then may send a message back to the tile saying "Change your color" or anything else, but the manager
     * is the one with the logic.
     */
    public void tilePressed(int gridX, int gridY, bool doit = false)
    {
        if (aiActive && currentTurn == 1 && !doit) return;

        Debug.Log("Pressed " + gridX + " " + gridY);
        if (activePowerup != null)
        {
            if (!isPowerupClickable(gridX, gridY))
            {
                SoundEffects.sf.playWrong();
                return;
            }
            switch(activePowerup.powerupType)
            {
                case PowerupEnum.CREATE:
                    hexGrid[gridY][gridX].RecreateTile();
                    break;
                case PowerupEnum.DESTROY:
                    hexGrid[gridY][gridX].DestroyTile();
                    break;
                case PowerupEnum.TELEPORT:
                    players[currentTurn].changeToPosition(gridX, gridY, hexGrid[gridY][gridX].getPlayerPosition());
                    checkGetPowerup();
                    break;
            }
            activePowerup = null;
            SoundEffects.sf.playPiece();
        }
        else if (checkValidMovement(gridX, gridY))
        {
            movementLeft -= hexGrid[gridY][gridX].consumedMovement();
            hexGrid[players[currentTurn].GridY][players[currentTurn].GridX].degrade();
            players[currentTurn].changeToPosition(gridX, gridY, hexGrid[gridY][gridX].getPlayerPosition());

            checkGetPowerup();
            SoundEffects.sf.playPiece();


        } else
        {
            SoundEffects.sf.playWrong();
        }

        if (movementLeft <= 0 && activePowerup == null)
        {
            nextTurn();
        }

        if (isGameWon())
        {
            showVictoryScreenUI((((currentTurn + 1) % 2) + 1));
        }

        

    }

    public bool isPowerupClickable(int gridX, int gridY)
    {
        if (activePowerup != null)
        {
            if (activePowerup.powerupType != PowerupEnum.CREATE && !hexGrid[gridY][gridX].isDestroyed())
            {
                return true;
            }
            else if (activePowerup.powerupType == PowerupEnum.CREATE)
            {
                if (hexGrid[gridY][gridX].isDestroyed())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    public void tileOver(int gridX, int gridY)
    {
        if (aiActive && currentTurn == 1) return;
        //checking against null for this is terrible and I should feel ashamed
        //Surely there is also a more elegant way to organize this
        if (activePowerup != null)
        {
            if (isPowerupClickable(gridX, gridY))
            {
                hexGrid[gridY][gridX].changeToValidColor();
            } else {
                hexGrid[gridY][gridX].changeToInvalidColor();
            }
        }
        else if (checkValidMovement(gridX, gridY))
        {
            hexGrid[gridY][gridX].changeToValidColor();
        }
        else
        {
            if (!hexGrid[gridY][gridX].isDestroyed())
            {
                hexGrid[gridY][gridX].changeToInvalidColor();
            }
        }
    }

    private void checkGetPowerup()
    {
        foreach (PowerupScript p in powerups)
        {
            if (p.GridX == players[currentTurn].GridX && p.GridY == players[currentTurn].GridY && p.getable())
            {
                activePowerup = p;
                break;
            }
        }
        if(activePowerup != null)
        {
            powerups.Remove(activePowerup);
            activePowerup.gotten();
        }
    }

    private bool isPlayerThisPos(int player, int gridX, int gridY)
    {
        return players[player].GridX == gridX && players[player].GridY == gridY;
    }

    public List<IntVector2> possibleMoves(int rx, int ry, bool notincplayer = false)
    {
        int otherPlayer = (currentTurn + 1) % 2;
        int gridXModModifier = rx + ((ry % 2) == 0 ? -1 : 1);
        List<IntVector2> moves = new List<IntVector2>();
        //Debug.Log("rx ry " + rx + " " + ry);
        for (int y = ry - 1; y <= ry + 1; y++)
        {
           // Debug.Log("y value " + y);
            if (y < 0 || y >= limitY) continue;

            if (Mathf.Abs(y - ry) == 1) //Case one above or below
            {
                if (hexGrid[y][rx].isWalkable() && (notincplayer || !isPlayerThisPos(otherPlayer, rx, y)))  //Case same X
                {
                   // Debug.Log("first added " + rx + " " + y);
                    moves.Add(new IntVector2(rx, y));
                }

                if (gridXModModifier >= 0 && gridXModModifier < limitX
                    && hexGrid[y][gridXModModifier].isWalkable() 
                    && (notincplayer || !isPlayerThisPos(otherPlayer, gridXModModifier, y))) //Diff x
                {
                   // Debug.Log("second added " + gridXModModifier + " " + y);
                    moves.Add(new IntVector2(gridXModModifier, y));
                }
            }
            else
            {
                //case same y
                if ((rx - 1 >= 0 && hexGrid[y][rx - 1].isWalkable()
                  && (notincplayer || !isPlayerThisPos(otherPlayer, rx - 1, y))))
                {
                  //  Debug.Log("third added " + (rx - 1) + " " + y);
                    moves.Add(new IntVector2(rx - 1, y));
                }

                if ((rx + 1 < limitX && hexGrid[y][rx + 1].isWalkable()
                  && (notincplayer || !isPlayerThisPos(otherPlayer, rx + 1, y))))
                {
                  //  Debug.Log("fourth added " + (rx + 1) + " " + y);
                    moves.Add(new IntVector2(rx + 1, y));
                }
            }
        }

        return moves;
    }

    public bool isGameWon()
    {
       /* Debug.Log("possible moves: ");
        foreach(IntVector2 m in possibleMoves(players[currentTurn].GridX, players[currentTurn].GridY))
        {
            Debug.Log(m.x + " - " + m.y);
        }*/
        return possibleMoves(players[currentTurn].GridX, players[currentTurn].GridY).Count == 0;
    }
    
	// Update is called once per frame
	void Update () {
        updatePlayerTurnUI();
        updatePlayerActionUI();
    }

    ////// UI functions
    // activePowerup
    // currentTurn -> jogador
    void updatePlayerTurnUI() {
        if (currentTurn == 0) {
            Player1TurnUI.SetActive(true);
            Player2TurnUI.SetActive(false);
        }
        else {
            Player1TurnUI.SetActive(false);
            Player2TurnUI.SetActive(true);
        }
    }
    // movementLeft
    void updatePlayerActionUI() {
        //Movement
        if (activePowerup == null) {
            PlayerActionUI.GetComponent<RawImage>().texture = actions[0];

            PlayerActionIconUI.GetComponent<RawImage>().texture = icons[0];
            PlayerActionIconUI.GetComponentInChildren<Text>().text = movementLeft.ToString();

        } // Powerup
        else {
            PlayerActionUI.GetComponent<RawImage>().texture = actions[1];
            PlayerActionIconUI.GetComponentInChildren<Text>().text = "";

            switch (activePowerup.powerupType) {
                case PowerupEnum.CREATE:
                    PlayerActionIconUI.GetComponent<RawImage>().texture = icons[1];
                    break;
                case PowerupEnum.DESTROY:
                    PlayerActionIconUI.GetComponent<RawImage>().texture = icons[2];
                    break;
                case PowerupEnum.TELEPORT:
                    PlayerActionIconUI.GetComponent<RawImage>().texture = icons[3];
                    break;
            }

        }
    }
    void showVictoryScreenUI(int winner) {
        audioSource.clip = victorySong;
        audioSource.Play();
        audioSource.loop = false;
        Debug.Log("Player " + winner + " won!");

        Player1TurnUI.SetActive(false);
        Player2TurnUI.SetActive(false);
        PlayerActionUI.SetActive(false);
        PlayerActionIconUI.SetActive(false);
        winnerScreen.SetActive(true);

        if (winner == 1) {
            winnerPlayer1.SetActive(true);
        } else {
            winnerPlayer2.SetActive(true);
        }
    }

    public void GoToMainMenu() {
        SoundEffects.sf.playButton();
        SceneManager.LoadScene("MainMenu");
    }
}
