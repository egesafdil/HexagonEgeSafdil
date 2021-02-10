using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DiamondMovement : MonoBehaviour
{
    [SerializeField] private GameObject outline = null; //Outline prefab

    public static bool destroyed = false; //Static value that holds if any diamond is destroyed

    #region Caches
    private GameCore gameCore = null; //Referance to the gameCore component
    GameObject diamondOutline = null; //Outline object on the scene

    //Diamonds which are selected by player
    GameObject diamond1 = null;
    GameObject diamond2 = null;
    GameObject diamond3 = null;

    //Mouse positions to compare for rotating
    float mouseDownPosition = 0f;
    float mouseUpPosition = 0f;

    bool tilesSelected = false; //If diamonds are selected
    bool alreadyRotating = false; //If any diamond is rotating
    #endregion
    
    void Start()
    {
        gameCore = GetComponent<GameCore>();
    }

    void Update()
    {
        InputSystem(); //Looks for input
    }

    //Input system for selecting and rotating
    private void InputSystem()
    {
        //Gets the mouse down position
        if(Input.GetMouseButtonDown(0) && !alreadyRotating)
        {
            mouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        }
        //Gets mouse up position
        //Selects or rotates
        else if(Input.GetMouseButtonUp(0) && !alreadyRotating)
        {
            mouseUpPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            
            //Selects the diamonds if there is just a click
            if (Mathf.Abs(mouseUpPosition - mouseDownPosition) <= 0.5f)
            {
                SelectTiles();
            }
            //Rotates if there is a swipe
            else if(mouseDownPosition < mouseUpPosition && tilesSelected)
            {
                StartCoroutine(RotateSelectedTiles(false)); //Rotates counter clock wise
            }
            else if(mouseDownPosition > mouseUpPosition && tilesSelected)
            {
                StartCoroutine(RotateSelectedTiles(true)); //Rotates clock wise
            }
        }
    }

    //Selects the diamonds
    private void SelectTiles()
    {
        //If an outline already exist, destroyes it
        if(diamondOutline != null)
        {
            Destroy(diamondOutline);
        }

        //Raycasts the position
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        //If hits nothing returns the method
        if(hit.collider == null)
        {
            return;
        }

        diamond1 = hit.collider.gameObject;

        diamondOutline = Instantiate(outline, transform);
        
            
        //Selects diamonds and draws outline based on positions
        if (worldPoint.x < diamond1.transform.position.x && worldPoint.y < diamond1.transform.position.y)
        {        
            diamond3 = GameObject.Find((diamond1.transform.position.x - 1.5f) + "," + (diamond1.transform.position.y - 1f));
            diamond2 = GameObject.Find((diamond1.transform.position.x) + "," + (diamond1.transform.position.y - 2f));
                
            diamondOutline.transform.position = new Vector2(diamond1.transform.position.x - 0.52f, diamond1.transform.position.y - 1f);
            diamondOutline.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else if (worldPoint.x < diamond1.transform.position.x && worldPoint.y > diamond1.transform.position.y)
        {
            diamond2 = GameObject.Find((diamond1.transform.position.x - 1.5f) + "," + (diamond1.transform.position.y + 1f));
            diamond3 = GameObject.Find((diamond1.transform.position.x) + "," + (diamond1.transform.position.y + 2f));

            diamondOutline.transform.position = new Vector2(diamond1.transform.position.x - 0.52f, diamond1.transform.position.y + 1f);
            diamondOutline.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else if (worldPoint.x > diamond1.transform.position.x && worldPoint.y < diamond1.transform.position.y)
        {
            diamond2 = GameObject.Find((diamond1.transform.position.x + 1.5f) + "," + (diamond1.transform.position.y - 1f));
            diamond3 = GameObject.Find((diamond1.transform.position.x) + "," + (diamond1.transform.position.y - 2f));

            diamondOutline.transform.position = new Vector2(diamond1.transform.position.x + 0.52f, diamond1.transform.position.y - 1f);
                
        }
        else if (worldPoint.x > diamond1.transform.position.x && worldPoint.y > diamond1.transform.position.y)
        {
            diamond3 = GameObject.Find((diamond1.transform.position.x + 1.5f) + "," + (diamond1.transform.position.y + 1f));
            diamond2 = GameObject.Find((diamond1.transform.position.x) + "," + (diamond1.transform.position.y + 2f));

            diamondOutline.transform.position = new Vector2(diamond1.transform.position.x + 0.52f, diamond1.transform.position.y + 1f);
                
        }

        //If diamonds cannot be chosen, return
        if(diamond2 == null || diamond3 == null)
        {
            Destroy(diamondOutline);
            diamond1 = null;
            tilesSelected = false;
            return;
        }

        tilesSelected = true;
    }

    //Rotates the diamonds 3 times 
    private IEnumerator RotateSelectedTiles(bool clockwise)
    {        
        alreadyRotating = true; //Sets true because diamonds are rotating
        int turningCount = 0; //Counts turnings

        do {
            //Gets positions
            Vector2 diamond1Position = diamond1.transform.position;
            Vector2 diamond2Position = diamond2.transform.position;
            Vector2 diamond3Position = diamond3.transform.position;
            
            //Rotates clockwise
            if(clockwise)
            {
                diamond1.transform.position = diamond3Position;
                diamond2.transform.position = diamond1Position;
                diamond3.transform.position = diamond2Position;
                
            }
            //Rotates counter clock wise
            else
            {
                diamond1.transform.position = diamond2Position;
                diamond2.transform.position = diamond3Position;
                diamond3.transform.position = diamond1Position;
            }            

            turningCount++; //Increases counter
            yield return new WaitForSeconds(1f); //Waits for a second
            GameCore.InvokeEvent(); //Invokes event to tell all diamonds to check their around
            Destroyed(); // Increases movement counter
        }
        while(turningCount < 3 && !destroyed);

        //Resets
        alreadyRotating = false;
        destroyed = false;
    }

    //Looks for destroying
    private void Destroyed()
    {
        //If any diamond is destroyed
        if(destroyed)
        {
            gameCore.IncreaseMovement(); //Increases the movement
            Destroy(diamondOutline);
            tilesSelected = false;

        }
        
    }
}
