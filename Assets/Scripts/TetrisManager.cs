using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : MonoBehaviour
{
    public Vector3 spawnPoint = new Vector3(0, 20, 0);
    public GameObject[] Bricks;
    public int width = 10;
    public int height = 20;

    private Transform[,] grid;

    public void Start()
    {
        grid = new Transform[width, height];
        SpawnBrick();
    }

    public void Update()
    {
        CheckRows();
    }

    public void SpawnBrick()
    {
        int randomIndex = Random.Range(0, Bricks.Length);
        Instantiate(Bricks[randomIndex], spawnPoint, Quaternion.identity);
    }

    public bool IsPositionValid(Transform block)
    {
        foreach (Transform child in block)
        {
            Vector3 pos = Round(child.position);

            if (!IsInsideGrid(pos) || IsOccupied(pos))
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateGrid(Transform block)
    {
        foreach (Transform child in block)
        {
            Vector3 pos = Round(child.position);

            if (IsInsideGrid(pos))
            {
                grid[(int)pos.x, (int)pos.y] = child; // Only update positions within bounds
            }
        }
    }


    Vector3 Round(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    }

    bool IsInsideGrid(Vector3 pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    bool IsOccupied(Vector3 pos)
    {
        return grid[(int)pos.x, (int)pos.y] != null;
    }

    void CheckRows()
    {
        for (int y = 0; y < height; y++)
        {
            if (IsRowFull(y))
            {
                ClearRow(y);
                MoveRowsDown(y + 1);
                y--; 
            }
        }
    }

    bool IsRowFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
            {
                return false; 
            }
        }
        return true;
    }


    void ClearRow(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    void MoveRowsDown(int startY)
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
}
