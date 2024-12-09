using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    GameObject scoreScreen;
    // Start is called before the first frame update
    void Start()
    {
        scoreScreen = GameObject.Find("Score");
        pauseMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Time.timeScale != 0)
            {
                PauseTheGame();
            }
            else
            ResumeTheGame();
        }        
    }
    public void PauseTheGame()
    {
        //Stops time
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        scoreScreen.SetActive(false);
    }
    public void ResumeTheGame()
    {
        //Resumes time
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        scoreScreen.SetActive(true);
    }
        public void ExitToMenu()
    {
        //Load main menu scene.
        SceneManager.LoadScene(0);
    }
}
