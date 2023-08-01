using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    //this script only contains function for the selection of object

    public void RaycastSelection(GameObject selection)
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("CanSelect") && selection == null)
                {
                    selection = hit.collider.gameObject;
                    selection.tag = "Selected";
                }
                else if (hit.collider.CompareTag("Selected") && selection == hit.collider.gameObject)
                {
                    selection.tag = "CanSelect";
                    selection = null;
                }
                else if (hit.collider.CompareTag("place"))
                {

                }
            }
        }
    }

    void feedbackVisu(GameObject select)
    {
        
    }
}
