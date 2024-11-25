using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public static Shape Instance { get; private set; }

    public static long blockIdCounter = 0;

    public static Color[] availableColors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        new Color(1f, 0.5f, 0f), // Orange
        new Color(0.5f, 0f, 1f), // Purple
    };

    public bool[,,] currentShape;
    public long currentBlockId;
    public Color currentColor;

    private Transform[,,] subCubes; // Array to hold references to the 27 sub GameObjects

    // Position in integers, (0, 0, 0) is at bottom left
    public Vector3Int currentPos = new(0, 0, 0);

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

    void Start()
    {
        // Initialize subCubes array
        subCubes = new Transform[3, 3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    // Find sub GameObjects named like "Cubexyz"
                    Transform subCube = transform.Find($"Cube{x}{y}{z}");
                    if (subCube == null)
                    {
                        Debug.Log($"Cube{x}{y}{z} not found! Game is broken!");
                    }
                    subCubes[x, y, z] = subCube;
                }
            }
        }

        GetNewShape();
    }

    public void GetNewShape()
    {
        SpawnRandomShape();
        UpdateShapeDisplay();
    }

    public void SpawnRandomShape()
    {
        GenerateRandomShape();
        GenerateRandomColor();
        GenerateBlockId();
    }

    private void GenerateRandomShape()
    {
        currentShape = new bool[3, 3, 3];
        int cnt = 0;
        for (int x = 0; x < 3; ++x)
        {
            for (int z = 0; z < 3; ++z)
            {
                currentShape[x, 0, z] = Random.Range(0, 2) == 1;
                if (currentShape[x, 0, z])
                {
                    ++cnt;
                }
            }
        }
        if (cnt == 0)
        {
            currentShape[1, 0, 1] = true;
        }

        for (int y = 1; y < 3; ++y)
        {
            for (int x = 0; x < 3; ++x)
            {
                for (int z = 0; z < 3; ++z)
                {
                    if (currentShape[x, y - 1, z])
                    {
                        currentShape[x, y, z] = Random.Range(0, 2) == 1;
                    }
                }
            }
        }
    }

    private void GenerateRandomColor()
    {
        currentColor = availableColors[Random.Range(0, availableColors.Length)];
    }

    private void GenerateBlockId()
    {
        currentBlockId = blockIdCounter++;
    }

    public void RotateHorizontally()
    {
        bool[,,] rotated = new bool[3, 3, 3];
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    rotated[z, y, 2 - x] = currentShape[x, y, z];
                }
            }
        }
        currentShape = rotated;
        UpdateShapeDisplay();
    }

    public void RotateVertically()
    {
        bool[,,] rotated = new bool[3, 3, 3];
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    rotated[2 - y, x, z] = currentShape[x, y, z];
                }
            }
        }
        currentShape = rotated;
        UpdateShapeDisplay();
    }

    public void UpdateShapeDisplay()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (currentShape[x, y, z])
                    {
                        // Cube exists, set it to active and update color
                        subCubes[x, y, z].gameObject.SetActive(true); // Make the cube visible
                        subCubes[x, y, z].GetComponent<Renderer>().material.color = currentColor;
                    }
                    else
                    {
                        // Cube does not exist, set it to inactive
                        subCubes[x, y, z].gameObject.SetActive(false); // Hide the cube
                    }
                }
            }
        }
    }

    public int LowestY(int x, int z)
    {
        for (int y = 0; y < 3; y++)
        {
            if ((currentShape[x, y, z]))
            {
                return y;
            }
        }
        return int.MaxValue / 3;
    }
}