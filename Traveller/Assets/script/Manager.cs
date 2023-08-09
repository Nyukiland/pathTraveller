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
    [Tooltip("number of tile in the hand of player")] [Range (1, 6)] [SerializeField] int inHandSize;

    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("GUI")]
    [Space(2)]
    [Tooltip("shows current tile in the hand of the player")] [SerializeField] GameObject sideHand;
    [Tooltip("visuale split between playground and hand")] [SerializeField] GameObject limitZone;


    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("Script variable field)")]
    [Space(2)]
    [Tooltip("need the selection manager script")] [SerializeField] SelectionManager scriptSelect;

    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("In hand quantity and deck itself (feedback variable DO NOT TOUCH)")]
    [Space(2)]
    [Tooltip("shows current tile in the hand of the player")] [SerializeField] List<GameObject> deck;
    [Tooltip("shows current tile in the hand of the player")] [SerializeField] GameObject[] inHand;

    /*
    Those matrix (multidimensionnal array) are filled with int each representing a value for the system to represent the capacity of a player
    5 digits are used to give the right amount of information.

    First digit is the state of tile. 0 is not placable, 1 is placable, 2 is never placable

    Second digit is the level of the tile. 0 is empty, 1 is level 1(forest), 2 is level 2(dirt), 3 is level 3(city)

    Third digit is the type of road. 0 is empty, 1 is straight, 2 is a turn, 3 is a triple road

    Four digit is the person reponsable for the tile. 0 is no player, 1 is player 1, 2 is player 2

    Fith digit is the rotation of the tile. 0 is 0 degree, 1 is 90 degree, 2 is 180 degree, 3 is 270 degree

    */
    int[,] matrixGame; //most important multidimensionnal array


    //set of none nessecerry visible variable, mostly set up or feature
    GameObject[,] matrixOBJ;
    int[] rotationTilePossible = new int[] { 0, 90, 180, 270 };
    Vector2 exitPoint;


    // Start is called before the first frame update
    void Start()
    {
        //automatically set the camera
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -matrixSize.y-5);
        Camera.main.orthographicSize = matrixSize.y;

        //set up the scene and game
        SetUp();
        CreateDeck();

        //visual feedback for the game
        limitZone.transform.position = new Vector3(matrixOBJ[(int)matrixSize.x - 1, 0].transform.position.x + 5, sideHand.transform.position.y, matrixOBJ[0, (int)matrixSize.y / 2].transform.position.z + 2);
        sideHand.transform.position = new Vector3(matrixOBJ[(int)matrixSize.x - 1, 0].transform.position.x + 15, sideHand.transform.position.y, matrixOBJ[0, (int)matrixSize.y / 2].transform.position.z + 2);

        //create the hand
        InHandSetUp();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetUp()
    {
        //create proper sized multidimensionnal array
        matrixGame = new int[(int)matrixSize.x, (int)matrixSize.y];
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
                matrixGame[i, j] = 00000;

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
                matrixGame[randomInt, k] += 20000;
            }
        }

        //determine the first tile as playable
        matrixGame[(int)matrixSize.x / 2, 0] += 10000;

        //determine the tile that will give the win
        exitPoint = new Vector2((int)matrixSize.x / 2, matrixSize.y - 1);
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
        inHand = new GameObject[inHandSize];

        for (int i = 0; i < inHandSize; i++)
        {
            //create the tile
            int rng = Random.Range(0, deck.Count);
            GameObject clone = Instantiate(deck[rng], transform.position, Quaternion.identity);

            //change the transform 
            clone.transform.position = new Vector3(sideHand.transform.position.x, sideHand.transform.position.y, sideHand.transform.position.z + ((i * 4) + (2 * (1 - inHandSize))));

            //remove it from the deck and add it to the array
            deck.RemoveAt(rng);
            inHand[i] = clone;

            //add tag to it
            clone.tag = "CanSelect";

        }
    }
}
