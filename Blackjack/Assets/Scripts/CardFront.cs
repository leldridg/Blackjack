using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    public Vector3 playerHandLocation;
    public Vector3 dealerHandLocation;
    public Vector3 targetRotation;
    public float rotationSpeed;
    public float movementSpeed;
    public List<Sprite> cardSprites;
    private int index;
    private SpriteRenderer spriteRenderer;
    private bool isRotating;
    private Vector3 targetLocation;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite.name + ": " + transform.position);
        if (tag == "Player")
        {
            targetLocation = playerHandLocation;
            isRotating = true;
            
        }
        else if (tag == "Dealer")
        {
            targetLocation = dealerHandLocation;
            isRotating = true;
        }
        else
        {
            Debug.LogWarning("Tag may not be set: " + tag);
        }
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
