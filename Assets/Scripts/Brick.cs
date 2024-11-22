using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public float fallTime = 1.0f;
    private float previousTime;
    private TetrisManager tetrisManager;

    public void Start()
    {
        tetrisManager = FindObjectOfType<TetrisManager>();
        if (!tetrisManager.IsPositionValid(transform))
        {
            Debug.Log("Game Over");
            enabled = false;
        }
    }

    public void Update()
    {
        HandleInput();
        HandleFall();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left;
            if (!tetrisManager.IsPositionValid(transform))
                transform.position -= Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right;
            if (!tetrisManager.IsPositionValid(transform))
                transform.position -= Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, 90);
            if (!tetrisManager.IsPositionValid(transform))
                transform.Rotate(0, 0, -90);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            FallOneStep();
        }
    }

    void HandleFall()
    {
        if (Time.time - previousTime >= fallTime)
        {
            FallOneStep();
            previousTime = Time.time;
        }
    }

    void FallOneStep()
    {
        transform.position += Vector3.down;

        if (!tetrisManager.IsPositionValid(transform))
        {
            transform.position -= Vector3.down;
            tetrisManager.UpdateGrid(transform);

            //spawn a new piece
            tetrisManager.SpawnBrick();

            enabled = false;
        }
    }
    void OnPieceLanded()
    {
        FindObjectOfType<TetrisManager>().UpdateGrid(transform);
        FindObjectOfType<TetrisManager>().SpawnBrick();
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Brick"))
        {
            OnPieceLanded();
        }
    }


}
