using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBase : MonoBehaviour
{
    public static TetrisBase Instance { get; private set; }

    public int width;
    public int depth;
    public int height;
    private Block[,,] grid;

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
        Initialize(16, 16, 8); // Example initialization
    }

    // Initialize the grid with dimensions and default Block objects
    public void Initialize(int width, int depth, int height)
    {
        this.width  = width;
        this.depth  = depth;
        this.height = height;

        grid = new Block[width, height, depth];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    grid[x, y, z] = new Block(); // Default block
                }
            }
        }
    }

    // Resize the grid for a new game
    public void ResizeBase(int newWidth, int newDepth, int newHeight)
    {
        width  = newWidth;
        depth  = newDepth;
        height = newHeight;
        Initialize(newWidth, newDepth, newHeight); // Re-initialize the grid
    }

    // Place a block shape in the grid
    public bool PlaceBlock(Vector3Int position, List<Vector3Int> shape, Color color, long blockId)
    {
        foreach (var offset in shape)
        {
            var blockPosition = position + offset;
            if (!IsPositionValid(blockPosition))
            {
                Debug.LogError("Cannot place block, position is invalid.");
                return false;
            }
        }

        foreach (var offset in shape)
        {
            var blockPosition = position + offset;
            var block = grid[blockPosition.x, blockPosition.y, blockPosition.z];
            block.IsOccupied = true;
            block.Color      = color;
            block.BlockId    = blockId;
            block.IsVisible  = true; // Optionally set visibility
        }

        return true;
    }

    private bool IsPositionValid(Vector3Int position)
    {
        return position.x >= 0 && position.x < width &&
               position.z >= 0 && position.z < depth &&
               position.y >= 0 && position.y < height &&
               !grid[position.x, position.y, position.z].IsOccupied;
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

    private bool IsLevelFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (!grid[x, y, z].IsOccupied)
                    return false;
            }
        }
        return true;
    }

    private void ClearLevel(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                var block = grid[x, y, z];
                block.IsOccupied = false;
                block.Color = Color.clear;
                block.BlockId = -1;
                block.IsVisible = false;
            }
        }
    }

    private void MoveDownLevelsAbove(int clearedLevel)
    {
        for (int y = clearedLevel + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    var currentBlock = grid[x, y, z];
                    var belowBlock = grid[x, y - 1, z];

                    belowBlock.IsOccupied = currentBlock.IsOccupied;
                    belowBlock.Color      = currentBlock.Color;
                    belowBlock.BlockId    = currentBlock.BlockId;
                    belowBlock.IsVisible  = currentBlock.IsVisible;

                    currentBlock.IsOccupied = false;
                    currentBlock.Color      = Color.clear;
                    currentBlock.BlockId    = -1;
                    currentBlock.IsVisible  = false;
                }
            }
        }
    }

    // Toggle visibility for all blocks
    public void ToggleBlockVisibility(bool visible)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    grid[x, y, z].IsVisible = visible;
                }
            }
        }
    }
}
