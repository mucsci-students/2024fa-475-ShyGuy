using UnityEngine;

public class Brick : MonoBehaviour
{
    private TetrisManager tetrisManager;
    private float dropInterval = 1f; // Time between automatic drops
    private float dropTimer = 1f;
    private float inputTimer = 1f;


    private bool isSettled = false;

    public void Initialize(TetrisManager manager)
    {
        tetrisManager = manager; // Link the TetrisManager
    }

    void Update()
    {
        if (isSettled) return;
        dropTimer += Time.deltaTime;
        inputTimer += Time.deltaTime;
        if (dropTimer >= dropInterval)
        {
            dropTimer = 0f;
            Drop();
        }
        HandleInput();
        
    }

    private void Drop()
    {
        transform.position += Vector3.down;        
    }

    private void HandleInput()
    {
        Debug.Log("Handling Input..."); // Add this line
        // Horizontal Movement
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector3.right);
        }

        // Rotation
        if (Input.GetKeyDown(KeyCode.W))
        {
            Rotate(90); // Rotate clockwise
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Rotate(-90); // Rotate counterclockwise
        }
    }


    private void Move(Vector3 direction)
    {
        transform.position += direction;

        // If the new position is invalid, undo the move
        if (!tetrisManager.IsPositionValid(GetChildPositions()))
        {
            transform.position -= direction;
        }
        else
        {
            inputTimer = 0f; // Reset input cooldown
        }
    }

    private void RotateWithWallKick(float angle)
    {
        transform.Rotate(0, 0, angle);

        if (!tetrisManager.IsPositionValid(GetChildPositions()))
        {
            // Try small adjustments (wall kicks)
            Vector3[] adjustments = { Vector3.left, Vector3.right, Vector3.down };

            foreach (var adjustment in adjustments)
            {
                transform.position += adjustment;

                if (tetrisManager.IsPositionValid(GetChildPositions()))
                {
                    Debug.Log("Wall kick applied!");
                    return; // Valid position found
                }

                transform.position -= adjustment; // Undo adjustment if still invalid
            }

            // Undo rotation if no valid position found
            transform.Rotate(0, 0, -angle);
        }
    }


    private void Rotate(float angle)
    
    {
        // Apply rotation
        transform.Rotate(0, 0, angle);

        // Check if the new rotation is valid
        if (!tetrisManager.IsPositionValid(GetChildPositions()))
        {
            transform.Rotate(0, 0, -angle); // Undo rotation if invalid
        }
        else
        {
            Debug.Log($"Rotated {angle} degrees successfully!");
        }
    }

    private void Settle()
    {
        isSettled = true;
        tetrisManager.UpdateGrid(transform); // Notify TetrisManager of final positions
        tetrisManager.currentBrick = null; // Allow spawning of next brick
    }

    private Vector3[] GetChildPositions()
    {
        Vector3[] positions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            positions[i] = transform.GetChild(i).position;
        }
        return positions;
    }
}
