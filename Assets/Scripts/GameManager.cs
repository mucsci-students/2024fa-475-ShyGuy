using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Camera rotation
    private Vector3 lastMousePosition;
    private bool rightMouseButtonPressed;

    // Whether or not the player can use keys at this moment
    public bool playerKeyControlled = true;

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

    void Update()
    {
        CheckMouseInput();

        if (playerKeyControlled)
        {
            CheckKeyInput();
        }
    }

    public void CheckMouseInput()
    {
        bool isRightMouseButtonPressed = Input.GetMouseButton(1);
        Vector3 mousePosition = Input.mousePosition;

        // Only proceed if right mouse button is pressed this and last frames
        if (isRightMouseButtonPressed && rightMouseButtonPressed)
        {
            Vector3 mouseDelta = mousePosition - lastMousePosition;

            // Convert mouse movement to camera rotation (spherical coordinates)
            float rotationX = -mouseDelta.y * (Time.deltaTime * 250.0f);
            float rotationY = mouseDelta.x * (Time.deltaTime * 250.0f);

            // Apply rotation around the origin (transform the camera's rotation)
            Vector3 focus = Vector3.zero;
            Camera.main.transform.RotateAround(focus, Camera.main.transform.right, rotationX);
            Camera.main.transform.RotateAround(focus, Vector3.up, rotationY);
        }
        lastMousePosition = mousePosition;
        rightMouseButtonPressed = isRightMouseButtonPressed;

        // Get the scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f) // Ensure noticeable input
        {
            // Get the camera transform
            Transform cameraTransform = Camera.main.transform;

            // Calculate the new position by moving the camera closer or farther from the origin
            Vector3 directionToOrigin = cameraTransform.position.normalized;
            float zoomSpeed = 10.0f; // Adjust this value to control zoom speed
            float newDistance = Mathf.Clamp(cameraTransform.position.magnitude - scroll * zoomSpeed, 2.0f, 10000.0f);

            // Set the camera's position
            cameraTransform.position = directionToOrigin * newDistance;

            // Ensure the camera still looks at the origin
            cameraTransform.LookAt(Vector3.zero);
        }
    }

    public void CheckKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Shape.Instance.GetNewShape();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            // Move forward
            MoveShape(new(0, 0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // Move backward
            MoveShape(new(0, 0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // Move left
            MoveShape(new(-1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // Move right
            MoveShape(new(1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            // Rotate horizontally
            Shape.Instance.RotateHorizontally();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            // Rotate vertically
            Shape.Instance.RotateVertically();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            // Place shape
            Base.Instance.PlaceBlock();
        }
    }

    public void MoveShape(Vector3Int movement)
    {
        Vector3Int newPos = Shape.Instance.currentPos + movement;
        if (newPos.x < 0 || newPos.x + 1 >= Base.Instance.width ||
            newPos.z < 0 || newPos.z + 1 >= Base.Instance.depth)
        {
            // Out of bounds
            return;
        }
        int heightDiff = int.MaxValue;
        for (int x = 0; x < 2; ++x)
        {
            for (int z = 0; z < 2; ++z)
            {
                Vector3Int pos = Shape.Instance.currentPos + new Vector3Int(x, 0, z);
                heightDiff = Mathf.Min(heightDiff,
                    pos.y + Shape.Instance.LowestY(x, z) - Base.Instance.HighestY(pos.x, pos.z));
            }
        }
        if (heightDiff < 6)
        {
            // Ensure 5 unit spaces between closest y
            newPos.y += 6 - heightDiff;
        }
        Shape.Instance.currentPos = newPos;
        Shape.Instance.transform.position = newPos;
    }
}