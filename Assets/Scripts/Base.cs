using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public static Base Instance { get; private set; }

    public int width;
    public int depth;
    public int height;
    private Dictionary<Vector3Int, Block> grid;

    private GameObject topRightCameraObject;

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
        Initialize(8, 8, 16); // Example initialization
        AddTopRightCamera();  // Add the top-right camera
    }

    void Update()
    {
        UpdateTopRightCamera();
    }

    public void UpdateTopRightCamera()
    {
        topRightCameraObject.transform.position = new Vector3(width / 2f, height + 10f, depth / 2f);
    }


    // Initialize the grid with dimensions
    public void Initialize(int width, int depth, int height)
    {
        this.width = width;
        this.depth = depth;
        this.height = height;

        grid = new Dictionary<Vector3Int, Block>();
    }

    public void AddTopRightCamera()
    {
        // Create a new camera
        topRightCameraObject = new GameObject("TopRightCamera");
        Camera topRightCamera = topRightCameraObject.AddComponent<Camera>();

        // Set the camera's position above the base
        float cameraHeight = height + 10f; // Adjust for a better view
        topRightCameraObject.transform.position = new Vector3(width / 2f, cameraHeight, depth / 2f);

        // Make the camera look down at the base
        topRightCameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Top-down view

        // Set the camera's viewport in the top-right corner
        topRightCamera.rect = new Rect(0.75f, 0.75f, 0.2f, 0.2f); // Adjust as needed for size and position
    }


    // Resize the grid for a new game
    public void ResizeBase(int newWidth, int newDepth, int newHeight)
    {
        width = newWidth;
        depth = newDepth;
        height = newHeight;
        Initialize(newWidth, newDepth, newHeight); // Re-initialize the grid
    }

    // Place a block shape in the grid
    public bool PlaceBlock()
    {
        var shape = Shape.Instance;
        Vector3Int shapePosition = shape.currentPos;
        bool[,,] currentShape = shape.currentShape;
        Color shapeColor = shape.currentColor;
        long blockId = shape.currentBlockId;

        int heightDiff = int.MaxValue;
        for (int x = 0; x < 3; ++x)
        {
            for (int z = 0; z < 3; ++z)
            {
                Vector3Int pos = shape.currentPos + new Vector3Int(x, 0, z);
                heightDiff = Mathf.Min(heightDiff,
                    pos.y + shape.LowestY(x, z) - HighestY(pos.x, pos.z));
            }
        }
        --heightDiff;

        // Add the shape to the grid
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (currentShape[x, y, z])
                    {
                        Vector3Int blockPosition = shapePosition + new Vector3Int(x, y, z);
                        blockPosition.y -= heightDiff;

                        if (blockPosition.y >= height)
                        {
                            // Game over
                            Debug.Log("Game over!");
                            return false;
                        }

                        // Add or update block in the grid
                        if (!grid.ContainsKey(blockPosition))
                        {
                            grid[blockPosition] = new Block();
                            grid[blockPosition].Cube = CreateCube(blockPosition, shapeColor);
                        }

                        var block = grid[blockPosition];
                        block.Color = shapeColor;
                        block.BlockId = blockId;
                        block.IsVisible = true;
                    }
                }
            }
        }
        Shape.Instance.GetNewShape();
        return true;
    }

    // Create a cube GameObject for a block
    private GameObject CreateCube(Vector3Int position, Color color)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = transform; // Attach to the Base GameObject
        cube.transform.localPosition = position;
        cube.transform.localScale = Vector3.one; // Ensure 1 unit size
        cube.GetComponent<Renderer>().material.color = color;
        return cube;
    }

    // Destroy a cube GameObject
    private void DestroyCube(GameObject cube)
    {
        if (cube != null)
        {
            Destroy(cube);
        }
    }

    // Check and clear full levels
    public void CheckAndClearFullLevels()
    {
        for (int y = 0; y < height;)
        {
            if (IsLevelFull(y))
            {
                ClearLevel(y);
                MoveDownLevelsAbove(y);
            }
            else
            {
                ++y;
            }
        }
    }

    private bool IsLevelFull(int y, int tolerance = 5)
    {
        int cnt = 0;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                var position = new Vector3Int(x, y, z);
                if (!grid.ContainsKey(position))
                {
                    ++cnt;
                }
            }
        }
        return cnt <= tolerance;
    }

    private void ClearLevel(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                var position = new Vector3Int(x, y, z);
                if (grid.ContainsKey(position))
                {
                    DestroyCube(grid[position].Cube);
                    grid.Remove(position); // Remove the block from the grid
                }
            }
        }
    }

    private void MoveDownLevelsAbove(int clearedLevel)
    {
        // Assume the bottom level is cleared

        for (int y = clearedLevel + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    var currentPosition = new Vector3Int(x, y, z);
                    var belowPosition = new Vector3Int(x, y - 1, z);

                    if (grid.ContainsKey(currentPosition))
                    {
                        grid[belowPosition] = grid[currentPosition];
                        grid.Remove(currentPosition);
                    }
                }
            }
        }
    }

    public int HighestY(int x, int z)
    {
        for (int y = height - 1; y >= 0; y--)
        {
            var position = new Vector3Int(x, y, z);
            if (grid.ContainsKey(position))
            {
                return y;
            }
        }
        return -1;
    }
}
