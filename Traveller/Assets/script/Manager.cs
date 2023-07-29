using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header ("Difficulty control")]
    [Space(2)]
    [Tooltip("1 is easy, 2 is normal, 3 is hard")] public static int difficulty = 1;
    [SerializeField] int matrixEasy;
    [SerializeField] int matrixNormal;
    [SerializeField] int matrixHard;
    /*
    Those matrix (multidimensionnal array) are filled with int each representing a value for the system to represent the capacity of a player
    0 is the mountain it is interpreted as nothing can be placed
    1 is an enmpty slot where anything can be placed
    2 is a forest road where only dirt and city can be placed
    3 is dirt where only 
    */
    [Space (10)]
    [Tooltip("matrix are used in order to represent the playground")] [Header("matrix control (read only)")]
    [Space(2)]
    [SerializeField] int[][] matrixGame;

    [Space(10)]
    [Tooltip("set object for both teams")] [Header("Prefab list")]
    [Space(2)]
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
        if(difficulty ==1)
        {
            //matrixGame = new int[matrixEasy][matrixEasy];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
