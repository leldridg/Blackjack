using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public void HandOver()
    {
        Debug.Log("Entered HandOver");
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
