using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public static Shape Instance { get; private set; }

    public static long blockIdCounter = 0;
    public int blockCount;

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

    private Transform[,,] subCubes; // Array to hold references to the 8 sub GameObjects

    // Position in integers, (0, 0, 0) is at bottom left
    public Vector3Int currentPos = new(0, 0, 0);

    private Camera[] cameras = new Camera[6];

    Vector3[] camerasOffsets = new Vector3[]
        {
        new Vector3(0, 1, -5),  // Front
        new Vector3(0, 0, 5),   // Back
        new Vector3(5, 0, 0),   // Left
        new Vector3(-5, 0, 0),  // Right
        new Vector3(0, -5, 0),  // Top
        new Vector3(0, 5, 0)    // Bottom
        };

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
        subCubes = new Transform[2, 2, 2];
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
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

        // Ensure the shape starts at the right height
        transform.position = new(0, 5, 0);

        //AddSixCamerasAround();
    }

    void Update()
    {
        UpdateShapeDisplay();
        //UpdateCameraPositions();
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
        currentShape = new bool[2, 2, 2];
        blockCount = 0;
        int cnt = 0;
        for (int x = 0; x < 2; ++x)
        {
            for (int z = 0; z < 2; ++z)
            {
                currentShape[x, 0, z] = Random.Range(0, 2) == 1;
                if (currentShape[x, 0, z])
                {
                    ++cnt;
                    ++blockCount;
                }
            }
        }
        if (cnt == 0)
        {
            currentShape[0, 0, 0] = true; // Ensure at least one block exists
            ++blockCount;
        }

        for (int y = 1; y < 2; ++y)
        {
            for (int x = 0; x < 2; ++x)
            {
                for (int z = 0; z < 2; ++z)
                {
                    if (currentShape[x, y - 1, z])
                    {
                        currentShape[x, y, z] = Random.Range(0, 2) == 1;
                        if(currentShape[x, y, z])
                        blockCount++;
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

    public void AddSixCamerasAround()
    {
        // Grid layout for camera viewports (3x2 grid in the top-left corner, smaller size)
        Rect[] viewports = new Rect[]
        {
        new Rect(0f, 0.66f, 0.16f, 0.16f), // Top-left
        new Rect(0.17f, 0.66f, 0.16f, 0.16f), // Top-middle
        new Rect(0f, 0.50f, 0.16f, 0.16f), // Middle-left
        new Rect(0.17f, 0.50f, 0.16f, 0.16f), // Middle-right
        new Rect(0f, 0.34f, 0.16f, 0.16f), // Bottom-left
        new Rect(0.17f, 0.34f, 0.16f, 0.16f)  // Bottom-right
        };

        // Create and configure cameras
        for (int i = 0; i < 6; i++)
        {
            GameObject cameraObject = new GameObject($"Camera_{i}");
            Camera cam = cameraObject.AddComponent<Camera>();

            // Position the camera based on the offset
            cameraObject.transform.position = transform.position + camerasOffsets[i];

            // Make the camera look at the shape
            cameraObject.transform.LookAt(transform.position);

            // Assign a viewport to each camera
            cam.rect = viewports[i];

            // Store the camera in a list for dynamic updates
            cameras[i] = cam;
        }
    }

    public void RotateHorizontally()
    {
        bool[,,] rotated = new bool[2, 2, 2];
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                for (int z = 0; z < 2; z++)
                {
                    rotated[z, y, 1 - x] = currentShape[x, y, z];
                }
            }
        }
        currentShape = rotated;
    }

    public void RotateVertically()
    {
        bool[,,] rotated = new bool[2, 2, 2];
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                for (int z = 0; z < 2; z++)
                {
                    rotated[1 - y, x, z] = currentShape[x, y, z];
                }
            }
        }
        currentShape = rotated;
    }

    public void UpdateCameraPositions()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            // Update camera position
            cameras[i].transform.position = transform.position + camerasOffsets[i];
        }
    }

    public void UpdateShapeDisplay()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
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
        for (int y = 0; y < 2; y++)
        {
            if ((currentShape[x, y, z]))
            {
                return y;
            }
        }
        return int.MaxValue / 3;
    }
}
