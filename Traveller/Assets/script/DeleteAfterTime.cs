using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour
{
    [SerializeField]
    float timeBeforeDestroy;

    private void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
    }
}
