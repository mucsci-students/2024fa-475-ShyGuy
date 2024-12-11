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
    public int scoreLimit;
    public int upgradeCount;
    // Start is called before the first frame update
    void Start()
    {
        scoreLimit = 1000;
        upgradeCount = 1;
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
            curScore -= 50;
            gameScript.skipHere = false;
            updateScore();

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
            TextDisplay.Instance.TriggerText("What is that? A plane?");
            StartCoroutine(WaitAndLoadScene());
        }

        IEnumerator WaitAndLoadScene()
        {
            yield return new WaitForSeconds(10f); // Wait for 10 seconds
            SceneManager.LoadScene("Main Menu");
        }

        // MDF: Temporary design
        if (curScore >= (scoreLimit + (upgradeCount * 500)))
        {
            Base.Instance.UpgradeBase();
            TextDisplay.Instance.TriggerText("Good job! What's next? A bigger one?");
            curScore = 0;
            upgradeCount +=1;
            updateScore();
        }
    }
    public void updateScore()
    {
        scoreCount.text = "Score: " + curScore;
    }
}
