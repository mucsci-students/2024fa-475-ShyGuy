using UnityEngine;

public class Airplane : MonoBehaviour
{
    public float speed = 20;
    private bool isActive = false; // Controls whether the airplane is moving

    private Renderer[] renderers;

    void Start()
    {
        // Set the starting position
        transform.position = new Vector3(-150, 32, 0);
        transform.rotation = Quaternion.Euler(8f, 90f, 0f);

        // Cache all renderers and disable visibility
        renderers = GetComponentsInChildren<Renderer>();
        SetVisibility(false); // Hide the airplane initially
    }

    void Update()
    {
        // Only move if active
        if (isActive)
        {
            float moveAmount = 3 * speed * Time.deltaTime;
            float rotateAmount = 4 * Time.deltaTime;
            transform.Translate(Vector3.forward * moveAmount);
            transform.Rotate(Vector3.forward * rotateAmount);
        }
    }

    // Method to activate the airplane
    public void ActivateAirplane()
    {
        isActive = true;
        SetVisibility(true); // Make the airplane visible
        Debug.Log("Airplane visible and moving");
    }

    // Method to deactivate the airplane
    public void DeactivateAirplane()
    {
        isActive = false;
        Debug.Log("Airplane stopped.");
    }

    // Helper method to control visibility
    private void SetVisibility(bool visible)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }
}
