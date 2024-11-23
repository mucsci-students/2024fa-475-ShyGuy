using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public float fallTime = 1.0f;
    private float previousTime;
    private TetrisManager tm;

    private void Awake()
    {
        tm = FindObjectOfType<TetrisManager>();
    }

    private void Update()
    {
        HandleInput();
        HandleFall();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate(90);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            FallOneStep();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            HardDrop();
        }
    }

    private void HandleFall()
    {
        if (Time.time - previousTime >= fallTime)
        {
            FallOneStep();
            previousTime = Time.time;
        }
    }

    private void Move(Vector3 direction)
    {
        transform.position += direction;

        if (!IsCurrentPositionValid())
        {
            transform.position -= direction; // Revert move if invalid
        }
    }

    private void Rotate(float angle)
    {
        transform.Rotate(0, 0, angle);

        if (!IsCurrentPositionValid())
        {
            transform.Rotate(0, 0, -angle); // Revert rotation if invalid
        }
    }

    private void FallOneStep()
    {
        Move(Vector3.down);

        if (!IsCurrentPositionValid())
        {
            transform.position -= Vector3.down; // Revert move
            tm.UpdateGrid(transform);          // Update grid with final position

            tm.SpawnBrick();                   // Spawn a new brick
            enabled = false;                   // Disable this brick's script
        }
    }

    private void HardDrop()
    {
        while (IsCurrentPositionValid())
        {
            transform.position += Vector3.down;
        }
        transform.position -= Vector3.down; // Revert the last invalid move

        tm.UpdateGrid(transform);
        tm.SpawnBrick();
        enabled = false;
    }

    private bool IsCurrentPositionValid()
    {
        foreach (Transform child in transform)
        {
            Vector3 roundedPosition = tm.Round(child.position);

            if (!tm.IsInsideGrid(roundedPosition) || tm.IsOccupied(roundedPosition))
            {
                return false;
            }
        }
        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Brick"))
        {
            tm.UpdateGrid(transform);
            tm.SpawnBrick();
            enabled = false;
        }
    }
}
