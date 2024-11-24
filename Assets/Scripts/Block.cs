using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public bool IsOccupied { get; set; } = false;
    public Color Color { get; set; } = Color.clear; // Default invisible
    public long BlockId { get; set; } = -1; // -1 means no block

    public bool IsVisible { get; set; } = true; // Controls visibility of the block

    public Block(bool isOccupied = false, Color color = default, int blockId = -1, bool isVisible = true)
    {
        IsOccupied = isOccupied;
        Color = color;
        BlockId = blockId;
        IsVisible = isVisible;
    }
}
