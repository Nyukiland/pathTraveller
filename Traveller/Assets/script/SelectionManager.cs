using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    //controls the selection of a tile

    [Header("visual and placement variable")]
    [SerializeField] GameObject selectedPlacement;
    [SerializeField] GameObject cancelTile;
    [SerializeField] GameObject rotateTile;

    [Header ("feedback variable (DO NOT TOUCH)")]
    [Tooltip("shows current tile currently selected")] public GameObject selectedTile;
    [Tooltip("store the position of tile")] public Vector3 posTile;

    //set of none nessecerry visible variable, mostly feature  
    float selectedPlacementPos;

    private void Update()
    {
        RaycastSelection();
        VisibleSelection();
    }

    void RaycastSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("CanSelect") && selectedTile == null)
                {
                    AttachSelected(hit);
                }
                else if (hit.collider.CompareTag("CanSelect") && selectedTile != null)
                {
                    DetachSelected();
                    AttachSelected(hit);
                }
                else if (hit.collider.gameObject == cancelTile && selectedTile != null)
                {
                    DetachSelected();
                }
                else if (hit.collider.gameObject == rotateTile && selectedTile != null)
                {
                    selectedTile.transform.SetParent(null);
                    selectedTile.transform.Rotate(selectedTile.transform.up, 90);
                    selectedTile.transform.SetParent(selectedPlacement.transform);
                }
            }
        }
    }

    void DetachSelected()
    {
        selectedTile.tag = "CanSelect";

        selectedTile.transform.SetParent(null);
        selectedTile.transform.position = posTile;

        selectedTile = null;
        posTile = Vector3.zero;
    }

    void AttachSelected(RaycastHit hitOBJ)
    {
        selectedTile = hitOBJ.collider.gameObject;
        posTile = hitOBJ.collider.gameObject.transform.position;

        selectedTile.transform.SetParent(selectedPlacement.transform);
        selectedTile.transform.localPosition = new Vector3(0, 0, 0);

        selectedTile.tag = "Selected";
    }

    void VisibleSelection()
    {
        if (selectedTile == null && selectedPlacement.transform.position.y > -1)
        {
            selectedPlacementPos -= (1 * Time.deltaTime);
        }
        else if (selectedTile != null && selectedPlacement.transform.position.y < 0)
        {
            selectedPlacementPos += (1 * Time.deltaTime);
        }

        selectedPlacementPos = Mathf.Clamp(selectedPlacementPos, -1, 0);
        selectedPlacement.transform.position = new Vector3(selectedPlacement.transform.position.x, selectedPlacementPos, selectedPlacement.transform.position.z);
    }
}