using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public GameObject scoreScreen;
    public ScoreHandler scoreScript;
    public GameObject baseObject;
    public Base baseScript;
    public GameObject w;
    public GameObject a;
    public GameObject d;
    public GameObject s;
    // Start is called before the first frame update
    void Start()
    {
        scoreScreen = GameObject.Find("Score");
        scoreScript = scoreScreen.GetComponent<ScoreHandler>();
        pauseMenu.SetActive(false);
        baseObject = GameObject.Find("Base");
        baseScript = baseObject.GetComponent<Base>();
         w = GameObject.Find("W");
         a = GameObject.Find("A");
         s = GameObject.Find("S");
         d = GameObject.Find("D");

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
        w.SetActive(false);
        a.SetActive(false);
        s.SetActive(false);
        d.SetActive(false);
    }
    public void ResumeTheGame()
    {
        //Resumes time
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        scoreScreen.SetActive(true);
        w.SetActive(true);
        a.SetActive(true);
        s.SetActive(true);
        d.SetActive(true);
    }
        public void ExitToMenu()
    {
        //Load main menu scene.
        SceneManager.LoadScene(0);
    }
    public void Restart()
    {
        baseScript.Initialize(2, 2, 14);
        baseScript.UpgradeBase();
        scoreScript.curScore = 0;
        scoreScript.rows = 0;
        scoreScript.updateScore();
        ResumeTheGame();
    }
}
