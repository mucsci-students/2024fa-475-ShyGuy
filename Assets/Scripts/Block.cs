using UnityEngine;

public class Block
{
    public Color Color { get; set; } = Color.clear; // Default invisible
    public long BlockId { get; set; } = -1; // -1 means no block
    public bool IsVisible { get; set; } = true; // Controls visibility of the block
    public GameObject Cube { get; set; } // Corresponding cube GameObject

    public Block(Color color = default, long blockId = -1, bool isVisible = true)
    {
        Color = color;
        BlockId = blockId;
        IsVisible = isVisible;
        Cube = null; // Cube will be assigned when the block is created
    }
}
