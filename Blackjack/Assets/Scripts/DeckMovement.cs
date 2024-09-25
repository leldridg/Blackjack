using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{

    public Vector3 onScreen;
    public Vector3 offScreen;
    public float speed;
    private bool isMovingUp;
    private bool isMovingDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, offScreen, speed * Time.deltaTime);
            if (transform.position == offScreen)
            {
                isMovingUp = false;
            }
        }
        if (isMovingDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, onScreen, speed * Time.deltaTime);
            if (transform.position == onScreen)
            {
                isMovingDown = false;
            }
        }
    }

    public void MoveOffscreen()
    {
        isMovingUp = true;
    }

    public void MoveOnScreen()
    {
        isMovingDown = true;
    }
}
