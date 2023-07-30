using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header ("Difficulty control")]
    [Space(2)]
    [Tooltip("1 is easy, 2 is normal, 3 is hard")] public static int difficulty = 1;
    [Tooltip("x is the number of tile on each side (better if odd) and y is the number of tile towards you")] [SerializeField] Vector2 matrixEasy;
    [Tooltip("x is the number of tile on each side (better if odd) and y is the number of tile towards you")] [SerializeField] Vector2 matrixNormal;
    [Tooltip("x is the number of tile on each side (better if odd) and y is the number of tile towards you")] [SerializeField] Vector2 matrixHard;


    /*
    Those matrix (multidimensionnal array) are filled with int each representing a value for the system to represent the capacity of a player
    5 digits are used to give the right amount of information.

    First digit is the state of tile. 0 is not used and placable, 1 is used and not placable, 2 is a used by a player

    Second digit is the level of the tile. 0 is empty, 1 is level 1(forest), 2 is level 2(dirt), 3 is level 3(city)

    Third digit is the type of road. 0 is empty, 1 is straight, 2 is a turn, 3 is a triple road
    
    Four digit is the person reponsable for the tile. 0 is no player, 1 is player 1, 2 is player 2

    Fith digit is the rotation of the tile. 0 is 0 degree, 1 is 90 degree, 2 is 180 degree, 3 is 270 degree
    
    */
    [Header("------------------------------------------------------")]
    [Space (10)]
    [Tooltip("matrix are used in order to represent the playground")] [Header("matrix control (read only)")]
    [Space(2)]
    [SerializeField] int[,] matrixGame; //most important multidimensionnal array
    Vector3[,] matrixPosition; 

    [Space(10)]
    [Tooltip("set object for both teams")] [Header("Prefab list")]
    [Space(2)]

    [SerializeField] GameObject[] prefabBegin;

    [Space]

    [SerializeField] GameObject[] prefabBlue;

    [Space]

    [SerializeField] GameObject[] prefabRed;

    [Tooltip("set the quantity for each tile in every difficulty")] [Header("Prefab list")]
    [Space(2)]
    [SerializeField] int[] easyQuantity;
    [SerializeField] int[] normalQuantity;
    [SerializeField] int[] hardQuantity;

    // Start is called before the first frame update
    void Start()
    {
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
        matrixGame = new int[(int)quantity.x, (int)quantity.y];
        matrixPosition = new Vector3[(int)quantity.x, (int)quantity.y];

        for (int i = 0; i < quantity.x; i++)
        {
            for (int j = 0; j < quantity.y; j++)
            {
                GameObject clone = Instantiate(prefabBegin[Random.Range(0, prefabBegin.Length)], new Vector3((j*2) - (quantity.y/2), 0, (i*2)- (quantity.x/2)), Quaternion.identity);
                clone.transform.eulerAngles = new Vector3(-90, 0, 0);
                matrixPosition[i, j] = clone.transform.position;
                IdentificationInMatrix(clone, i, j);
            }
        }
    }

    void IdentificationInMatrix(GameObject tilePlace ,int col, int line)
    {
        if (tilePlace == prefabBegin[0]) matrixGame[col, line] = 10000;
        else matrixGame[col, line] = 00000;
    }
}
