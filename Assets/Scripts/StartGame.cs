using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame : MonoBehaviour
{

    public void StartGameNow()
    {
        //Loads game scene.
        // MDF: Temporary design
        SceneManager.LoadScene("MDF_playground");
    }
    public void Instructions()
    {
        //Open instruction menu.
    }
    public void Settings()
    {
        //Settings menu
    }
    public void QuitGame(){
        Application.Quit();
    }
}