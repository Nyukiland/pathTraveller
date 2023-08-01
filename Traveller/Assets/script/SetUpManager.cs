using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpManager : MonoBehaviour
{
    //this script only contains the set up fonction for the game

    public void SetUp(int[,] matrixInt, GameObject[,] matrixGO, GameObject[] prefabB, int[] rota, int probaM, Vector2 end, Vector2 quantity)
    {
        //create proper sized multidimensionnal array
        matrixInt = new int[(int)quantity.x, (int)quantity.y];
        matrixGO = new GameObject[(int)quantity.x, (int)quantity.y];

        //generate the grass (/empty) tile and fill the multidimensionnal array
        for (int i = 0; i < quantity.x; i++)
        {
            for (int j = 0; j < quantity.y; j++)
            {
                //creation of the tile
                GameObject clone = Instantiate(prefabB[Random.Range(1, prefabB.Length)], new Vector3((i - ((quantity.x / 2) - 0.5f)) * 2, 0, -j * 2), Quaternion.identity);
                clone.transform.eulerAngles = new Vector3(-90, 0, rota[Random.Range(0, rota.Length)]);

                //multidiemnsionnal array control
                matrixGO[i, j] = clone;
                matrixInt[i, j] = 00000;

            }
        }

        //generate the mountains (/never placeable tile) and correct the multidimensionnal array (start at 1 to avoid softlock)
        for (int k = 1; k < quantity.y; k += 3)
        {
            //random the spawn possibility
            int shouldIHaveAMountain = Random.Range(0, probaM);

            //make sure it doesn't instantiate on the last raw so it doesn't block the exit
            if (k != quantity.y - 1 && shouldIHaveAMountain == 0)
            {
                //creation of the tile
                int randomInt = Random.Range(0, (int)quantity.x);
                GameObject clone = Instantiate(prefabB[0], matrixGO[randomInt, k].transform.position, Quaternion.identity);
                clone.transform.eulerAngles = new Vector3(-90, 0, rota[Random.Range(0, rota.Length)]);

                //multidiemnsionnal array control
                Destroy(matrixGO[randomInt, k]);
                matrixGO[randomInt, k] = clone;
                matrixInt[randomInt, k] += 20000;
            }
        }

        //determine the first tile as playable
        matrixInt[(int)quantity.x / 2, 0] += 10000;

        //determine the tile that will give the win
        end = new Vector2((int)quantity.x / 2, quantity.y - 1);
    }

    public void CreateDeck(List<GameObject> d, int[] quantityT, GameObject[] prefabT)
    {
        //create a deck of tile based on the quantity wanted
        for (int i = 0; i < quantityT.Length; i++)
        {
            for (int j = 0; j < quantityT[j] + 1; j++)
            {
                d.Add(prefabT[i]);
            }
        }
    }

    public void InHandSetUp(GameObject[] hand, int handS, List<GameObject> d, GameObject side)
    {
        hand = new GameObject[handS];

        for (int i = 0; i < handS; i++)
        {
            //create the tile
            int rng = Random.Range(0, d.Count);
            GameObject clone = Instantiate(d[rng], transform.position, Quaternion.identity);

            //change the transform 
            clone.transform.eulerAngles = new Vector3(-90, 0, 0);
            clone.transform.position = new Vector3(side.transform.position.x, side.transform.position.y, side.transform.position.z + ((i * 4) + (2 * (1 - handS))));

            //remove it from the deck and add it to the array
            d.RemoveAt(rng);
            hand[i] = clone;

            //add tag to it
            clone.tag = "CanSelect";

        }
    }
}
