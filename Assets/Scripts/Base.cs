using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Base : MonoBehaviour
{
    public static Base Instance { get; private set; }

    public int width;
    public int depth;
    public int height;
    public int clearedRows;
    public bool gameOver;
    private Dictionary<Vector3Int, Block> grid;

    private GameObject topRightCameraObject;

    private GameObject planeParent;
    private Dictionary<Vector3Int, GameObject> planeSquares;

    public SoundManager audio1;

    public GameObject arrowsParent;

    public GameObject arrowPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        audio1 = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        gameOver = false;
        Initialize(4, 4, 16); // Example initialization
        AddTopRightCamera();  // Add the top-right camera
        CreateBasePlane();    // Create the plane of squares
    }

    void Update()
    {
        UpdateTopRightCamera();
        UpdatePlaneHighlights();
        if(gameOver)
            audio1.PauseTracks();
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
        this.clearedRows = 0;

        if (grid != null)
        {
            foreach (var block in grid)
            {
                Destroy(block.Value.Cube);
            }
        }
        grid = new Dictionary<Vector3Int, Block>();
    }

    public void AddTopRightCamera()
    {
        // Create a new camera
        topRightCameraObject = new GameObject("TopRightCamera");
        Camera topRightCamera = topRightCameraObject.AddComponent<Camera>();

        // Set the camera's position above the base
        topRightCameraObject.transform.position = new Vector3(width / 2f, height + 10f, depth / 2f);

        // Make the camera look down at the base
        topRightCameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Top-down view

        // Set the camera's viewport in the top-right corner
        topRightCamera.rect = new Rect(0.75f, 0.75f, 0.2f, 0.2f); // Adjust as needed for size and position
    }

    // Create a plane of squares at (0, 0, 0)
    private void CreateBasePlane()
    {
        planeParent = new GameObject("BasePlane");
        planeParent.transform.parent = transform;

        planeSquares = new Dictionary<Vector3Int, GameObject>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3Int position = new Vector3Int(x, 0, z);
                GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
                square.transform.parent = planeParent.transform;
                square.transform.localPosition = new Vector3(x + 0.5f, -0.5f, z + 0.5f); // Base level at -0.5
                square.transform.localRotation = Quaternion.Euler(90, 0, 0); // Face up
                square.transform.localScale = new Vector3(1, 1, 1); // Grid-aligned
                square.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Color"));
                square.GetComponent<Renderer>().material.color = Color.gray; // Initial gray color

                planeSquares[position] = square;
            }
        }

        CreateArrows();
    }

    private void CreateArrows()
    {
        arrowsParent = new GameObject("DirectionArrows");
        arrowsParent.transform.parent = planeParent.transform;

        // Create arrows
        CreateArrow(new Vector3(width / 2.0f - 0.5f, -0.5f, depth - 0.5f + 1.0f), Vector3.forward, Color.red, arrowsParent.transform); // Forward
        CreateArrow(new Vector3(width - 0.5f + 1.0f, -0.5f, depth / 2.0f - 0.5f), Vector3.right, Color.yellow, arrowsParent.transform); // Right
        CreateArrow(new Vector3(width / 2.0f - 0.5f, -0.5f, -0.5f - 1.0f), Vector3.back, Color.blue, arrowsParent.transform); // Back
        CreateArrow(new Vector3(-0.5f - 1.0f, -0.5f, depth / 2.0f - 0.5f), Vector3.left, Color.green, arrowsParent.transform); // Left
    }

    private void CreateArrow(Vector3 position, Vector3 direction, Color color, Transform parent)
    {
        if (arrowPrefab == null)
        {
            Debug.Log("Need to be assigned in the scene.");
            return;
        }

        // Instantiate the arrow at the specified position and parent
        GameObject arrowInstance = Instantiate(arrowPrefab, position, Quaternion.identity, parent);

        // Rotate the arrow so it faces the given direction, aligning with the world-up vector (0, 1, 0).
        // Use the FromToRotation to rotate from 'up' (0,1,0) to the direction, which ensures proper orientation
        arrowInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        if (direction == Vector3.left)
        {
            arrowInstance.transform.rotation *= Quaternion.Euler(0, -90, 0);
        }
        else if (direction == Vector3.right)
        {
            arrowInstance.transform.rotation *= Quaternion.Euler(0, 90, 0);
        }
        else if (direction == Vector3.back)
        {
            arrowInstance.transform.rotation *= Quaternion.Euler(0, 180, 0);
        }

        // Access the subgameobjects and change their colors
        Transform part1 = arrowInstance.transform.GetChild(0); // First subgameobject
        Transform part2 = arrowInstance.transform.GetChild(1); // Second subgameobject

        // Apply the color to both subgameobjects' MeshRenderer components
        Renderer part1Renderer = part1.GetComponent<Renderer>();
        part1Renderer.material.color = color;   

        Renderer part2Renderer = part2.GetComponent<Renderer>();
        part2Renderer.material.color = color;
    }

    public void ResetArrowColor(string facing)
    {
        if (facing == "right") // Camera is facing roughly to the right
        {
            arrowsParent.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // forward
            arrowsParent.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // forward
            arrowsParent.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // right
            arrowsParent.transform.GetChild(1).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // right
            arrowsParent.transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // back
            arrowsParent.transform.GetChild(2).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // back
            arrowsParent.transform.GetChild(3).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // left
            arrowsParent.transform.GetChild(3).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // left
        }
        // Check camera facing left
        else if (facing == "left") // Camera is facing roughly to the left
        {
            arrowsParent.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // forward
            arrowsParent.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // forward
            arrowsParent.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // right
            arrowsParent.transform.GetChild(1).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // right
            arrowsParent.transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // back
            arrowsParent.transform.GetChild(2).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // back
            arrowsParent.transform.GetChild(3).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // left
            arrowsParent.transform.GetChild(3).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // left
        }
        // Check camera facing forward
        else if (facing == "forward") // Camera is facing forward
        {
            arrowsParent.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // forward
            arrowsParent.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // forward
            arrowsParent.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // right
            arrowsParent.transform.GetChild(1).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // right
            arrowsParent.transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // back
            arrowsParent.transform.GetChild(2).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // back
            arrowsParent.transform.GetChild(3).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // left
            arrowsParent.transform.GetChild(3).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // left
        }
        // Check camera facing backward
        else if (facing == "back") // Camera is facing backward
        {
            arrowsParent.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // forward
            arrowsParent.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.blue; // forward
            arrowsParent.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // right
            arrowsParent.transform.GetChild(1).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.green; // right
            arrowsParent.transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // back
            arrowsParent.transform.GetChild(2).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.red; // back
            arrowsParent.transform.GetChild(3).GetChild(0).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // left
            arrowsParent.transform.GetChild(3).GetChild(1).GetComponent<Renderer>().material.color = UnityEngine.Color.yellow; // left
        }
    }

    // Update highlights based on the highest block on the base
    private void UpdatePlaneHighlights()
    {
        foreach (var square in planeSquares)
        {
            Vector3Int position = square.Key;
            GameObject squareObject = square.Value;

            // Find the highest block directly above this square on the base
            int highestY = -1;

            // Iterate vertically from the base up to find the highest block on the base (not the shape)
            for (int y = 0; y < height; y++) // Iterate vertically from the base up
            {
                Vector3Int blockPosition = new Vector3Int(position.x, y, position.z);
                if (grid.ContainsKey(blockPosition)) // If a block exists at this position in the grid
                {
                    highestY = y; // Update the highest Y value
                }
            }

            // Now, check if there is a shape cube directly above this square
            bool isShapeAbove = false;
            var shape = Shape.Instance;
            Vector3Int shapePosition = shape.currentPos;
            bool[,,] currentShape = shape.currentShape;

            for (int x = 0; x < 2; x++) // Assuming the shape spans up to 2 units in x
            {
                for (int z = 0; z < 2; z++) // Assuming the shape spans up to 2 units in z
                {
                    for (int y = 0; y < 2; y++) // Assuming the shape spans up to 2 units in y
                    {
                        if (currentShape[x, y, z]) // If there's a shape cube
                        {
                            Vector3Int blockPosition = shapePosition + new Vector3Int(x, y, z);

                            // Check if this block is directly above the current square
                            if (blockPosition.x == position.x && blockPosition.z == position.z)
                            {
                                isShapeAbove = true;
                                break; // Break early if a shape cube is found above
                            }
                        }
                    }
                }
            }

            // Highlight the square if there's a shape cube above it and adjust position
            if (isShapeAbove)
            {
                squareObject.GetComponent<Renderer>().material.color = Color.white; // Highlight with white
                                                                                    // Move the square to the top of the highest block already placed on the base
                squareObject.transform.localPosition = new Vector3(position.x, 0.51f + highestY, position.z); // Adjust position
            }
            else
            {
                // Reset the square's color and position if no shape cube is above it
                squareObject.GetComponent<Renderer>().material.color = Color.gray; // Use white for highlight
                squareObject.transform.localPosition = new Vector3(position.x, -0.5f, position.z); // Base level at -0.5
            }
        }
    }

    // Resize the grid for a new game
    public void ResizeBase(int newWidth, int newDepth, int newHeight)
    {
        width = newWidth;
        depth = newDepth;
        height = newHeight;
        Initialize(newWidth, newDepth, newHeight); // Re-initialize the grid
    }

    // Upgrade the grid for a new game
    public void UpgradeBase()
    {
        // Increase dimensions
        width += 2;
        depth += 2;
        height += 2;

        // Reinitialize the grid and related structures
        Initialize(width, depth, height);

        // Update the top-right camera
        UpdateTopRightCamera();

        // Recreate the base plane to match the new dimensions
        Destroy(planeParent); // Remove the old plane
        CreateBasePlane();

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
        for (int x = 0; x < 2; ++x)
        {
            for (int z = 0; z < 2; ++z)
            {
                Vector3Int pos = shape.currentPos + new Vector3Int(x, 0, z);
                heightDiff = Mathf.Min(heightDiff,
                    pos.y + shape.LowestY(x, z) - HighestY(pos.x, pos.z));
            }
        }
        --heightDiff;

        // Add the shape to the grid
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    if (currentShape[x, y, z])
                    {
                        Vector3Int blockPosition = shapePosition + new Vector3Int(x, y, z);
                        blockPosition.y -= heightDiff;

                        if (blockPosition.y >= height)
                        {
                            // Game over
                            Debug.Log("Game over!");
                            gameOver = true;
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
        audio1.PlayEffect(audio1.drop);
        Shape.Instance.GetNewShape();

        CheckAndClearFullLevels();

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
                clearedRows++;
            }
            else
            {
                ++y;
            }
        }
    }

    public void ClearLevelAnyway(int y)
    {
        ClearLevel(y);
        MoveDownLevelsAbove(y);
        // No gain for this clear
        //clearedRows++;
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
        audio1.PlayEffect(audio1.rowClear);
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
                        grid[belowPosition].Cube.transform.position = belowPosition;
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
    public Vector3 Center()
    {
        return new Vector3(width / 2.0f, 0.0f, depth / 2.0f);
    }
}
