using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log(directionToOrigin);

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
            Shape.Instance.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // Move backward
            Shape.Instance.transform.position -= new Vector3(1.0f, 0.0f, 0.0f);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // Move left
            Shape.Instance.transform.position += new Vector3(0.0f, 0.0f, 1.0f);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // Move right
            Shape.Instance.transform.position -= new Vector3(0.0f, 0.0f, 1.0f);
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
    }
}