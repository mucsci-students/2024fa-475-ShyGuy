using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    public Image blackScreen; // Assign this in the inspector with the black screen UI Image.
    public Animator animator; // Assign this in the inspector with the Animator controlling the desired animation.

    private bool isAnimationPlaying; // Track animation state to prevent repeated triggering

    private void Start()
    {
        EnsureAssignments(); // Validate and assign references
        SetVisibility(false); // Ensure black screen starts invisible
        animator.enabled = false;
    }

    private void EnsureAssignments()
    {
        if (blackScreen == null)
        {
            Debug.LogError("BlackScreen Image not assigned in inspector!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not assigned in inspector!");
        }
    }

    // Call this method to show the black screen and play the animation (once)
    public void ShowGameOver()
    {
        SetVisibility(true); // Make the black screen visible.
        Debug.Log("screen visible and animation played.");
    }

    private void SetVisibility(bool visible)
    {
        if (blackScreen != null)
        {
            blackScreen.enabled = visible;
        }
        else
        {
            Debug.LogError("BlackScreen Image not assigned");
        }
    }

    public void PlayAnimation()
    {
        animator.enabled = true;
        
        
    }
}