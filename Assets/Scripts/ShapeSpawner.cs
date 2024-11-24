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
    public long     currentBlockId;
    public Color    currentColor;

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
        GetNewShape();
    }

    public void GetNewShape()
    {
        SpawnRandomShape();
    }

    public void SpawnRandomShape()
    {
        GenerateRandomShape();
        GenerateRandomColor();
        GenerateBlockId();
    }

    private void GenerateRandomShape()
    {
        // Create a random shape within 3x3x3 space
        currentShape = new bool[3, 3, 3];
        // Build the bottom level first
        for (int x = 0; x < 3; ++x)
        {
            for (int z = 0; z < 3; ++z)
            {
                currentShape[x, 0, z] = Random.Range(0, 2) == 1;
            }
        }
        // Build the rest two levels
        for (int y = 1; y < 3; ++y)
        {
            for (int x = 0; x < 3; ++x)
            {
                for (int z = 0; z < 3; ++z)
                {
                    if (currentShape[x, y - 1, z])
                    {
                        // Only when their is a block immediately below this block
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
        // Rotate around the Y-axis
        bool[,,] rotated = new bool[3, 3, 3];
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    // Swap X and Z, and reverse the Z index
                    rotated[z, y, 2 - x] = currentShape[x, y, z];
                }
            }
        }
        currentShape = rotated;
    }

    public void RotateVertically()
    {
        // Rotate around the Z-axis
        bool[,,] rotated = new bool[3, 3, 3];
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    // Swap X and Y, and reverse the X index
                    rotated[2 - y, x, z] = currentShape[x, y, z];
                }
            }
        }
        currentShape = rotated;
    }
}