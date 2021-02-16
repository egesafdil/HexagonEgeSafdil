using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] private int score = 100; //Score for dimond
    #endregion

    #region Caches
    private Rigidbody2D myRigidbody = null; //Referance to rigidbody
    private GridMaker gridMaker = null; //Referance to gridMaker component
    protected GameCore gameCore = null; //Referance to gameCore component
    private int lastRow = 9; //gridRow in the gridMaker
    private float timer = 0f; //Timer that stablizes the fall downs
    #endregion

    //Other diamonds around this diamond
    #region AroundDiamonds
    private GameObject diamondUp = null;
    private GameObject diamondRightUp = null;
    private GameObject diamondRightDown = null;
    private GameObject diamondDown = null;
    private GameObject diamondLeftDown = null;
    private GameObject diamondLeftUp = null;
    #endregion

    #region Public methods
    //Renames the diamond based on the positon
    public void RenameDiamond()
    {
        this.name = transform.position.x + "," + transform.position.y;
    }
    
    //Finds the other diamonds and checks if they have the same color
    public void FindAround()
    {
        diamondDown = GameObject.Find((transform.position.x) + "," + (transform.position.y - 2f));
        diamondUp = GameObject.Find((transform.position.x) + "," + (transform.position.y + 2f));
        diamondRightUp = GameObject.Find((transform.position.x + 1.5f) + "," + (transform.position.y + 1f));
        diamondRightDown = GameObject.Find((transform.position.x + 1.5f) + "," + (transform.position.y - 1f));
        
        diamondLeftDown = GameObject.Find((transform.position.x - 1.5f) + "," + (transform.position.y - 1f));
        diamondLeftUp = GameObject.Find((transform.position.x - 1.5f) + "," + (transform.position.y + 1f));

        CheckSameColor();
    }
    #endregion

    protected void Start()
    {
        GameCore.CheckSameColorEvent += ListenEvent; //Subscribes the event in the gameCore component
        gridMaker = FindObjectOfType<GridMaker>();
        gameCore = FindObjectOfType<GameCore>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.bodyType = RigidbodyType2D.Static;

        lastRow = (gridMaker.Rows * -2) + 2; //Calculates the last row

        ListenEvent(); //Runs the event just in case
        
    }

    void OnDestroy()
    {
        GameCore.CheckSameColorEvent -= ListenEvent; //Unsubscribes the event on destroy
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If there is no diamond down this diamond and
        //If it is not in the last row
        if(diamondDown == null && transform.position.y > lastRow) 
        {
            FallDown();
        }
        //If it is reached the position
        else if (transform.position.y <= lastRow)
        {
            //Make rigidbody static
            myRigidbody.bodyType = RigidbodyType2D.Static;

            //Set the position
            String posX = transform.position.x.ToString();
            float posY = posX.Contains(",") ? lastRow -1 : lastRow;
            transform.position = new Vector2(transform.position.x, Mathf.Round(posY));
        }
    }

    //Runs this method if event is invoked
    private void ListenEvent()
    {
        RenameDiamond();
        FindAround();
    }

    //Falls down
    private void FallDown()
    {
        //If diamond is reached the position
        if(timer > 0f && Mathf.Abs(myRigidbody.velocity.y) <= 0.5f)
        {
            myRigidbody.bodyType = RigidbodyType2D.Static; //Makes static
            transform.position = new Vector2(transform.position.x, Mathf.Round(transform.position.y)); // Sets the position
            timer = 0f; //Resets the timer

            GameCore.InvokeEvent(); //Invokes the look around event
        }
        //Make it fall
        else
        {
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            this.name = "Falling";
            
            timer += Time.deltaTime;
            
        }              
    }

    //Checks the colors
    private void CheckSameColor()
    {
        //If any diamond around this diamond isn't null, get its color
        Color currentDiamondColor = GetComponent<SpriteRenderer>().color;
        Color diamondUpColor = diamondUp!=null ? diamondUp.GetComponent<SpriteRenderer>().color: currentDiamondColor;
        Color diamondRightUpColor = diamondRightUp!=null ? diamondRightUp.GetComponent<SpriteRenderer>().color: currentDiamondColor;
        Color diamondRightDownColor = diamondRightDown!=null ? diamondRightDown.GetComponent<SpriteRenderer>().color: currentDiamondColor;
        Color diamondDownColor = diamondDown!=null ? diamondDown.GetComponent<SpriteRenderer>().color: currentDiamondColor;
        Color diamondLeftDownColor = diamondLeftDown!=null ? diamondLeftDown.GetComponent<SpriteRenderer>().color: currentDiamondColor;
        Color diamondLeftUpColor = diamondLeftUp!=null ? diamondLeftUp.GetComponent<SpriteRenderer>().color: currentDiamondColor;

        //Compare color based on hexegon then destroy
        if (diamondUp != null && diamondRightUp != null &&
            (currentDiamondColor == diamondUpColor) && (currentDiamondColor == diamondRightUpColor))
        {
            DestroyMatchedDiamonds(diamondUp, diamondRightUp);
        }
        else if (diamondRightUp != null && diamondRightDown != null &&
            (currentDiamondColor == diamondRightUpColor) && (currentDiamondColor == diamondRightDownColor))
        {
            DestroyMatchedDiamonds(diamondRightUp, diamondRightDown);
        }
        else if (diamondRightDown != null && diamondDown != null &&
            (currentDiamondColor == diamondRightDownColor) && (currentDiamondColor == diamondDownColor))
        {
            DestroyMatchedDiamonds(diamondRightDown, diamondDown);
        }
        else if (diamondDown != null && diamondLeftDown != null &&
            (currentDiamondColor == diamondDownColor) && (currentDiamondColor == diamondLeftDownColor))
        {
            DestroyMatchedDiamonds(diamondDown, diamondLeftDown);
        }
        else if (diamondLeftDown != null && diamondLeftUp != null &&
            (currentDiamondColor == diamondLeftDownColor) && (currentDiamondColor == diamondLeftUpColor))
        {
            DestroyMatchedDiamonds(diamondLeftDown, diamondLeftUp);
        }
        else if(diamondLeftUp != null && diamondUp != null &&
            (currentDiamondColor == diamondLeftUpColor) && (currentDiamondColor == diamondUpColor))
        {
            DestroyMatchedDiamonds(diamondLeftUp, diamondUp);
        }
    }

    //Destroy the diamonds
    private void DestroyMatchedDiamonds(GameObject diamond1, GameObject diamond2)
    {
        DiamondMovement.destroyed = true; //Set it true

        gameCore.IncreaseScore(score); //Increase the score

        //Destroy
        Destroy(diamond1);
        Destroy(diamond2);
        Destroy(gameObject);
    }    
}
