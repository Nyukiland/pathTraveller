using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Difficulty control")]
    [Space(2)]
    [Tooltip("x is the number of tile on each side (better if odd) and y is the number of tile towards you (do not go over 15)")] [SerializeField] Vector2 matrixSize; //the scene is not set up to go beyond a size but the code work with high quantity
    [Space]
    [Tooltip("0 is everytime and 10 is 10% (mountains appear every 3 tile if possible)")] [Range(0, 10)] [SerializeField] int probabilityOfMountain;

    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("Prefab list")]
    [Space(2)]

    [Tooltip("element 0 should be mountains, others are juste empty grass")] [SerializeField] GameObject[] prefabBegin;

    [Space]

    [Tooltip("order should be the same as the set quantity")] [SerializeField] GameObject[] prefabPlayable;

    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("Tile deck quantity")]
    [Space(2)]
    [Tooltip("order should be the same as the prefab list")] [SerializeField] int[] playableQuantity;
    [Tooltip("number of tile in the hand of player")] [Range(1, 6)] [SerializeField] int inHandSize;

    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("GUI")]
    [Space(2)]
    [Tooltip("shows current tile in the hand of the player")] [SerializeField] GameObject sideHand;
    [Tooltip("visuale split between playground and hand")] [SerializeField] GameObject limitZone;
    [Tooltip("a gameObject that will be used as feedback for the placement")] [SerializeField] GameObject placementTile;
    [Tooltip("particle effect that will be spawned when a tile is created")] [SerializeField] GameObject particleFeedbackF;
    [Tooltip("particle effect that will be spawned when a tile is created")] [SerializeField] GameObject particleFeedbackD;
    [Tooltip("particle effect that will be spawned when a tile is created")] [SerializeField] GameObject particleFeedbackC;


    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("Script variable field")]
    [Space(2)]
    [Tooltip("need the selection manager script")] [SerializeField] SelectionManager selectionManager;

    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("In hand quantity and deck itself (feedback variable DO NOT TOUCH)")]
    [Space(2)]
    [Tooltip("shows current tile in the deck of the player")] [SerializeField] List<GameObject> deck;
    [Tooltip("shows current tile in the hand of the player")] public List<GameObject> inHand;
    [Tooltip("list of the placement feedbackTile")] [SerializeField] List<GameObject> placementTileList;
    [Tooltip("list of the position feedbackTile")] [SerializeField] List<Vector2> placementTileListM;

    /*
    Those matrix (multidimensionnal array) are filled with int each representing a value for the system to represent the capacity of a player
    4 digits are used to give the right amount of information.

    First digit is the state of tile. 0 is not placable, 1 is placable, 2 is never placable

    Second digit is the level of the tile. 0 is empty, 1 is level 1(forest), 2 is level 2(dirt), 3 is level 3(city)

    Third digit is the type of road. 0 is empty, 1 is straight, 2 is a turn, 3 is a triple road

    Fourth digit is the rotation of the tile. 0 is 0 degree, 1 is 90 degree, 2 is 180 degree, 3 is 270 degree

    */
    string[,] matrixGame; //most important multidimensionnal array


    //set of none nessecerry visible variable, mostly set up or feature
    GameObject[,] matrixOBJ;
    int[] rotationTilePossible = new int[] { 0, 90, 180, 270 };
    bool firstTile = true;


    // Start is called before the first frame update
    void Start()
    {
        //automatically set the camera
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -matrixSize.y - 5);
        if (matrixSize.y > matrixSize.x) Camera.main.orthographicSize = Mathf.Clamp(matrixSize.y, 8, 10);
        else Mathf.Clamp(matrixSize.x, 8, 10);

        //set up the scene and game
        SetUp();
        CreateDeck();

        //visual feedback for the game
        limitZone.transform.position = new Vector3(matrixOBJ[(int)matrixSize.x - 1, 0].transform.position.x + 5, sideHand.transform.position.y, matrixOBJ[0, (int)matrixSize.y / 2].transform.position.z + 2);
        sideHand.transform.position = new Vector3(matrixOBJ[(int)matrixSize.x - 1, 0].transform.position.x + 15, sideHand.transform.position.y, matrixOBJ[0, (int)matrixSize.y / 2].transform.position.z + 2);

        //create the hand
        InHandSetUp();
    }

    void SetUp()
    {
        //create proper sized multidimensionnal array
        matrixGame = new string[(int)matrixSize.x, (int)matrixSize.y];
        matrixOBJ = new GameObject[(int)matrixSize.x, (int)matrixSize.y];

        //generate the grass (/empty) tile and fill the multidimensionnal array
        for (int i = 0; i < matrixSize.x; i++)
        {
            for (int j = 0; j < matrixSize.y; j++)
            {
                //creation of the tile
                GameObject clone = Instantiate(prefabBegin[Random.Range(1, prefabBegin.Length)], new Vector3((i - ((matrixSize.x / 2) - 0.5f)) * 2, 0, -j * 2), Quaternion.identity);
                clone.transform.eulerAngles = new Vector3(0, rotationTilePossible[Random.Range(0, rotationTilePossible.Length)], 0);

                //multidiemnsionnal array control
                matrixOBJ[i, j] = clone;
                matrixGame[i, j] = "0000";

            }
        }

        //generate the mountains (/never placeable tile) and correct the multidimensionnal array (start at 1 to avoid softlock)
        for (int k = 1; k < matrixSize.y; k += 3)
        {
            //random the spawn possibility
            int shouldIHaveAMountain = Random.Range(0, probabilityOfMountain);

            //make sure it doesn't instantiate on the last raw so it doesn't block the exit
            if (k != matrixSize.y - 1 && shouldIHaveAMountain == 0)
            {
                //creation of the tile
                int randomInt = Random.Range(0, (int)matrixSize.x);
                GameObject clone = Instantiate(prefabBegin[0], matrixOBJ[randomInt, k].transform.position, Quaternion.identity);
                clone.transform.eulerAngles = new Vector3(0, rotationTilePossible[Random.Range(0, rotationTilePossible.Length)], 0);

                //multidiemnsionnal array control
                Destroy(matrixOBJ[randomInt, k]);
                matrixOBJ[randomInt, k] = clone;
                matrixGame[randomInt, k] = string.Concat("2", matrixGame[randomInt, k][1], matrixGame[randomInt, k][2], matrixGame[randomInt, k][3]);
            }
        }

        //determine the first tile as playable
        matrixGame[(int)matrixSize.x / 2, 0] = string.Concat("1", matrixGame[(int)matrixSize.x / 2, 0][1], matrixGame[(int)matrixSize.x / 2, 0][2], matrixGame[(int)matrixSize.x / 2, 0][3]);
    }

    void CreateDeck()
    {
        //create a deck of tile based on the quantity wanted
        for (int i = 0; i < playableQuantity.Length; i++)
        {
            for (int j = 0; j < playableQuantity[j] + 1; j++)
            {
                deck.Add(prefabPlayable[i]);
            }
        }
    }

    void InHandSetUp()
    {

        for (int i = 0; i < inHandSize; i++)
        {
            //create the tile
            int rng = Random.Range(0, deck.Count);
            GameObject clone = Instantiate(deck[rng], transform.position, Quaternion.identity);

            //change the transform 
            clone.transform.position = new Vector3(sideHand.transform.position.x, sideHand.transform.position.y, sideHand.transform.position.z + ((i * 4) + (2 * (1 - inHandSize))));

            //remove it from the deck and add it to the array
            deck.RemoveAt(rng);
            inHand.Add(clone);

            //add tag to it
            clone.tag = "CanSelect";

        }
    }

    public void AddTileToHand(Vector3 posToCreate)
    {
        //create the tile
        int rng = Random.Range(0, deck.Count);
        GameObject clone = Instantiate(deck[rng], transform.position, Quaternion.identity);

        //change the transform 
        clone.transform.position = posToCreate;

        //remove it from the deck and add it to the array
        deck.RemoveAt(rng);
        inHand.Add(clone);

        //add tag to it
        clone.tag = "CanSelect";
    }

    //when called place all the placement feedback to the possible position
    public void FeedbackVisuPlacement(string ID)
    {
        if (selectionManager.selectedTile != null)
        {

            for (int i = 0; i < matrixSize.x; i++)
            {
                for (int j = 0; j < matrixSize.y; j++)
                {
                    if (VerifyTileForFeedback(ID, new Vector2(i, j)))
                    {
                        placementTileListM.Add(new Vector2(i, j));
                        placementTileList.Add(Instantiate(placementTile, matrixOBJ[i, j].transform.position, Quaternion.identity));
                    }
                }
            }
        }
    }
    
    //remove all the placement feedback
    public void ClearFeedBackPlacement()
    {
        if (placementTileListM.Count == 0) return;

        for (int i = 0; i < placementTileListM.Count; i++)
        {
            GameObject temp = placementTileList[0];
            placementTileList.RemoveAt(0);
            Destroy(temp);
        }
        placementTileListM.Clear();
    }

    //called when the selected tile need/can be placed
    public void PlaceTile(GameObject pos)
    {
        firstTile = false;
        Vector2 storing = placementTileListM[placementTileList.IndexOf(pos)]; ;
        selectionManager.placingTile.transform.position = pos.transform.position;
        Particlespawn(pos.transform.position, selectionManager.placingTile.GetComponent<identifier>().identification);
        ClearFeedBackPlacement();
        GestionTileAfterPlacement(storing);
        UseIdentifierToActualizeTiles(selectionManager.placingTile.GetComponent<identifier>().identification, storing);
        selectionManager.placingTile = null;
    }

    //called to spawn the proper particle effects
    void Particlespawn(Vector3 pos, string ID)
    {
        pos = pos + Vector3.up*3;

        if (ID[1] == 1.ToString()[0]) Instantiate(particleFeedbackF, pos, Quaternion.identity);
        if (ID[1] == 2.ToString()[0]) Instantiate(particleFeedbackD, pos, Quaternion.identity);
        if (ID[1] == 3.ToString()[0]) Instantiate(particleFeedbackC, pos, Quaternion.identity);
    }

    //removes the previous tile on the pos and set the new tile in the matrix
    void GestionTileAfterPlacement(Vector2 matrixPos)
    {
        Destroy(matrixOBJ[(int)matrixPos.x, (int)matrixPos.y]);
        matrixOBJ[(int)matrixPos.x, (int)matrixPos.y] = selectionManager.placingTile;
    }

    //gives the id of the tile to the position and set the tile around as playable based on the rotation of the tile
    void UseIdentifierToActualizeTiles(string ID, Vector2 pos)
    {
        //actualize the tile
        matrixGame[(int)pos.x, (int)pos.y] = ID;

        //actualize the tile around
        // straight road (ID[2] == 1) on rotation 0 (ID[3] == 0) has an exit at x-1 and x+1
        // turn road (ID[2] == 2) on rotation 0 (ID[3] == 0) has an exit at x-1 and y-1
        // triple road (ID[2] == 3) on rotation 0 (ID[3] == 0) has an exit at x-1, x+1 and y-1 
        if (ID.ToString()[2] == 1.ToString()[0])
        {
            if (ID.ToString()[3] == 0.ToString()[0] || ID.ToString()[3] == 2.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x - 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x + 1, (int)pos.y));
            }
            else if (ID.ToString()[3] == 1.ToString()[0] || ID.ToString()[3] == 3.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x, (int)pos.y - 1));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y + 1));
            }
        }
        else if (ID.ToString()[2] == 2.ToString()[0])
        {
            if (ID.ToString()[3] == 0.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x - 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y - 1));
            }
            else if (ID[3] == 1.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x + 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y - 1));
            }
            else if (ID[3] == 2.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x, (int)pos.y + 1));
                ActivateTile(new Vector2((int)pos.x + 1, (int)pos.y));
            }
            if (ID[3] == 3.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x - 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y + 1));
            }
        }
        if (ID.ToString()[2] == 3.ToString()[0])
        {
            if (ID[3] == 0.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x - 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x + 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y - 1));
            }
            else if (ID[3] == 1.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x, (int)pos.y - 1));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y + 1));
                ActivateTile(new Vector2((int)pos.x + 1, (int)pos.y));
            }
            else if (ID[3] == 2.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x - 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x + 1, (int)pos.y));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y + 1));
            }
            if (ID[3] == 3.ToString()[0])
            {
                ActivateTile(new Vector2((int)pos.x, (int)pos.y - 1));
                ActivateTile(new Vector2((int)pos.x, (int)pos.y + 1));
                ActivateTile(new Vector2((int)pos.x - 1, (int)pos.y));
            }
        }
    }

    //called to set the tile as playable (also check if the position given correspond to a tile)
    void ActivateTile(Vector2 posToActivate)
    {
        if (posToActivate.x < 0 || posToActivate.x > matrixSize.x - 1) return;
        if (posToActivate.y < 0 || posToActivate.y > matrixSize.y - 1) return;
        if (matrixGame[(int)posToActivate.x, (int)posToActivate.y][0] != 0.ToString()[0]) return;

        matrixGame[(int)posToActivate.x, (int)posToActivate.y] = "1000";
    }

    //when called check if the given tile is placable on the given postion (verify it's rotation to check if the path is connected)
    bool VerifyTileForFeedback(string ID, Vector2 posToCheck)
    {
        //verify if the tile is said as placable
        if (matrixGame[(int)posToCheck.x, (int)posToCheck.y][0] != 1.ToString()[0]) return false;
        else if (firstTile) return true;

        //verify if the tile can go over another tile
        if (ID[1] <= matrixGame[(int)posToCheck.x, (int)posToCheck.y][1]) return false;

        //rotation check
        if (ID.ToString()[2] == 1)
        {
            if (ID.ToString()[3] == 0.ToString()[0] || ID.ToString()[3] == 2.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x - 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x - 1, (int)posToCheck.y][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x + 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x + 1, (int)posToCheck.y][2] != 0) return true;
                else return false;
            }
            else if (ID.ToString()[3] == 1.ToString()[0] || ID.ToString()[3] == 3.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y - 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y - 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y + 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y + 1][2] != 0) return true;
                else return false;
            }
        }
        else if (ID.ToString()[2] == 2.ToString()[0])
        {
            if (ID.ToString()[3] == 0.ToString()[0])
            {
                if (IfTilePosExist(new Vector2 ((int)posToCheck.x, (int)posToCheck.y - 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y - 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x - 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x - 1, (int)posToCheck.y][2] != 0) return true;
                else return false;
            }
            else if (ID[3] == 1.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y - 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y - 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x + 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x + 1, (int)posToCheck.y][2] != 0) return true;
                else return false;
            }
            else if (ID[3] == 2.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y + 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y + 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x - 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x - 1, (int)posToCheck.y][2] != 0) return true;
                else return false;
            }
            if (ID[3] == 3.ToString()[0])
            {
                if (IfTilePosExist(new Vector2 ((int)posToCheck.x, (int)posToCheck.y + 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y + 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x - 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x - 1, (int)posToCheck.y][2] != 0) return true;
                else return false;
            }
        }
        if (ID.ToString()[2] == 3.ToString()[0])
        {
            if (ID[3] == 0.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x - 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x - 1, (int)posToCheck.y][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x + 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x + 1, (int)posToCheck.y][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y - 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y - 1][2] != 0) return true;
                else return false;
            }
            else if (ID[3] == 1.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y + 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y + 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2 ((int)posToCheck.x, (int)posToCheck.y - 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y - 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x + 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x + 1, (int)posToCheck.y][2] != 0) return true;
                else return false;
            }
            else if (ID[3] == 2.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x - 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x - 1, (int)posToCheck.y][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x + 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x + 1, (int)posToCheck.y][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y + 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y + 1][2] != 0) return true;
                else return false;
            }
            if (ID[3] == 3.ToString()[0])
            {
                if (IfTilePosExist(new Vector2((int)posToCheck.x, (int)posToCheck.y + 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y + 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2 ((int)posToCheck.x, (int)posToCheck.y - 1)) && matrixGame[(int)posToCheck.x, (int)posToCheck.y - 1][2] != 0) return true;
                else if (IfTilePosExist(new Vector2((int)posToCheck.x - 1, (int)posToCheck.y)) && matrixGame[(int)posToCheck.x - 1, (int)posToCheck.y][2] != 0) return true;
                else return false;
            }
        }

        return false;
    }

    //make sure the position exist
    bool IfTilePosExist(Vector2 posToCheck)
    {
        if (posToCheck.x >= matrixSize.x || posToCheck.x < 0) return false;
        else if (posToCheck.y >= matrixSize.y || posToCheck.y < 0) return false;
        else return true;
    }
}
