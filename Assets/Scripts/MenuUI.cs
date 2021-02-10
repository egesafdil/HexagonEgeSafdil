using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    //Method that controls start button in MENU scene
    public void StartButton()
    {
        SceneManager.LoadScene("Level");
    }   
}
