using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] private Diamond tile = null; //Referance to diamond prefab
    [SerializeField] private BombDiamond bombTile = null; //Referance to bomb diamond prefab
    [SerializeField] private int rows = 9; //Row number in the grid
    [SerializeField] private int columns = 8; //Column number in the grid
    [SerializeField] List<Color> diamondColors = null; //Colors for diamonds
    [SerializeField] private int sendBombScore = 1000; //Score that bomb needs to be sent
    #endregion

    #region Caches
    private GameCore gameCore = null; //Cache for gameCore component
    private int bombScoreCounter = 0; //Holds every 1000 score
    #endregion

    #region Getters
    public int Rows { get { return rows; } }
    #endregion

    //Saves the new score for bomb
    public void IncreaseBombScoreCounter(int score)
    {
        bombScoreCounter += score;
    }

    void Start()
    {
        gameCore = GetComponent<GameCore>(); //Finds the component for caching

        //If diamond tile is null do nothing
        if(tile == null)
        {
            Debug.LogError("Tile cannot be null!");
        }
        else
        {
            CreateTiles(); //Creates diamond at the start
            StartCoroutine(SearchForMissingDiamonds()); //Looks to send new diamond if there is any missing
        }
    }

    // Method that creates diamonds
    private void CreateTiles()
    {
        Color previousColor = Color.black; //Temporary color

        //Loops for rows and columns
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Diamond diamond = Instantiate(tile, transform);
                
                Color newColor = ColorChooser();
                //If the color of diamond is same as previous one picks a new color
                while (newColor == previousColor)
                {
                    newColor = ColorChooser();
                }

                previousColor = newColor; // Sets new color as previous color

                diamond.GetComponent<SpriteRenderer>().color = newColor;

                //X and Y positions
                float posX = column * 1.5f;
                float posY = 0f;

                //Calculates position of hexagon correctly
                if(column % 2 != 0)
                {
                    posY = row * -2f - 1f;
                }
                else
                {
                    posY = row * -2f;
                }
                
                //Sets the position of diamond
                diamond.transform.position = new Vector2(posX, posY);
            }
        }
    }

    // Method that chooses random color
    private Color ColorChooser()
    {
        int randomColorIndex = Random.Range(0, diamondColors.Count); //Picks a color randomly
        return diamondColors[randomColorIndex];
    }

    //Method that sends new diamonds
    private void SendNewDiamonds()
    {
        for (int column = 0; column < columns; column++)
        {
            //Calculates position for new diamond
            float positionX = column * 1.5f;
            float positionY = 0f;
            if(column % 2 != 0)
            {
                positionY = -1f;
            }

            //If no diamond exist on the location creates new diamond
            if(GameObject.Find(positionX + "," + positionY) == null)
            {
                //If score counter for the bomb is more than the score that bomb needs to be sent
                //Sends bomb diamond
                if(bombScoreCounter >= sendBombScore)
                {
                    BombDiamond newDiamond = Instantiate(bombTile, transform);
                    newDiamond.GetComponent<SpriteRenderer>().color = ColorChooser(); //Picks a random color
                    newDiamond.transform.position = new Vector2(positionX, positionY); //Sets the position
                    bombScoreCounter = 0; //Resets the counter
                }
                //Sends regular diamond
                else
                {
                    Diamond newDiamond = Instantiate(tile, transform);
                    newDiamond.GetComponent<SpriteRenderer>().color = ColorChooser(); //Picks a rabdom color
                    newDiamond.transform.position = new Vector2(positionX, positionY); //Sets the position
                }
            }
        }
    }

    //Looks for missing diamond every 1 second
    private IEnumerator SearchForMissingDiamonds()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SendNewDiamonds();
        }
        
    }

}
