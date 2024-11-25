using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : MonoBehaviour
{
    public Vector3 spawnPoint = new Vector3(5, 20, 0); // Spawn point for bricks
    public GameObject[] Bricks; // Array of brick prefabs
    public int width = 10; // Grid width
    public int height = 30; // Grid height

    private Transform[,] grid; // 2D array to track the grid
    public GameObject currentBrick; // Reference to the currently active brick
    private int lastBrickIndex = -1; // Avoid consecutive duplicates

    void Start()
    {
        grid = new Transform[width, height]; // Initialize the grid
        SpawnBrick(); // Spawn the first brick
    }

    void Update()
    {
        if (currentBrick == null)
        {
            CheckRows();
            SpawnBrick();
        }
    }



    public void SpawnBrick()
    {
        if (currentBrick == null) // Only spawn if there is no active brick
        {
            int randomIndex = Random.Range(0, Bricks.Length); // Pick a random brick
            currentBrick = Instantiate(Bricks[randomIndex], spawnPoint, Quaternion.identity); // Spawn it at the spawn point

            // Ensure the brick starts with no rotation
            currentBrick.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
    }


    private int GetRandomBrickIndex()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, Bricks.Length);
        } while (randomIndex == lastBrickIndex); // Avoid consecutive duplicates
        lastBrickIndex = randomIndex;
        return randomIndex;
    }

    public void UpdateGrid(Transform brick)
    {
        foreach (Transform child in brick)
        {
            Vector3 pos = Round(child.position);
            if (IsInsideGrid(pos))
            {
                grid[(int)pos.x, (int)pos.y] = child; // Add to grid
            }
        }
    }

    public bool IsPositionValid(Vector3[] blockPositions)
    {
        foreach (Vector3 position in blockPositions)
        {
            Vector3 pos = Round(position);
            if (!IsInsideGrid(pos) || IsOccupied(pos))
            {
                return false;
            }
        }
        return true;
    }

    public void CheckRows()
    {
        for (int y = 0; y < height; y++)
        {
            if (IsRowFull(y))
            {
                ClearRow(y);
                MoveRowsDown(y + 1);
                y--; // Recheck after rows are moved down
            }
        }
    }

    private bool IsRowFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null) return false;
        }
        return true;
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    private void MoveRowsDown(int startY)
    {
        for (int y = startY; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }

    private bool IsInsideGrid(Vector3 pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private bool IsOccupied(Vector3 pos)
    {
        return grid[(int)pos.x, (int)pos.y] != null;
    }

    private Vector3 Round(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    }
}
