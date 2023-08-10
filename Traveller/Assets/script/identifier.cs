using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class identifier : MonoBehaviour
{
    //this variable could be automatically set up in the start if needed
    [Tooltip ("the code that correspond to the tile (need a precise identification see the script annotation for guide)")] public int identification = 00000;

    /*
    Those matrix (multidimensionnal array) are filled with int each representing a value for the system to represent the capacity of a player
    5 digits are used to give the right amount of information.

    First digit is the state of tile. 0 is not placable, 1 is placable, 2 is never placable

    Second digit is the level of the tile. 0 is empty, 1 is level 1(forest), 2 is level 2(dirt), 3 is level 3(city)

    Third digit is the type of road. 0 is empty, 1 is straight, 2 is a turn, 3 is a triple road

    Four digit is the person reponsable for the tile. 0 is no player, 1 is player 1, 2 is player 2

    Fith digit is the rotation of the tile. 0 is 0 degree, 1 is 90 degree, 2 is 180 degree, 3 is 270 degree
    */

    private void Start()
    {
        rotationForIdentifier();
    }

    public void rotationForIdentifier()
    {
        if (transform.rotation.y == 0) identification = identification.ToString()[0] + identification.ToString()[1] + identification.ToString()[2] + identification.ToString()[3] + 0;
        else if (transform.rotation.y == 90) identification = identification.ToString()[0] + identification.ToString()[1] + identification.ToString()[2] + identification.ToString()[3] + 1;
        else if (transform.rotation.y == 180) identification = identification.ToString()[0] + identification.ToString()[1] + identification.ToString()[2] + identification.ToString()[3] + 2;
        else if (transform.rotation.y == 270) identification = identification.ToString()[0] + identification.ToString()[1] + identification.ToString()[2] + identification.ToString()[3] + 3;
    }
}
