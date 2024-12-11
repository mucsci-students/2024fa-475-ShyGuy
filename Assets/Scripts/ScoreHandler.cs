using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class ScoreHandler : MonoBehaviour
{
    [SerializeField] TMP_Text scoreCount;
    public GameObject baseObject;
    public Base baseScript;
    public GameObject shapeObject;
    public Shape shapeScript;
    public GameObject gameManager;
    public GameManager gameScript;
    public int curScore;
    public int rows;
    public long curBlock;
    // Start is called before the first frame update
    void Start()
    {
        rows = 0;
        curScore = 0;
        curBlock = -1;
        baseObject = GameObject.Find("Base");
        baseScript = baseObject.GetComponent<Base>();
        shapeObject = GameObject.Find("Shape");
        shapeScript = shapeObject.GetComponent<Shape>();
        gameManager = GameObject.Find("GameManager");
        gameScript = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameScript.skipHere == true)
        {
            curBlock += 1;
            gameScript.skipHere = false;

        }
        if (!baseScript.gameOver)
        {
            if (baseScript.clearedRows > rows)
            {
                curScore += 100;
                rows += 1;
                updateScore();
            }
            if (shapeScript.currentBlockId != curBlock)
            {
                curBlock += 1;
                updateScore();
                curScore += (shapeScript.blockCount * 10);
            }
        }
        else
        {
            gameScript.GameOver(); // MDF: Temporary design
            StartCoroutine(WaitAndLoadScene());
        }

        IEnumerator WaitAndLoadScene()
        {
            yield return new WaitForSeconds(10f); // Wait for 10 seconds
            SceneManager.LoadScene("Main Menu");
        }

        // MDF: Temporary design
        if (curScore >= 1000)
        {
            Base.Instance.UpgradeBase();
            curScore = 0;
            updateScore();
        }
    }
    public void updateScore()
    {
        scoreCount.text = "Score: " + curScore;
        // if(reset)
        // {
        //     scoreCount.text = "Score: " + 0;
        // }
    }
}
