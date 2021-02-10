using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCore : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] Text scoreText = null; //Referance to score text in Canvas.
    [SerializeField] Text movementText = null; //Referance to movement counter text in Canvas
    #endregion

    #region Caches
    private int score = 0; //Holds score for scoreText
    private int movement = 0; //Holds movement for movementText
    private GridMaker gridMaker = null; //Reference for gridMaker component which creates grid
    #endregion

    #region Public Events
    public static event Action CheckSameColorEvent; //Event that all diamonds listen to check other diamonds around them
    public static bool isMoving = false; //Holds if diamonds are moving
    #endregion

    #region Getters
    public int Score { get { return score; } } //Getter for score
    public int Movement { get { return movement; } } //Getter for movement
    #endregion

    #region Public methods
    //Method that increases the score if diamonds explode
    public void IncreaseScore(int score)
    {
        this.score += score;
        gridMaker.IncreaseBombScoreCounter(score);
    }

    //Method that counts every successful movement that player does    
    public void IncreaseMovement()
    {
        movement++;
    }

    //Static method that invokes CheckSameColorEvent and all diamonds check other diamonds around them
    public static void InvokeEvent()
    {
        CheckSameColorEvent?.Invoke();
    }

    //Method that loads menu scene if bomb explodes
    public void GameOver()
    {
        SceneManager.LoadScene("Menu");

    }
    #endregion

    void Start()
    {
        gridMaker = GetComponent<GridMaker>(); //Finds the component for caching
    }

    void Update()
    {
        //Updates UI elements
        movementText.text = "#" + movement; 
        scoreText.text = "Score: " + score;
    }

    

    
}
