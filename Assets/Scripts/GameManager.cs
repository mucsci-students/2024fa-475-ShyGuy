using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    // Camera rotation
    private Vector3 lastMousePosition;
    private bool rightMouseButtonPressed;
    public bool skipHere;

    public Airplane airplane;

    public Base gamebase;

    // Whether or not the player can use keys at this moment
    public bool playerKeyControlled = true;

    public Vector3Int forwardDirection = Vector3Int.forward;
    public Vector3Int backDirection = Vector3Int.back;
    public Vector3Int leftDirection = Vector3Int.left;
    public Vector3Int rightDirection = Vector3Int.right;

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
        gamebase = FindObjectOfType<Base>();
        airplane = FindObjectOfType<Airplane>();

    }

    void Update()
    {
        CheckMouseInput();

        UpdateMoveDirection();

        if (playerKeyControlled)
        {
            CheckKeyInput();
        }
        if(gamebase.gameOver)
            GameOver();
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
            float rotationX = -mouseDelta.y * (Time.deltaTime * 150.0f);
            float rotationY = mouseDelta.x * (Time.deltaTime * 150.0f);

            // Apply rotation around the origin (transform the camera's rotation)
            Vector3 focus = Base.Instance.Center();
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
            skipHere = true;
            Shape.Instance.GetNewShape();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // Clear the bottom level
            Base.Instance.ClearLevelAnyway(0);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            // Move forward
            MoveShape(forwardDirection);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // Move backward
            MoveShape(backDirection);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // Move left
            MoveShape(leftDirection);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // Move right
            MoveShape(rightDirection);
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

    public void UpdateMoveDirection()
    {
        // Get the camera's forward and right direction
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Normalize the vectors (optional, to ensure uniform movement speed)
        cameraForward.y = 0; // Remove vertical component to keep movement in the XZ plane
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        // Check camera facing right
        if (Vector3.Dot(cameraForward, Vector3.right) > 0.5f) // Camera is facing roughly to the right
        {
            forwardDirection = Vector3Int.right; // Move right
            backDirection = Vector3Int.left;     // Move left
            leftDirection = Vector3Int.forward;  // Move forward
            rightDirection = Vector3Int.back;    // Move backward
            Base.Instance.ResetArrowColor("right");
        }
        // Check camera facing left
        else if (Vector3.Dot(cameraForward, Vector3.left) > 0.5f) // Camera is facing roughly to the left
        {
            forwardDirection = Vector3Int.left;  // Move left
            backDirection = Vector3Int.right;    // Move right
            leftDirection = Vector3Int.back;     // Move backward
            rightDirection = Vector3Int.forward; // Move forward
            Base.Instance.ResetArrowColor("left");
        }
        // Check camera facing forward
        else if (Vector3.Dot(cameraForward, Vector3.forward) > 0.5f) // Camera is facing forward
        {
            forwardDirection = Vector3Int.forward;  // Move forward
            backDirection = Vector3Int.back;        // Move backward
            leftDirection = Vector3Int.left;        // Move left
            rightDirection = Vector3Int.right;      // Move right
            Base.Instance.ResetArrowColor("forward");
        }
        // Check camera facing backward
        else if (Vector3.Dot(cameraForward, Vector3.back) > 0.5f) // Camera is facing backward
        {
            forwardDirection = Vector3Int.back;    // Move backward
            backDirection = Vector3Int.forward;    // Move forward
            leftDirection = Vector3Int.right;      // Move right
            rightDirection = Vector3Int.left;      // Move left
            Base.Instance.ResetArrowColor("back");
        }
    }

    public void MoveShape(Vector3Int movement)
    {
        Vector3Int newPos = Shape.Instance.currentPos + movement;
        if (newPos.x < 0 || newPos.x + 1 >= Base.Instance.width ||
            newPos.z < 0 || newPos.z + 1 >= Base.Instance.depth)
        {
            // Out of bounds
            TextDisplay.Instance.TriggerText("Maybe I should try to rotate it?(Press Q/E)");
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

    public void GameOver()
    {
        PlaneCamera cam = FindObjectOfType<PlaneCamera>();

        if (cam == null)
        {
            Debug.LogError("PlaneCamera not found in the scene.");
            return;
        }

        Transform target = cam.targetTransform;
        if (target == null)
        {
            Debug.LogError("PlaneCamera's targetTransform is null.");
            return;
        }

        cam.Move(target);
        airplane.ActivateAirplane();
        SoundManager ap = FindObjectOfType<SoundManager>();
        if (ap != null)
        {
            ap.PlayPlaneSound();
        }
    }

}