using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Whether or not the player can use keys at this moment
    public bool playerKeyControlled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if (playerKeyControlled)
        {
            CheckKeyInput();
        }
    }

    public void CheckKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Move forward

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // Move backward
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // Move left
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // Move right
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            // Rotate horizontally
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            // Rotate vertically
        }
    }
}