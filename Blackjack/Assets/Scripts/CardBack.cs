using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBack : MonoBehaviour
{
    public Vector3 targetLocation;
    public Vector3 targetRotation;
    public float rotationSpeed;
    public float movementSpeed;
    private bool isRotating;
    public List<Sprite> cards = new List<Sprite>();
    
    // Start is called before the first frame update
    void Start()
    {
        isRotating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            Vector3 currentRotation = transform.eulerAngles;
            Vector3 newRotationEuler = Vector3.MoveTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = newRotationEuler;
            
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, movementSpeed * Time.deltaTime);
            
            if (transform.eulerAngles == targetRotation && transform.position == targetLocation)
            {
                isRotating = false;
            }
        }
    }
}
