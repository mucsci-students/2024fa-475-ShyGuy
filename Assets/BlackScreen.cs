// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class ScreenFader : MonoBehaviour
// {
//     public Image blackScreen;
//     public float fadeSpeed = 5f;

//     public void FadeToBlack()
//     {
//         StartCoroutine(FadeCoroutine());
//     }

//     private IEnumerator FadeCoroutine()
//     {
//         Color color = blackScreen.color;
//         while (color.a < 1f)
//         {
//             color.a += Time.deltaTime * fadeSpeed;
//             blackScreen.color = color;
//             yield return null;
//         }
//     }
// }
