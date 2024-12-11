using UnityEngine;

public class PlaneCamera : MonoBehaviour
{
    public Transform targetTransform;
    public float positionSpeed = 5f;
    public float rotationSpeed = 5f;

    private bool moving = false;

    void Start()
    {
        // Create a new target object dynamically
        GameObject targetObject = new GameObject("CameraTarget");
        targetTransform = targetObject.transform;

        // Set its position and rotation
        targetTransform.position = new Vector3(0f, 15f, 2f);
        targetTransform.rotation = Quaternion.Euler(0f, -85f, 0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Example trigger for testing
        {
            Move(targetTransform);
        }

        if (moving)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, positionSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetTransform.position) < 0.01f &&
                Quaternion.Angle(transform.rotation, targetTransform.rotation) < 0.1f)
            {
                moving = false;
                Debug.Log("Camera movement completed!");
            }
        }
    }

    public void Move(Transform newTarget)
    {
        targetTransform = newTarget;
        moving = true;
    }

    
}
