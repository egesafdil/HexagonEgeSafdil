using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDiamond : Diamond
{
    #region SerializeFields
    [SerializeField] private int bombExplodeMovementCounter = 3; //Counter for bomb to explode
    [SerializeField] private TextMesh text = null; //Referance to textMesh on the bomb diamond
    #endregion

    //Cache
    private int oldMovement = 0;

    //Decreases movement counter for the bomb and sets the text
    public void DecreaseMovementCounter()
    {
        bombExplodeMovementCounter--;
        text.text = bombExplodeMovementCounter.ToString();

        if(bombExplodeMovementCounter <= 0)
        {
            gameCore.GameOver();
        }
    }

    new void Start()
    {
        base.Start();
        text.text = bombExplodeMovementCounter.ToString();
        gameCore = FindObjectOfType<GameCore>();
    }

    void Update()
    {
        //Checks if it needs to count down
        if(gameCore.Movement - oldMovement >= 1)
        {
            oldMovement = gameCore.Movement;
            DecreaseMovementCounter();
        }
    }
}
