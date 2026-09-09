using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadein : MonoBehaviour
{
 
    public Material material; // Reference to the material you want to fade out
    public float fadeSpeed = 0.5f; // Speed of the fade-out (adjust as needed)

    private Color originalColor; // Stores the original color of the material
    private bool isFading = false; // Tracks if the fade-out is in progress

    void Start()
    {
        // Store the original color of the material
        originalColor = material.color;
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartFadeOut();
        }

        
        if (isFading)
        {
            FadeMaterial();
        }
    }

   
    public void StartFadeOut()
    {
        isFading = true;
    }

   
    private void FadeMaterial()
    {
        
        Color currentColor = material.color;

        // Reduce the alpha value over time
        currentColor.a -= fadeSpeed * Time.deltaTime;

        // Clamp the alpha value to ensure it doesn't go below 0
        currentColor.a = Mathf.Clamp(currentColor.a, 0f, 1f);

        // Apply the new color to the material
        material.color = currentColor;

        // Stop fading when the material is fully transparent
        if (currentColor.a <= 0f)
        {
            isFading = false;
            Debug.Log("Fade-out complete!");
        }
    }
}

