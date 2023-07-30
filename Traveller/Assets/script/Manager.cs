using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Difficulty control")]
    [Space(2)]
    [Tooltip("x is the number of tile on each side (better if odd) and y is the number of tile towards you")] [SerializeField] Vector2 matrixEasy;
    [Tooltip("x is the number of tile on each side (better if odd) and y is the number of tile towards you")] [SerializeField] Vector2 matrixNormal;
    [Tooltip("x is the number of tile on each side (better if odd) and y is the number of tile towards you")] [SerializeField] Vector2 matrixHard;
    [Space]
    [Tooltip("0 is everytime and 10 is 10% (mountains appear every 3 tile if possible)")] [Range(0, 10)] [SerializeField] int probabilityOfMountain;

    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("Prefab list")]
    [Space(2)]

    [Tooltip("element 0 should be mountains, others are juste empty grass")] [SerializeField] GameObject[] prefabBegin;

    [Space]

    [Tooltip("order should be the same as the set quantity")] [SerializeField] GameObject[] prefabBlue;

    [Space]

    [Tooltip("order should be the same as the set quantity")] [SerializeField] GameObject[] prefabRed;


    [Space(10)]
    [Header("------------------------------------------------------")]
    [Header("Tile deck quantity")]
    [Space(2)]
    [Tooltip("order should be the same as the prefab list")] [SerializeField] int[] easyQuantity;
    [Tooltip("order should be the same as the prefab list")] [SerializeField] int[] normalQuantity;
    [Tooltip("order should be the same as the prefab list")] [SerializeField] int[] hardQuantity;

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
    public static int difficulty = 1;



    // Start is called before the first frame update
    void Start()
    {
        //verify which diffficulty has been selected
        if (difficulty == 1) SetUp(matrixEasy);
        else if (difficulty == 2) SetUp(matrixNormal);
        else if (difficulty == 3) SetUp(matrixHard);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUp(Vector2 quantity)
    {
        //create proper sized multidimensionnal array
        matrixGame = new int[(int)quantity.x, (int)quantity.y];
        matrixOBJ = new GameObject[(int)quantity.x, (int)quantity.y];

        //generate the grass (/empty) tile and fill the multidimensionnal array
        for (int i = 0; i < quantity.x; i++)
        {
            for (int j = 0; j < quantity.y; j++)
            {
                //creation of the tile
                GameObject clone = Instantiate(prefabBegin[Random.Range(1, prefabBegin.Length)], new Vector3(((i * 2) - (quantity.x / 2) - 0.5f), 0, -j * 2), Quaternion.identity);
                clone.transform.eulerAngles = new Vector3(-90, 0, rotationTilePossible[Random.Range(0, rotationTilePossible.Length)]);

                //multidiemnsionnal array control
                matrixOBJ[i, j] = clone;
                matrixGame[i, j] = 00000;

            }
        }

        //generate the mountains (/never placeable tile) and correct the multidimensionnal array (start at 1 to avoid softlock)
        for (int k = 1; k < quantity.y; k += 3)
        {
            //random the spawn possibility
            int shouldIHaveAMountain = Random.Range(0, probabilityOfMountain);

            //make sure it doesn't instantiate on the last raw so it doesn't block the exit
            if (k != quantity.y - 1 && shouldIHaveAMountain == 0)
            {
                //creation of the tile
                int randomInt = Random.Range(0, (int)quantity.x);
                GameObject clone = Instantiate(prefabBegin[0], matrixOBJ[randomInt, k].transform.position, Quaternion.identity);
                clone.transform.eulerAngles = new Vector3(-90, 0, rotationTilePossible[Random.Range(0, rotationTilePossible.Length)]);

                //multidiemnsionnal array control
                Destroy(matrixOBJ[randomInt, k]);
                matrixOBJ[randomInt, k] = clone;
                matrixGame[randomInt, k] += 20000;
            }
        }

        //determine the first tile as playable
        matrixGame[(int)quantity.x / 2, 0] += 10000;
    }

    void IdentificationInMatrix(int col, int line)
    {

    }
}
